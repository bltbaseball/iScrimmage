using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iScrimmage.Core.Data.Mapping;

namespace iScrimmage.Core.Data.Extensions
{
    public static class PropertyMapExtensions
    {
        public static string ToWhereClause(this PropertyMap mapping)
        {
            return ToWhereClause(new[] {mapping});
        }

        public static string ToWhereClause(this IEnumerable<PropertyMap> mappings)
        {
            var where = new StringBuilder();
            var conjunction = "";
            
            foreach(var property in mappings)
            {
                where.AppendFormat(" {0} [{1}] = @{2}", conjunction, property.Name, property.Name);
                conjunction = "and";
            }

            return where.ToString();
        }

        public static IDictionary<string, object> GetValues(this IEnumerable<PropertyMap> mappings, object entity)
        {
            var values = new Dictionary<string, object>();

            foreach (var property in mappings)
            {
                values.Add(property.Name, property.Property.GetValue(entity));
            }

            return values;
        }
    }
}
