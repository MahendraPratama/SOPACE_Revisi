using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    public class SuperAdminController : Controller
    {
        // GET: SuperAdmin
        private ACEEntities sopace = new ACEEntities();
        Encryptor encryptor = new Encryptor();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddAdmin()
        {
            return View("InputAdmin");
        }

        [HttpPost]
        [Route("Add"),ActionName("Add")]
        public JsonResult InsertAdministrator(InsertAdmin insertAdmin)
        {
            int countadmin = sopace.users.Where(ca => 
                    ca.NIP == insertAdmin.User.NIP &&
                    ca.role.Contains("admin")
                ).Count();
            string jsonResult = "";

            if (countadmin > 0)
            {
                jsonResult = insertAdmin.User.NIP + "Already Registered to Admin";
            }
            else
            {
                insertAdmin.User.password = encryptor.Encrypt(insertAdmin.User.password);
                sopace.users.Add(insertAdmin.User);
                sopace.SaveChanges();
                jsonResult = "Insert Data Success";
            }
            
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        [ActionName("ViewAdmin")]
        public ActionResult ViewAdmin()
        {
            string admin = "admin";
            var list_admin = sopace.users.Where(u => u.role == admin).ToList();
            return View("ViewAllAdmin", list_admin);
        }

        [ActionName("Edit_Admin")]
        [Route("Edit_Admin/{id}")]
        public ActionResult Edit_Admin(string id)
        {            

            user user = sopace.users.Where(e => e.username.Equals(id)).FirstOrDefault();
            user.password = encryptor.Decrypt(user.password);

            return View("editAdmin", user);
        }

        [HttpPost, Route("UpdateAdmin"), ActionName("UpdateAdmin")]
        public JsonResult UpdateAdmin(user updateAdmin)
        {
            var old = sopace.users.Where(p => p.NIP == updateAdmin.NIP && p.role.Contains("admin")).FirstOrDefault();
            if(old != null)
            {
                sopace.users.Remove(old);
                updateAdmin.password = encryptor.Encrypt(updateAdmin.password);
                sopace.users.Add(updateAdmin);
                sopace.SaveChanges();
            }
            
            return Json("success",JsonRequestBehavior.AllowGet);
        }

        [ActionName("Delete_Admin"),HttpGet]
        [Route("Delete_Admin/{id_user}")]
        public ActionResult DeleteAdmin(string id_user)
        {
            if (id_user == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsertAdmin insertAdmin = new InsertAdmin();
            user usr = sopace.users.Where
                                  (u => u.username == id_user).FirstOrDefault();
            if (usr == null)
            {
                return HttpNotFound();
            }
            else
            {
                sopace.users.Remove(usr);
                sopace.SaveChanges();
            }
            return RedirectToAction("ViewAdmin");
        }

        [ActionName("CekNIP")]
        [Route("CekNIP")]
        public JsonResult CekNIP(string nip)
        {
            int cekCount = sopace.personal_information.Where(a => a.NIP == nip).Count();
            if (cekCount > 0)
            {
                string nama = sopace.personal_information.Where(a => a.NIP == nip).Select(a => a.nama_pegawai).FirstOrDefault();
                return Json(nama);
            }
            else
            {
                return Json(0);
            }
        }

        [ActionName("CekUsername")]
        [Route("CekUsername")]
        public JsonResult CekUsername(string usrnme)
        {
            int counter = sopace.users.Where(c => c.username == usrnme).Count();
            if (counter > 0)
            {
                string usr = "Username sudah ada";
                return Json(usr);
            }
            else
            {
                return Json(0);
            }
        }
    }
}