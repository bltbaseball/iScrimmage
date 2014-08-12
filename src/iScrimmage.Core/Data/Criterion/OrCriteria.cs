using System;
using Dapper;

namespace iScrimmage.Core.Data.Criterion
{
    public class OrCriteria : ICriteria
    {
        public ICriteria Left { get; set; }
        public ICriteria Right { get; set; }

        public OrCriteria(ICriteria left, ICriteria right)
        {
            Left = left;
            Right = right;

            Parameters = new DynamicParameters(left.Parameters);
            Parameters.AddDynamicParams(right.Parameters);
        }

        public string ToSql()
        {
            return String.Format("({0} or {1})", Left.ToSql(), Right.ToSql());
        }

        public DynamicParameters Parameters { get; protected set; }
    }
}