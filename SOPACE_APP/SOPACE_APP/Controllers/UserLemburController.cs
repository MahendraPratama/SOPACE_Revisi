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

        [Route("detailLembur/{idlembur?}"), ActionName("detailLembur"), AcceptVerbs("POST", "GET")]
        public JsonResult detailLembur(string id_lembur)
        {
            var detail = sopace.lemburs.ToList().Where(l => l.id_lembur.Equals(id_lembur))
                .Select(l => new
                {
                    l.id_lembur,
                    l.NIP,
                    value = l.tanggal.Value.ToString("dd/MM/yyyy"),
                    jammu = l.jam_mulai.Value.ToString(@"hh\:mm"),
                    jamse = l.jam_selesai.Value.ToString(@"hh\:mm"),
                    totjam = l.total_jam.Value.ToString(@"hh\:mm"),
                    l.alasan,
                    l.stats,
                    l.keterangan
                });
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
    }
}