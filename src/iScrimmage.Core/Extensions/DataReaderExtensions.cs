using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Extensions
{
    public static class DataReaderExtensions
    {
        public static T GetValueOrDefault<T>(this IDataReader reader, int column)
        {
            object value = reader[column];

            T returnValue = default(T);

            if (value is DBNull && typeof(T) == typeof(DateTime))
            {
                value = DateTime.Parse(SqlDateTime.MinValue.ToString());
            }
                
            returnValue = (T)Convert.ChangeType(value, typeof(T));

            return returnValue;
        }

        public static object GetValueOrNull<T>(this IDataReader reader, int column)
        {
            object value = reader[column];

            T returnValue = default(T);

            if (value is DBNull)
            {
                return null;
            }

            returnValue = (T)Convert.ChangeType(value, typeof(T));

            return returnValue;
        }
    }
}
