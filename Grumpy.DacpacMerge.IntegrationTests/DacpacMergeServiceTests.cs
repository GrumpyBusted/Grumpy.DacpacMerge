using System;
using System.IO;
using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Grumpy.DacpacMerge.IntegrationTests.Helpers;
using Microsoft.SqlServer.Dac;
using Xunit;

namespace Grumpy.DacpacMerge.IntegrationTests
{
    [UseApprovalSubdirectory("Approvals")]
    [UseReporter(typeof(DiffReporter))]
    public sealed class DacpacMergeServiceTests : IDisposable
    {
        private readonly string _databaseName;

        public DacpacMergeServiceTests()
        {
            _databaseName = $"Test_Database_{Guid.NewGuid().ToString()}";
        }

        public void Dispose()
        {
            try
            {
                DatabaseHelper.DropDatabase(_databaseName);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        [Fact]
        public void ApplyToNonExistingDatabase_ShouldThrowException()
        {
            Assert.Throws<DacServicesException>(() => TestRunnerHelper.ExecuteTest("Database3", "SchemaA", _databaseName));
        }

        [Fact]
        public void ApplyNewSchemaToEmptyDatabase_ShouldCreateNewSchema()
        {
            DeployDacpac("Database1");

            var script = TestRunnerHelper.ExecuteTest("Database3", "SchemaB", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplySameDefinition_ShouldNotAlterDatabase()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database2", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyNewSchemaToExistingDatabase_ShouldCreateNewSchema()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database3", "SchemaB", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithChangedTable_ShouldAlterTable()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database4", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithPostDeploymentScript_ShouldAddPostDeployScript()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database5", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithFunction_ShouldAddFunction()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database7", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithRole_ShouldAddRole()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database8", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyMultipleSchemas_ShouldChangeBothSchemas()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database6", "SchemaA;SchemaC", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyAllSchemas_ShouldChangeBothSchemas()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database6", "", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithChangedItemOutsideSchema_ShouldChangeItem()
        {
            DeployDacpac("Database2");

            var script = TestRunnerHelper.ExecuteTest("Database9", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        private void DeployDacpac(string testDatabase)
        {
            var dacpacFile = string.Format(TestRunnerHelper.DacpacLocation, testDatabase);

            using (var package = DacPackage.Load(dacpacFile, DacSchemaModelStorageType.Memory, FileAccess.Read))
            {
                var service = new DacServices(DatabaseHelper.ConnectionString(_databaseName));

                service.Deploy(package, _databaseName);
            }
        }
    }
}
