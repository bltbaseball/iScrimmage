using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Mapping
{
    /// <summary>
    /// Represents and class property that maps to another domain class
    /// </summary>
    public class ReferenceMap
    {
        public virtual String Alias { get; set; }

        public virtual String Name { get; set; }

        public virtual Type EntityType { get; set; }

        public virtual String Column{ get; set; }

        public virtual bool Nullable { get; set; }
    }
}
