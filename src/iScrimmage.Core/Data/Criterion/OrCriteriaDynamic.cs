using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;

namespace iScrimmage.Core.Data.Criterion
{
    public class OrCriteriaDynamic: ICriteria
    {  
        public OrCriteriaDynamic(dynamic param)
        {
           
            if (param is DynamicParameters)
            {
                Parameters = param as DynamicParameters;
            }
            else
            {
                Parameters = new DynamicParameters();

                var dict = param as IEnumerable<KeyValuePair<string, object>>;
                if (dict != null)
                {
                    foreach (var kvp in dict)
                    {
                        Parameters.Add(kvp.Key, kvp.Value);
                    }
                }
                else
                {
                    var t = param.GetType();
                    foreach (var prop in t.GetProperties())
                    {
                        Parameters.Add(prop.Name, prop.GetValue(param));
                    }
                }
            }
        }

        public DynamicParameters Parameters { get; protected set; }

        public string ToSql()
        {
            var clauses = Parameters.ParameterNames.ToList().Select(p => String.Format("[{0}] = @{1}", p, p));

            return "(" + String.Join(" or ", clauses) + ")";
        }
    }
}