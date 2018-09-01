using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using FluentAssertions;
using Grumpy.DacpacMerge.IntegrationTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.SqlServer.Dac;
using System;
using System.IO;
using System.Text;
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
            DatabaseHelper.DeployDacpac("Database1", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database3", "SchemaB", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplySameDefinition_ShouldNotAlterDatabase()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database2", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyNewSchemaToExistingDatabase_ShouldCreateNewSchema()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database3", "SchemaB", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithChangedTable_ShouldAlterTable()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database4", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithPostDeploymentScript_ShouldAddPostDeployScript()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database5", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithFunction_ShouldAddFunction()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database7", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithRole_ShouldAddRole()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database8", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyMultipleSchemas_ShouldChangeBothSchemas()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database6", "SchemaA;SchemaC", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyAllSchemas_ShouldChangeBothSchemas()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database6", "", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void ApplyWithChangedItemOutsideSchema_ShouldChangeItem()
        {
            DatabaseHelper.DeployDacpac("Database2", _databaseName);

            var script = TestRunnerHelper.ExecuteTest("Database9", "SchemaA", _databaseName);

            Approvals.Verify(script);
        }

        [Fact]
        public void TestDefaultArguments()
        {
            var dacpacFile = $"Test_{Guid.NewGuid().ToString()}.dacpac";

            try
            {
                DatabaseHelper.DeployDacpac("Database1", _databaseName);
                var logger = NullLogger.Instance;
                var modelFactory = new ModelFactory(logger);
                var packageFactory = new PackageFactory(logger, modelFactory);
                var service = new DacpacMergeService(logger, packageFactory, modelFactory);
                var inputFile = string.Format(TestRunnerHelper.DacpacLocation, "Database3");
                File.Copy(inputFile, dacpacFile);

                service.Merge(dacpacFile, DatabaseHelper.DatabaseSource, _databaseName);

                using (var package = System.IO.Packaging.Package.Open(dacpacFile, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(package.GetPart(new Uri("/model.xml", UriKind.Relative)).GetStream(FileMode.Open), Encoding.UTF8))
                    {
                        reader.ReadToEnd().Should().Contain("<Element Type=\"SqlSchema\" Name=\"[SchemaB]\">");
                    }
                }
            }
            finally 
            {
                File.Delete(dacpacFile);
            }
        }
    }
}
