using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Generators
{
    public interface IIdentifyGenerator
    {
        object Generate(object entity);
    }
}
