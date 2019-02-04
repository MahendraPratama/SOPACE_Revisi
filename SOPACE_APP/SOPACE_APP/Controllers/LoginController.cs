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
                string nip = sopace.users.Where(n => n.username == usr.username).Select(n => n.NIP).First();
                Session["user_role"] = retRole;
                Session["user_nip"] = nip;

                if (retRole.Contains("user"))                
                {
                    sopace.pr_RenewalClaim(nip);
                    kuota_cuti LoginUser = sopace.kuota_cuti.Where(e => e.NIP == nip).First();                    
                    var sisa_cuti = LoginUser.sisa_cuti;
                    var cuti_baru = LoginUser.cuti_baru;
                    var join_date = LoginUser.personal_information.tgl_masuk;                    

                    if (DateTime.Now.ToString("dd-MM") == join_date.Value.ToString("dd-MM") && LoginUser.status == "not update")
                    {
                        if (cuti_baru <= 0)
                        {
                            LoginUser.sisa_cuti = 0;
                            LoginUser.cuti_baru = 12 + cuti_baru;
                            LoginUser.exp_sisa_cuti = LoginUser.exp_cuti_baru;
                            LoginUser.status = "update";
                            LoginUser.exp_cuti_baru = DateTime.Now.AddMonths(18);
                            sopace.Entry(LoginUser).State = EntityState.Modified;
                            sopace.SaveChanges();
                        }
                        else
                        {
                            LoginUser.sisa_cuti = cuti_baru;
                            LoginUser.cuti_baru = 12;
                            LoginUser.status = "update";
                            LoginUser.exp_sisa_cuti = LoginUser.exp_cuti_baru;
                            LoginUser.exp_cuti_baru = DateTime.Now.AddMonths(18);
                            sopace.Entry(LoginUser).State = EntityState.Modified;
                            sopace.SaveChanges();
                        }
                    }
                    else if (DateTime.Now.ToString("dd-MM") != join_date.Value.ToString("dd-MM"))
                    {
                        LoginUser.status = "not update";
                        if (DateTime.Now >= LoginUser.exp_sisa_cuti.Value)
                        {
                            LoginUser.sisa_cuti = 0;
                        }
                        sopace.Entry(LoginUser).State = EntityState.Modified;
                        sopace.SaveChanges();
                    }
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