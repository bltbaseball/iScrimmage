using System.Collections;
using System.Text;

namespace iScrimmage.Core.Data.Criterion
{
    public class Criteria
    {
        /// <summary>
        /// Joins two existing criteria statements into an and expression. 
        /// </summary>
        /// <remarks>Dapper ands params by default.  Use this method to and multiple complex criteria expressions and 2 Ors</remarks>
        public static AndCriteria And(ICriteria left, ICriteria right)
        {
            return new AndCriteria(left, right);
        }

        /// <summary>
        /// Property name exactly equals supplied value
        /// </summary>
        public static PropertyCriteria Eq(string propertyName, object value)
        {
            return new PropertyCriteria(propertyName, value, "=");
        }

        /// <summary>
        /// Creates a statement where the property is any of the supplied values
        /// </summary>
        public static PropertyCriteria In(string propertyName, IEnumerable values)
        {
            return new PropertyCriteria(propertyName, values, "in");
        }
        
        /// <summary>
        /// Creates a default like clause: propertyName = '%value%'
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyCriteria Like(string propertyName, string value)
        {
            return new PropertyCriteria(propertyName, "%" + value + "%", "like");
        }

        /// <summary>
        /// Creates an or statement with an option for each property in the parameters object
        /// </summary>
        /// <param name="parameters"></param>
        public static OrCriteriaDynamic Or(dynamic parameters)
        {
            return new OrCriteriaDynamic(parameters);
        }

        /// <summary>
        /// Joins two existing criteria objects into an or expression
        /// </summary>
        public static OrCriteria Or(ICriteria left, ICriteria right)
        {
            return new OrCriteria(left, right);
        }

    }
}
