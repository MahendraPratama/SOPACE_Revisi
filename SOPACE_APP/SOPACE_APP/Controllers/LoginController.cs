using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("Home")]
    public class LoginController : Controller
    {
        static private ACEEntities sopace = new ACEEntities();
        
        Encryptor encryptor = new Encryptor();
        // GET: Login
        
        [Route(""),ActionName("Index")]
        public ActionResult Index()
        {            
            return View("HalamanLogin");
        }

        [HttpGet]
        [ActionName("CekLogin")]
        [Route("CekLogin")]
        public ActionResult CekAlreadyLogin()
        {
            string url="";
            string sessionRole = "";
            if(Session["user_role"] != null)
            {
                sessionRole = Session["user_role"].ToString();
                if (sessionRole.Contains("admin"))
                {
                    url = "admin";
                }
                else
                {
                    url = "user";
                }
            }
            return Json(url, JsonRequestBehavior.AllowGet);
        }
     
        [HttpPost]
        [ActionName("Login")]
        [Route("login")]
        public ActionResult Login(user usr)
        {
            int cekUserNonAktif = sopace.users.Join(sopace.personal_information, us => us.NIP, pi => pi.NIP,
                    (us, pi) => new
                    {
                        us.username,
                        pi.status_aktif
                    }
                ).Where(cek => cek.username == usr.username && cek.status_aktif == "nonaktif").Count();
            string retRole = "";
            try
            {
                retRole = sopace.pr_CekLogin(usr.username, encryptor.Encrypt(usr.password)).ElementAtOrDefault(0);
                Debug.Write("encrypted pwd ==> "+ encryptor.Encrypt(usr.password));
            }
            catch (NullReferenceException)
            {
                retRole = "Connection Error Please Try Again";
            }
            
            if (retRole == null)
            {
                retRole = "Password Salah";
            }
            else if (cekUserNonAktif > 0)
            {
                retRole = "User Non Aktif";
            }
            else if (retRole.Contains("admin") || retRole.EndsWith("user"))
            {
                var jabatan = sopace.users.Where(n => n.username == usr.username).Join(sopace.personal_information, n => n.NIP,
                                                                pi => pi.NIP, (n, pi) => pi.posisi).FirstOrDefault();
                string nip = sopace.users.Where(n => n.username == usr.username).Select(n => n.NIP).First();

                Session["user_role"] = retRole;
                Session["user_nip"] = nip;
                Session["jabatan"] = jabatan;

                if (retRole.Contains("user"))
                {
                    sopace.pr_RenewalClaim(nip);
                    sopace.pr_KuotaCuti(nip);
                }
            }
            return Json(retRole);           
        }


        [HttpGet, Route("Logout")]
        public ActionResult Logout()
        {
            Session.RemoveAll();
            return RedirectToAction("Index");
        }
    }
}