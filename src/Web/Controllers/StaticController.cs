using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class StaticController : Controller
    {
        public ActionResult StaticView(string page)
        {
            return View(page);
        }
    }
}