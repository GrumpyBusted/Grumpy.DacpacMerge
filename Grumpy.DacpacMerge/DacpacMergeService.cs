using Grumpy.Common.Extensions;
using Grumpy.DacpacMerge.Interfaces;
using Grumpy.Logging;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Grumpy.DacpacMerge
{
    public class DacpacMergeService : IDacpacMergeService
    {
        private readonly ILogger _logger;
        private readonly IPackageFactory _packageFactory;
        private readonly IModelFactory _modelFactory;

        public DacpacMergeService(ILogger logger, IPackageFactory packageFactory, IModelFactory modelFactory)
        {
            _logger = logger;
            _packageFactory = packageFactory;
            _modelFactory = modelFactory;
        }

        public void Merge(string inputDacpacFileName, string databaseSource, string databaseName, IEnumerable<string> databaseSchemas = null, string schemaOwnerUser = null, string outputDacpacFileName = null)
        {
            var schemas = databaseSchemas as string[] ?? databaseSchemas?.ToArray() ?? new string[] { };

            using (var package = _packageFactory.Create(inputDacpacFileName))
            {
                var deploySchemas = schemas.Any() ? schemas.Select(s => s.Trim('[', ']')).ToList() : package.Model.Schemas.ToList();

                _logger.Information("Deploying Database Schemas {@Schemas}", deploySchemas);

                using (var databaseModel = _modelFactory.Create(databaseSource, databaseName))
                {
                    var deployObjects = package.Model.GetObjects(deploySchemas, true).ToArray();

                    var deployNames = deployObjects
                        .Union(databaseModel.GetObjects(deploySchemas, false))
                        .Where(o => o.Name.HasName)
                        .Select(o => o.Name.ToString())
                        .Distinct();

                    databaseModel.Remove(deployNames);

                    if (!schemaOwnerUser.NullOrEmpty() && databaseModel.GetUser(schemaOwnerUser) == null)
                        databaseModel.AddUser(schemaOwnerUser);

                    databaseModel.AddObjects(deployObjects, schemaOwnerUser);

                    package.Model = databaseModel;

                    var fileName = outputDacpacFileName ?? inputDacpacFileName;
                    
                    package.Save(fileName);

                    _logger.Information("New Data-tier Application Package file saved {FileName}", fileName);
                }
            }
        }
    }
}