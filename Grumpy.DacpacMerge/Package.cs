using System;
using System.IO;
using Grumpy.DacpacMerge.Interfaces;
using Grumpy.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.SqlServer.Dac;
using Microsoft.SqlServer.Dac.Model;

namespace Grumpy.DacpacMerge
{
    public class Package : IPackage
    {
        private readonly ILogger _logger;
        private readonly IModelFactory _modelFactory;
        private readonly DacPackage _package;
        private TSqlModel _model;
        private bool _disposed;

        internal Package(ILogger logger, IModelFactory modelFactory, string fileName)
        {
            _logger = logger;
            _modelFactory = modelFactory;
            _package = DacPackage.Load(fileName);
            _model = new TSqlModel(fileName);
        }

        public void Save(string fileName)
        {
            DacPackageExtensions.BuildPackage(fileName, _model, Metadata);

            if (_package.PreDeploymentScript != null || _package.PostDeploymentScript != null)
            {
                using (var package = System.IO.Packaging.Package.Open(fileName, FileMode.Open, FileAccess.ReadWrite))
                {
                    AddScript(package, _package.PreDeploymentScript, "predeploy");
                    AddScript(package, _package.PostDeploymentScript, "postdeploy");
                }
            }
        }

        private void AddScript(System.IO.Packaging.Package package, Stream source, string scriptName)
        {
            if (source != null)
            {
                var part = package.CreatePart(new Uri($"/{scriptName}.sql", UriKind.Relative), "text/plain");

                if (part != null)
                    source.CopyTo(part.GetStream(FileMode.OpenOrCreate, FileAccess.ReadWrite));
                else
                    _logger.Warning("Unable to copy deployment script to new package");
            }
        }

        public IModel Model
        {
            get => _modelFactory.Create(_model);
            set => _model = value.SqlModel;
        }

        private PackageMetadata Metadata => new PackageMetadata
        {
            Description = _package.Description,
            Name = _package.Name,
            Version = _package.Version.ToString()
        };

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!_disposed)
                {
                    _disposed = true;

                    _package.Dispose();
                    _model.Dispose();
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}