using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Exceptions
{
    public class RecordNotFoundException<T> : ApplicationException
    {
        private const string defaultMessage = "A '{0}' record with the id '{1}' was not found.";

        public string Id { get; set; }

        public RecordNotFoundException(string id)
            : base(String.Format(defaultMessage, typeof(T).Name, id))
        {
            Id = id;
        }

        public RecordNotFoundException(string id, string message) 
            : base(message)
        {
            Id = id;
        }
    }
}
