using System.Collections.Generic;

namespace Grumpy.DacpacMerge.Interfaces
{
    public interface IDacpacMergeService
    {
        void Merge(string inputDacpacFileName, string databaseSource, string databaseName, IEnumerable<string> databaseSchemas, string schemaOwnerUser, string outputDacpacFileName);
    }
}