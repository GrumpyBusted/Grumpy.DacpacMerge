using Microsoft.SqlServer.Dac.Model;

namespace Grumpy.DacpacMerge.Extensions
{
    internal static class SqlObjectExtensions
    {
        public static string SchemaName(this TSqlObject obj)
        {
            return obj.Name.HasName && 
                   obj.IsSchema() ? obj.Name.Parts[0].Trim('[', ']') :
                   obj.Name.Parts.Count > 1 ? obj.Name.Parts[0].Trim('[', ']') : null;
        }

        public static bool IsSchema(this TSqlObject type)
        {
            return type.ObjectType.Name == "Schema";
        }

        public static bool IsUser(this TSqlObject type)
        {
            return type.ObjectType.Name == "User";
        }
        
        public static bool IsDatabaseOptions(this TSqlObject type)
        {
            return type.ObjectType.Name == "DatabaseOptions";
        }
    }
}