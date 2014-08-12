using System;
using Dapper;

namespace iScrimmage.Core.Data.Criterion
{
    /// <summary>
    /// Creates a where statement by comparing a property value 
    /// </summary>
    public class PropertyCriteria: ICriteria
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public string Op { get; set; }

        public PropertyCriteria(string propertyName, dynamic value, string op)
        {
            PropertyName = propertyName;
            Value = value;
            Op = op;

            Parameters = new DynamicParameters();
            Parameters.Add(PropertyName, value);
        }

        public DynamicParameters Parameters { get; protected set; }

        public string ToSql()
        {
            return String.Format("[{0}] {1} @{2}", PropertyName, Op, PropertyName);
        }
    }
}