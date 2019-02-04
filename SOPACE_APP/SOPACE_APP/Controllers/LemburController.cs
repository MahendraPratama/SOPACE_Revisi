using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("Lembur")]
    public class LemburController : Controller
    {
        static private ACEEntities sopace = new ACEEntities();
        // GET: Lembur
        [ActionName("GetIDLembur")]
        public JsonResult getId()
        {
            return Json(sopace.pr_getIDLembur(), JsonRequestBehavior.AllowGet);
        }

        [ActionName("Show"),Route("Show")]
        public ActionResult TampilkanDaftarLembur()
        {            
            return View("ShowLembur",sopace.lemburs.ToList());
        }

        [ActionName("Add"), Route("Add")]
        public ActionResult InputFormLembur()
        {
            return View("InputLembur");
        }
        
        public ActionResult Edit(string id)
        {
            lembur lbr = sopace.lemburs.Where(a => a.id_lembur == id).First();
            return View(lbr);
        }

        [HttpPost, ActionName("EditLembur")]
        [Route("EditLembur")]
        public ActionResult EditLbr(lembur lmbr)
        {
            lembur lbr = sopace.lemburs.Where(a => a.id_lembur == lmbr.id_lembur).FirstOrDefault();
            if (lbr != null)
            {
                sopace.Entry(lbr).CurrentValues.SetValues(lmbr);
                sopace.SaveChanges();
            }            
            return RedirectToAction("Show");            
        }

        [HttpGet]
        public ActionResult DeleteLembur(string id)
        {
            lembur lmbr = sopace.lemburs.Where(a => a.id_lembur == id).First();
            sopace.lemburs.Remove(lmbr);
            sopace.SaveChanges();            
            return RedirectToAction("Show");

        }

        [HttpPost]
        [Route("AddLembur")]
        public JsonResult InsertLembur(lembur Lmbr)
        {
            if (ModelState.IsValid)
            {
                sopace.lemburs.Add(Lmbr);
                sopace.SaveChanges();
            }
            return Json("Insert Data Success", JsonRequestBehavior.AllowGet);
        }

        [ActionName("GetNIP")]
        public JsonResult GetEmp(string nip)
        {
            int search = sopace.personal_information.Where(x => x.NIP == nip).Count();
            if (search > 0)
            {
                string nama = sopace.personal_information.Where(x => x.NIP == nip).Select(x => x.nama_pegawai).FirstOrDefault();               
                return Json(nama);
            }
            else
            {
                return Json(0);
            }
        }
    }
}