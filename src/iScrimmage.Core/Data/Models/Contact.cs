using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iScrimmage.Core.Data.Models
{
    [Table("blt.Contact")]
    public class Contact : BaseModel
    {
        public virtual Guid MemberId { get; set; }
        public virtual String Type { get; set; }
        public virtual String PhoneNumber { get; set; }
        public virtual String Address { get; set; }
        public virtual String City { get; set; }
        public virtual String State { get; set; }
        public virtual String Zip { get; set; }
    }

}
