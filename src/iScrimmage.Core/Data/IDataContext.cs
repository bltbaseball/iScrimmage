using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data
{
    public interface IDataContext
    {
        IConnectionFactory ConnectionFactory { get; }

        void BeginTransaction(IsolationLevel isolation = IsolationLevel.ReadCommitted);
        void CommitTransaction();
        void RollbackTransaction();
        void Insert<T>(dynamic data);
        int Update<T>(dynamic id, dynamic data);
        bool Delete<T>(T entityToDelete);
        T Get<T>(dynamic id) where T : class;
        IEnumerable<T> Query<T>(string sql, dynamic parameters);
        T Execute<T>(IQuerySpec<T> spec) where T : class;
        MultipleResultReader QueryMultiple(string sql, dynamic parameters);
    }
}
