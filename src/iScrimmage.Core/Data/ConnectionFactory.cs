using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iScrimmage.Core.Common;

namespace iScrimmage.Core.Data
{
    public class ConnectionFactory : IConnectionFactory
    {
        private readonly IUserSessionProvider userSession;
        private String userInstanceName;

        public ConnectionConfiguration Instances { get; protected set; }


        public ConnectionFactory(IUserSessionProvider userSession)
        {
            this.userSession = userSession;

            Initialize();
        }

        private void Initialize()
        {
            Instances = ConnectionConfiguration.LoadConfiguration();

            var instanceName = this.userSession.ServerInstanceName;

            if (String.IsNullOrEmpty(instanceName))
            {
                instanceName = ConnectionConfiguration.DefaultInstanceName;
            }

            this.userInstanceName = instanceName;
        }

        public IDbConnection OpenConnection()
        {
            return OpenConnection(this.userInstanceName, DataEnvironment.Main);
        }

        public IDbConnection OpenConnection(DataEnvironment env)
        {
            return OpenConnection(this.userInstanceName, env);
        }

        public IDbConnection OpenConnection(String instanceName, DataEnvironment env)
        {
            if (String.IsNullOrEmpty(instanceName)) instanceName = this.userInstanceName;

            var connectionString = Instances.GetConnection(instanceName, env);

            var conn = new SqlConnection(connectionString);

            conn.Open();

            return conn;
        }
    }
}
