using iScrimmage.Core.Data.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace iScrimmage.Core.Data.Configuration
{
    public class ConnectionConfiguration
    {
        public const string DefaultInstanceName = "Default";

        public IList<InstanceConnectionConfiguration> Instances { get; protected set; }

        public ConnectionConfiguration()
        {
            Instances = new List<InstanceConnectionConfiguration>();
        }

        public string GetConnection()
        {
            return GetConnection(DataEnvironment.Main);
        }

        public string GetConnection(DataEnvironment env)
        {
            return GetConnection(DefaultInstanceName, env);
        }

        public string GetConnection(string instance, DataEnvironment env)
        {
            var config = Instances.FirstOrDefault(x => x.Name.ToLower() == instance.ToLower());
            if (config == null)
            {
                config = Instances.FirstOrDefault(x => x.Name == DefaultInstanceName);
                if (config == null)
                {
                    throw new ConnectionConfigurationException(instance);
                }
            }

            return config.GetConnection(env);
        }

        public static ConnectionConfiguration LoadConfiguration()
        {
            var config = new ConnectionConfiguration();

            foreach (ConnectionStringSettings conn in ConfigurationManager.ConnectionStrings)
            {
                var name = conn.Name.Split(new[]{"."},2, StringSplitOptions.RemoveEmptyEntries);
                if (name.Length == 2)
                {
                    config.AddConnection(name[0], name[1], conn.ConnectionString);
                }
            }

            return config;
        }

        private  void AddConnection(string instanceName, string environment, string connectionString)
        {
            var instance = Instances.Count == 0? null: Instances.FirstOrDefault(i => i.Name == instanceName);
            DataEnvironment env;
            
            if (instance == null)
            {
                instance = new InstanceConnectionConfiguration() {Name = instanceName};
                Instances.Add(instance);
            }

            if (!Enum.TryParse(environment, true, out env))
            {
                env = DataEnvironment.Main;
            }

            instance.Environments[env] = connectionString;
        }
    }

    public class InstanceConnectionConfiguration
    {
        public InstanceConnectionConfiguration()
        {
            Environments = new Dictionary<DataEnvironment, string>();
        }

        public String Name { get; set; }

        public string GetConnection(DataEnvironment env)
        {
            return Environments.ContainsKey(env) ? Environments[env] : Environments[DataEnvironment.Main];
        }

        public IDictionary<DataEnvironment, string> Environments { get; protected set;} 
       
    }
}
