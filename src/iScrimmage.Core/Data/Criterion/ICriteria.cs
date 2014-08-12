using Dapper;
namespace iScrimmage.Core.Data.Criterion
{
    public interface ICriteria
    {
        /// <summary>
        /// Contains the parameters required for this criteria
        /// </summary>
        DynamicParameters Parameters { get; }

        /// <summary>
        /// Resolve the criteria expression to a sql where clause
        /// </summary>
        /// <returns></returns>
        string ToSql();
    }
}
