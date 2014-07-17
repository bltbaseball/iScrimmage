using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Web.Models;

namespace Web.Helpers
{
    public class PlayerHelper
    {
        public static double PlayerAge(DateTime? DateOfBirth)
        {
            if (!DateOfBirth.HasValue)
                return 0;
            return PlayerAge(DateOfBirth.Value);
        }
        public static double PlayerAge(DateTime DateOfBirth)
        {
            if (DateOfBirth == null)
                return 0;
            return ((DateTime.Now.AddMonths(-4).Year - DateOfBirth.AddMonths(-4).Year));
        }
        public static DateTime PlayerCutoffDate(int AgeGroup)
        {
            return new DateTime(DateTime.Now.AddMonths(-4).Year - AgeGroup, 4, 1);
        }
    }
}