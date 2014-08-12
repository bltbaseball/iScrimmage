using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Mapping
{
    /// <summary>
    /// Specifies the details of a join between two tables to create a composite entity like when using a subClass
    /// </summary>
    public class JoinPart
    {
        public String TableName { get; set; }

        public String KeyColumn { get; set; }
    }
}
