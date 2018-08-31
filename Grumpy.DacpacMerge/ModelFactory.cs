using Grumpy.DacpacMerge.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Dac.Model;

namespace Grumpy.DacpacMerge
{
    public class ModelFactory : IModelFactory
    {
        private readonly ILogger _logger;

        public ModelFactory(ILogger logger)
        {
            _logger = logger;
        }

        public IModel Create(string source, string name)
        {
            return new Model(_logger, source, name);
        }

        public IModel Create(TSqlModel model)
        {
            return new Model(_logger, model);
        }
    }
}