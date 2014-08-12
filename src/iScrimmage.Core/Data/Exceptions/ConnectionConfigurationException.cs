using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Exceptions
{
    public class ConnectionConfigurationException : ApplicationException
    {
        public string InstanceName { get; private set; }

        public ConnectionConfigurationException(string instance) : base("Could not find default connection in configuration.")
        {
            InstanceName = instance;
        }
    }
}
