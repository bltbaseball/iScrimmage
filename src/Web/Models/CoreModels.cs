using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Web.WebPages.OAuth;
using NHibernate.Criterion;

namespace Web.Models
{
    // Models now in their own separate class files.

    public static class Transforms
    {
        public static SelectList ToSelectList<TEnum>(this TEnum enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };

            return new SelectList(values, "Id", "Name", enumObj);
        }

        public static SelectList ToSelectList<TEnum>(this IList<TEnum> enumObj)
        {
            var values = from TEnum e in Enum.GetValues(typeof(TEnum))
                         select new { Id = e, Name = e.ToString() };

            return new SelectList(values, "Id", "Name", enumObj);
        }

    }

    public enum CellPhoneCarrier
    {
        ATT,
        TMobile,
        VirginMobile,
        Cingular,
        Sprint,
        Verizon,
        MetroPCS,
        Nextel
    }
}