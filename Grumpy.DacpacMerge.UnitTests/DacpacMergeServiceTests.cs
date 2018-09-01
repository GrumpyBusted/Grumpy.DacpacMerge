using Grumpy.DacpacMerge.Interfaces;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace Grumpy.DacpacMerge.UnitTests
{
    public class DacpacMergeServiceTests
    {
        [Fact]
        public void InputSchemasShouldGetObjectsForSpecificSchema()
        {
            const string source = "MSSQL";
            const string name = "TestDB";
            var logger = NullLogger.Instance;
            var packageFactory = Substitute.For<IPackageFactory>();
            var modelFactory = Substitute.For<IModelFactory>();
            var model = Substitute.For<IModel>();
            modelFactory.Create(source, name).Returns(model);
            IDacpacMergeService cut = new DacpacMergeService(logger, packageFactory, modelFactory);

            cut.Merge("input.dacpac", source, name, new []{"A", "B"}, "User", "output.dacpac");

            model.GetObjects(new []{"A", "B"}, false).Received(1);
        }
    }
}
