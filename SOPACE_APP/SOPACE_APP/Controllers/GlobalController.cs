using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    public class GlobalController : Controller
    {
        // GET: Global
        private ACEEntities sopace = new ACEEntities();
        public ActionResult Index()
        {
            return View();
        }
        public void getTempData()
        {
            

            
        }
    }
}