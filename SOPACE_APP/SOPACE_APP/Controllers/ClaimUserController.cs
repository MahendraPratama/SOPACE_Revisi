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
        private string nips;
        public void cekSession()
        {
            try
            {
                nips = Session["user_nip"].ToString();
            }
            catch (NullReferenceException)
            {
                Redirect("/home");
            }
        }
        // GET: ClaimUser

        [HttpGet]
        [Route("GetPlafond")]
        public JsonResult SisaSaldo()
        {
            cekSession();
            var sisa = sopace.tunjangan_medical.ToList().Where(e => e.NIP == nips).Select(e =>
            new {
                    e.NIP,
                    e.saldo_rawat_jalan,
                    e.saldo_rawat_inap,
                    e.saldo_kacamata,
                    value = e.renewalDate.Value.ToString("dd/MM/yyyy")
                }).FirstOrDefault();
            return Json(sisa, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetCL")]
        [ActionName("ShowClaim")]
        public JsonResult TampilkanClaim()
        {
            cekSession();
            var clm = sopace.klaim_medis.ToList()
                                .Where(e => e.id_klaim_medis == e.id_klaim_medis && e.NIP == nips).Select(
                                    e => new {
                                        e.id_klaim_medis,
                                        e.NIP,
                                        e.nama_pasien,
                                        e.hubungan_pasien,
                                        e.nama_dokter,
                                        value = e.tgl_perawatan.Value.ToString("dd/MM/yyyy"),
                                        e.alamat_rs,
                                        e.no_tlp,
                                        e.diagnosa,
                                        e.total_tagihan,
                                        e.status,
                                        e.id_tipe,
                                        e.payout
                                    }
                                );
            return Json(clm, JsonRequestBehavior.AllowGet);
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
            cekSession();
            var list_claim = sopace.klaim_medis.ToList()
                                .Where(e => e.NIP == nips && e.id_klaim_medis == id).Select(
                                    e => new {
                                        e.id_klaim_medis,
                                        e.NIP,
                                        e.nama_pasien,
                                        e.hubungan_pasien,
                                        e.nama_dokter,
                                        value = e.tgl_perawatan.Value.ToString("dd/MM/yyyy"),
                                        e.alamat_rs,
                                        e.no_tlp,
                                        e.diagnosa,
                                        e.total_tagihan,
                                        e.alasan_reject,
                                        e.status,
                                        e.id_tipe,
                                        e.payout
                                    }
                                );

            return Json(list_claim, JsonRequestBehavior.AllowGet);
        }
    }
}