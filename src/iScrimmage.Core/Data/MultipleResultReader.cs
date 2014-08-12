using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace iScrimmage.Core.Data
{
    public class MultipleResultReader
    {
        private SqlMapper.GridReader gridReader;

        internal MultipleResultReader(SqlMapper.GridReader reader)
        {
            gridReader = reader;
        }

        public IEnumerable<T> Read<T>()
        {
            return gridReader.Read<T>();
        }
    }
}
