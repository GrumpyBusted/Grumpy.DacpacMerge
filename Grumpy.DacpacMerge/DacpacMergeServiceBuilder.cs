using Grumpy.DacpacMerge.Interfaces;
using Microsoft.Extensions.Logging;

namespace Grumpy.DacpacMerge
{
    public class DacpacMergeServiceBuilder
    {
        private readonly ILogger _logger;

        public DacpacMergeServiceBuilder(ILogger logger)
        {
            _logger = logger;
        }

        public IDacpacMergeService Build()
        {
            var modelFactory = new ModelFactory(_logger);
            var packageFactory = new PackageFactory(_logger, modelFactory);

            return new DacpacMergeService(_logger, packageFactory, modelFactory);
        }
    }
}