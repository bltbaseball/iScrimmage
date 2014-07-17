using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public enum SignStatus
    {
        NotSigned = 0,
        RequestSent = 1,
        Viewed = 2,
        Signed = 3
    }
}