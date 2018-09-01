using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.SqlServer.Dac;

namespace Grumpy.DacpacMerge.IntegrationTests.Helpers
{
    internal static class DatabaseHelper
    {
        public const string DatabaseSource = @"(localdb)\MSSQLLocalDB";
        
        public static void DeployDacpac(string testDatabase, string databaseName)
        {
            var dacpacFile = string.Format(TestRunnerHelper.DacpacLocation, testDatabase);

            using (var package = DacPackage.Load(dacpacFile, DacSchemaModelStorageType.Memory, FileAccess.Read))
            {
                var service = new DacServices(ConnectionString(databaseName));

                service.Deploy(package, databaseName);
            }
        }

        public static string ConnectionString(string databaseName)
        {
            return new SqlConnectionStringBuilder
            {
                InitialCatalog = databaseName,
                DataSource = DatabaseSource,
                IntegratedSecurity = true
            }.ConnectionString;
        }

        private static SqlConnection OpenSqlConnection(string databaseName)
        {
            var connection = new SqlConnection(ConnectionString(databaseName));

            connection.Open();

            Execute(connection, "SET LOCK_TIMEOUT 90000");

            return connection;
        }

        public static void DropDatabase(string databaseName)
        {
            if (DatabaseExist(databaseName))
            {
                using (var connection = OpenSqlConnection("master"))
                {
                    Execute(connection, $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
                    Execute(connection, $"DROP DATABASE [{databaseName}]");
                }
            }
        }
        
        private static bool DatabaseExist(string databaseName)
        {
            using (var connection = OpenSqlConnection("master"))
            {
                return DatabaseExist(connection, databaseName);
            }
        }

        private static bool DatabaseExist(SqlConnection connection, string databaseName)
        {
            return (int)ExecuteScalar(connection, $"SELECT COUNT(*) FROM [sys].[databases] WHERE [name] = '{databaseName}'") == 1;
        }

        private static void Execute(SqlConnection connection, string commandText, int commandTimeOut = 0)
        {
            using (var command = GetCommandObject(connection, commandText, commandTimeOut))
            {
                command.ExecuteNonQuery();
            }
        }

        private static object ExecuteScalar(SqlConnection connection, string commandText, int commandTimeOut = 30)
        {
            using (var command = GetCommandObject(connection, commandText, commandTimeOut))
            {
                return command.ExecuteScalar();
            }
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static SqlCommand GetCommandObject(SqlConnection connection, string commandText, int commandTimeOut)
        {
            var command = connection.CreateCommand();

            command.CommandTimeout = commandTimeOut;
            command.CommandText = commandText;

            return command;
        }
    }
}