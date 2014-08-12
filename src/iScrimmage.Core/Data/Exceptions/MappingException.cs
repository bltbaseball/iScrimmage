using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Exceptions
{
    public class MappingException: ApplicationException
    {
        public string Property { get; set; }

        public MappingException(string property) : base("Could recognize mapping")
        {
            Property = property;
        }

        public MappingException(string property, string message) : base(message)
        {
            Property = property;
        }
    }
}
