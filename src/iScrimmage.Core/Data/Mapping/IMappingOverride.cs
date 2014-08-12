using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Mapping
{
    public interface IMappingOverride<T> where T:class
    {
        void Override(ClassMap mapping);
    }
}
