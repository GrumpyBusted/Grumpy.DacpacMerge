using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dac.Model;

namespace Grumpy.DacpacMerge.Interfaces
{
    public interface IModel : IDisposable
    {
        void AddObjects(IEnumerable<TSqlObject> objects, string schemaOwnerUser);
        void Remove(IEnumerable<string> objectNames);
        IEnumerable<string> Schemas { get; }
        IEnumerable<TSqlObject> GetObjects(IEnumerable<string> databaseSchemas, bool includeObjectOutsideSchemas);
        TSqlObject GetUser(string user);
        void AddUser(string user);
        TSqlModel SqlModel { get; }
    }
}