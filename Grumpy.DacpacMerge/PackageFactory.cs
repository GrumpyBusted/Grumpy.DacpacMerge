using Grumpy.DacpacMerge.Interfaces;
using Microsoft.Extensions.Logging;

namespace Grumpy.DacpacMerge
{
    public class PackageFactory : IPackageFactory
    {
        private readonly ILogger _logger;
        private readonly IModelFactory _modelFactory;

        public PackageFactory(ILogger logger, IModelFactory modelFactory)
        {
            _logger = logger;
            _modelFactory = modelFactory;
        }

        public IPackage Create(string fileName)
        {
            return new Package(_logger, _modelFactory, fileName);
        }
    }
}