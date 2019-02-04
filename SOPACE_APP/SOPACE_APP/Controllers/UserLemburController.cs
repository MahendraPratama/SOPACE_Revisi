using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("UserLembur")]
    public class UserLemburController : Controller
    {
        static private ACEEntities sopace = new ACEEntities();
        // GET: UserLembur
        [ActionName("ShowLembur")]
        public ActionResult TampilkanLemburUser()
        {
            string nips = Session["user_nip"].ToString();
            var lmbr = (from e in sopace.lemburs
                       where e.NIP == nips
                       select e).ToList();
                        
            return View("Show",lmbr);
        }
    }
}