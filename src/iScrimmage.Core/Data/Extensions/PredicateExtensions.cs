using iScrimmage.Core.Extensions;
using System;

namespace iScrimmage.Core.Data.Extensions
{
    public static class PredicateExtensions
    {
        public static string SelectToCountSql(this string sql, string keyColumn, string countColumn = "TotalCount")
        {
            var countSql = String.Format("select count({0}) as {1} from {2}", keyColumn, countColumn, sql.SubstringAfter("from"));
            return countSql;
        }
    }
}
