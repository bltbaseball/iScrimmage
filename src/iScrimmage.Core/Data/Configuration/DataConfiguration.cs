using System;
using System.Collections.Generic;

namespace iScrimmage.Core.Data.Configuration
{
    public class DataConfiguration
    {
        public static List<Type> BasicTypes = new List<Type>
                               {
                                   typeof(byte),
                                   typeof(sbyte),
                                   typeof(short),
                                   typeof(ushort),
                                   typeof(int),
                                   typeof(uint),
                                   typeof(long),
                                   typeof(ulong),
                                   typeof(float),
                                   typeof(double),
                                   typeof(decimal),
                                   typeof(bool),
                                   typeof(string),
                                   typeof(char),
                                   typeof(Guid),
                                   typeof(DateTime),
                                   typeof(DateTimeOffset),
                                   typeof(byte[])
                               };

        public static bool IsBasicType(Type t)
        {
            Type actualType = t;
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                actualType = t.GetGenericArguments()[0];
            }

            return BasicTypes.Contains(actualType);
        }
    }
}
