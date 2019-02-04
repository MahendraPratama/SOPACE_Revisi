using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("UserClaim")]
    public class ClaimUserController : Controller
    {
        static private ACEEntities sopace = new ACEEntities();
        // GET: ClaimUser
        [HttpGet]
        [Route("GetCL")]
        [ActionName("ShowClaim")]
        public JsonResult TampilkanClaim()
        {
            string nips = Session["user_nip"].ToString();
            var clm = sopace.klaim_medis.ToList()
                                .Where(e => e.NIP == nips).Select(
                                    e => new {
                                        e.id_klaim_medis,
                                        e.nama_pasien,
                                        e.hubungan_pasien,
                                        e.nama_dokter,
                                        value = e.tgl_perawatan.Value.ToString("dd/MM/yyyy"),
                                        e.alamat_rs,
                                        e.no_tlp,
                                        e.diagnosa,
                                        e.total_tagihan,
                                        e.status,
                                        e.id_tipe
                                    }
                                );
            return Json(clm,JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("ViewMedis")]
        public ActionResult ViewMedis()
        {
            return View("ShowClaimUser");
        }

        [HttpGet]
        [Route("GetClaim/{id}")]
        public JsonResult GetClaim(string id)
        {
            string nips = Session["user_nip"].ToString();
            var list_claim = sopace.klaim_medis.ToList()
                                .Where(e => e.NIP == nips).Select(
                                    e => new {
                                        e.id_klaim_medis,
                                        e.nama_pasien,
                                        e.hubungan_pasien,
                                        e.nama_dokter,
                                        value = e.tgl_perawatan.Value.ToString("dd/MM/yyyy"),
                                        e.alamat_rs,
                                        e.no_tlp,
                                        e.diagnosa,
                                        e.total_tagihan,
                                        e.status,
                                        e.id_tipe                                        
                                    }
                                );            

            return Json(list_claim, JsonRequestBehavior.AllowGet);
        }
    }
}