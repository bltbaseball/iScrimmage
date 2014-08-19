using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using iScrimmage.Core.Common;
using iScrimmage.Core.Data.Extensions;
using iScrimmage.Core.Data.Mapping;
using iScrimmage.Core.Extensions;

namespace iScrimmage.Core.Data
{
    public class SqlDataContext : IDisposable, IDataContext
    {
        private IUserSessionProvider userProvider;
        private IConnectionFactory connectionFactory;
        private IDbConnection connection;
        private IDbTransaction transaction;

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        public DataEnvironment Environment { get; protected set; }

        protected IDbConnection Connection
        {
            get
            {
                if (this.connection == null || this.connection.State == ConnectionState.Closed)
                {
                    this.connection = this.connectionFactory.OpenConnection(Environment);
                }

                return this.connection;
            }

            set { this.connection = value; }
        }

        public SqlDataContext(IConnectionFactory connectionFactory, IUserSessionProvider userSessionProvider)
        {
            this.connectionFactory = connectionFactory;
            this.userProvider = userSessionProvider;

            Environment = DataEnvironment.Main;
        }

        public IConnectionFactory ConnectionFactory
        {
            get { throw new NotImplementedException(); }
        }

        public void BeginTransaction(System.Data.IsolationLevel isolation = IsolationLevel.ReadCommitted)
        {
            if (this.transaction == null)
            {
                this.transaction = Connection.BeginTransaction(isolation);
            }
        }

        public void CommitTransaction()
        {
            if (this.transaction != null)
            {
                this.transaction.Commit();
            }

            this.transaction = null;
        }

        public void RollbackTransaction()
        {
            if (this.transaction != null)
            {
                this.transaction.Rollback();
                this.transaction = null;
            }
        }

        public void Insert<T>(dynamic data)
        {
            var map = ClassMapper.GetMap(typeof(T));
            var builder = new SqlBuilderDomain(map);

            //map.AssignIdentifierValues(data)
            //   .AssignTimeStamp(data);

            var sql = builder.Insert.RawSql;

            try
            {
                Connection.Execute(sql, (object)data, this.transaction);
            }
            catch (Exception ex)
            {
                RollbackTransaction();
                throw ex;
            }
        }

        public int Update<T>(dynamic id, dynamic data)
        {
            var map = ClassMapper.GetMap(typeof(T));
            var tableName = map.Table;
            var paramNames = GetParamNames((object)data);

            var builder = new StringBuilder();
            builder.Append("update ").Append(tableName).Append(" set ");
            builder.AppendLine(string.Join(",", paramNames.Where(n => n != map.Id.ColumnName).Select(p => "[" + p + "]" + "= @" + p)));
            builder.AppendFormat("where [{0}] = @{1}", map.Id.ColumnName, map.Id.ColumnName);

            var parameters = new DynamicParameters(data);
            parameters.Add(map.Id.ColumnName, id);

            var result = Connection.Execute(builder.ToString(), parameters, this.transaction);
            return result;
        }

        public bool Delete<T>(T entityToDelete)
        {
            var type = typeof(T);
            var mapping = ClassMapper.GetMap(type);

            var name = mapping.Table;

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where {1}", name, mapping.Id.ToWhereClause());

            var deleted = Connection.Execute(sb.ToString(), entityToDelete, transaction: this.transaction);

            return deleted > 0;
        }

        public T Get<T>(dynamic id) where T : class
        {
            var type = typeof(T);
            var mapping = ClassMapper.GetMap(type);

            string sql;

            if (!GetQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var builder = new SqlBuilderDomain(mapping);
                builder.Where(mapping.Id.ColumnName + "= @id ");
                sql = builder.Select.RawSql;
                GetQueries[type.TypeHandle] = sql;
            }

            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("@id", id);

            return Connection.Query<T>(sql, dynamicParams, transaction: this.transaction).FirstOrDefault();
        }

        public IEnumerable<T> Query<T>(string sql, dynamic parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            return Connection.Query<T>(sql, dynamicParameters, transaction: this.transaction);
        }

        public T Execute<T>(IQuerySpec<T> spec) where T : class
        {
            return spec.Execute(this);
        }

        public MultipleResultReader QueryMultiple(string sql, dynamic parameters)
        {
            var dynamicParameters = new DynamicParameters(parameters);

            var reader = Connection.QueryMultiple(sql, dynamicParameters, transaction: this.transaction);

            return new MultipleResultReader(reader);
        }

        public void Dispose()
        {
            if (this.connection == null) return;

            if (this.connection.State != ConnectionState.Closed)
            {
                if (this.transaction != null)
                {
                    this.transaction.Rollback();
                }

                this.connection.Close();
                this.connection = null;
            }
        }

        internal List<string> GetParamNames(object o)
        {
            if (o is DynamicParameters)
            {
                return (o as DynamicParameters).ParameterNames.ToList();
            }

            var t = o.GetType();

            if (t.IsAnonymousType())
            {
                return t.GetProperties().Select(p => p.Name).ToList();
            }

            var mapping = ClassMapper.GetMap(o.GetType());
            return mapping.Properties.Values.Select(p => p.Name).ToList();
        }
    }
}
