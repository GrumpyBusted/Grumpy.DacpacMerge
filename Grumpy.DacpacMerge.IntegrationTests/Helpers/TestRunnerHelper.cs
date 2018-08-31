using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SqlServer.Dac;

namespace Grumpy.DacpacMerge.IntegrationTests.Helpers
{
    internal static class TestRunnerHelper
    {
#if DEBUG
        public const string DacpacLocation = @"..\..\..\Grumpy.DacpacMerge.{0}\bin\Debug\Grumpy.DacpacMerge.{0}.dacpac";
#else
        public const string DacpacLocation = @"..\..\..\Grumpy.DacpacMerge.{0}\bin\Release\Grumpy.DacpacMerge.{0}.dacpac";
#endif

        public static string ExecuteTest(string testDatabase, string databaseSchemas, string databaseName)
        {
            var fileName = $"{testDatabase}_{Guid.NewGuid().ToString()}.dacpac";

            var inputFile = string.Format(DacpacLocation, testDatabase);

            var logger = NullLogger.Instance;
            var modelFactory = new ModelFactory(logger);
            var packageFactory = new PackageFactory(logger, modelFactory);
            var service = new DacpacMergeService(logger, packageFactory, modelFactory);

            service.Build(inputFile, DatabaseHelper.DatabaseSource, databaseName, databaseSchemas != "" ? databaseSchemas.Split(';') : new string[] { }, "TestUser", fileName);

            PublishResult result;

            using (var package = DacPackage.Load(fileName, DacSchemaModelStorageType.Memory, FileAccess.Read))
            {
                var dacServices = new DacServices(DatabaseHelper.ConnectionString(databaseName));

                result = dacServices.Script(package, databaseName, new PublishOptions {GenerateDeploymentScript = true} );
            }

            File.Delete(fileName);

            var script = result.DatabaseScript.Replace(databaseName, "{DatabaseName}");

            var lines = Regex.Split(script, "\r\n|\r|\n").Where(l => !l.StartsWith(":setvar DefaultDataPath", true, CultureInfo.InvariantCulture) && !l.StartsWith(":setvar DefaultLogPath", true, CultureInfo.InvariantCulture));

            return lines.Aggregate("", (current, line) => current + line + Environment.NewLine);
        }
    }
}