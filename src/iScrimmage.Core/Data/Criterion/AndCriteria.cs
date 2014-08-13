using Dapper;
using System;

namespace iScrimmage.Core.Data.Criterion
{
    public class AndCriteria: ICriteria
    {
        public ICriteria Left { get; set; }
        public ICriteria Right { get; set; }

        public AndCriteria(ICriteria left, ICriteria right)
        {
            Left = left;
            Right = right;

            Parameters = new DynamicParameters(left.Parameters);
            Parameters.AddDynamicParams(right.Parameters);
        }

        public DynamicParameters Parameters { get; protected set; }

        public string ToSql()
        {
            return String.Format("({0} and {1})", Left.ToSql(), Right.ToSql());
        }
    }
}
