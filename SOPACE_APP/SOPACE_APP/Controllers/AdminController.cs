using Newtonsoft.Json;
using SOPACE_MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("Admin")]
    public class AdminController : Controller
    {
        static private ACEEntities sopace = new ACEEntities();
        // GET: Admin        
        public ActionResult Index()
        {            
            if (Session["user_role"] == null)
            {
                return Redirect("/Home");
            }
            else if (Session["user_role"].ToString().StartsWith("us"))
            {
                return Redirect("/User");
            }
            else
            {
                getTempData();
                return View("DashboardAdmin");
            }

        }

        public void getTempData()
        {
            string nip = Session["user_nip"].ToString();
            TempData["nip"] = nip;
            var firstname = sopace.personal_information.Where(f => f.NIP == nip).FirstOrDefault();
            TempData["FirstName"] = firstname.nama_pegawai;

            int empint = sopace.personal_information.Where(it => it.penempatan.Contains("ACE")).Count();
            int empeks = sopace.personal_information.Where(ek => ek.NIP.Contains("ACE") && ek.penempatan != "ACE").Count();
            TempData["EmpInt"] = empint;
            TempData["EmpEks"] = empeks;

            int requested = sopace.requests.Where(r => r.status.Equals("Requested")).Count();
            int processed = sopace.requests.Where(r => r.status.Equals("Processed")).Count();
            TempData["requested"] = requested;
            TempData["processed"] = processed;

            var usrnm = sopace.users.Where(e => e.NIP == nip && e.role.Contains("admin")).FirstOrDefault().username;
            Session["username"] = usrnm;
            TempData.Keep();
        }

        [HttpPost]
        public ActionResult Getdata(string month)
        {
            int SG   = counter("GAJI",month);
            int KKRJ = counter("KKRJ", month);
            int PBTH = counter("PBTH", month); 
            int BREK = counter("BREK", month);
            int VISA = counter("VISA", month);

            int counter(string kodeReq, string selectMonth){
               return sopace.requests.Where(e => e.id_req.Contains(kodeReq) && 
                        e.tanggal_request.Value.Month.ToString().Contains(selectMonth) &&
                        e.tanggal_request.Value.Year.Equals(DateTime.Now.Year)).Count();
            }

            Ratio obj = new Ratio();
            obj.SG = SG;
            obj.KKRJ = KKRJ;
            obj.PBTH = PBTH;
            obj.BREK = BREK;
            obj.VISA = VISA;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddPersonalInfo()
        {
            return RedirectToAction("AddPInfo","PersonInfo");//View("InputDataKaryawan");
        }

        [ActionName("ViewAll")]
        public ActionResult ViewAll()
        {
            //var list_pInfo = sopace.personal_information.ToList();
            return RedirectToAction("ViewAll", "PersonInfo");//return View("ViewAllPInfo", list_pInfo);
        }

        [ActionName("ShowLembur")]
        public ActionResult ShowLembur()
        {
            
            return Redirect("/Lembur/Show");
        }

        public ActionResult AddLembur()
        {
            //var nmr = sopace.lemburs.Where(a => a.NIP.Equals(nip)).FirstOrDefault();
            return Redirect("/Lembur/Add");
        }

        [Route("CreateSuperAdmin")]
        public ActionResult CreateSuperAdmin()
        {
            Encryptor encryptor = new Encryptor();
            sopace.pr_createSU(encryptor.Encrypt("superadmin"));
            return Redirect("/Home");
        }

    }
}