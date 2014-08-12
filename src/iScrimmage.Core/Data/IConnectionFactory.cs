using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data
{
    public interface IConnectionFactory
    {
        IDbConnection OpenConnection();
        IDbConnection OpenConnection(DataEnvironment environment);
        IDbConnection OpenConnection(String instanceName, DataEnvironment environment);
    }
}
