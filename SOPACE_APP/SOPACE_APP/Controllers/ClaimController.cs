using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    public class ClaimController : Controller
    {
        private ACEEntities sopace = new ACEEntities();
        private string nip;
        public void cekSession()
        {
            try
            {
                nip = Session["user_nip"].ToString();
            }
            catch (NullReferenceException)
            {
                Redirect("/home");
            }
        }
        // GET: Claim
        public ActionResult AddClaimMedical()
        {

            return View("InputClaimMedical");
        }

        [ActionName("ViewClaimMedical")]
        public ActionResult ViewClaimMedical()
        {

            return View("ListClaimMedical");
        }

        [HttpPost]
        public JsonResult alasanReject(string idklaim_medis, string alasan_reject)
        {
            var rejectKlaim = sopace.klaim_medis.Where(e => e.id_klaim_medis
                                                        == idklaim_medis).FirstOrDefault();
            rejectKlaim.alasan_reject = alasan_reject;
            sopace.Entry(rejectKlaim).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Rejected success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet, ActionName("updateClaim"), Route("updateClaim")]
        public JsonResult updateClaim(string idClaim, string status)
        {
            var updClaim = sopace.klaim_medis.Where(r => r.id_klaim_medis.Contains(idClaim)).FirstOrDefault();
            if (updClaim != null)
            {
                updClaim.status = status;
                sopace.Entry(updClaim).State = EntityState.Modified;
                sopace.SaveChanges();
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [Route("detailClaim/{idklaim?}"), ActionName("detailClaim"), AcceptVerbs("POST", "GET")]
        public JsonResult detailClaim(string idklaim)
        {
            var detail = sopace.klaim_medis.ToList().Where(kl => kl.id_klaim_medis.Equals(idklaim))
                .Select(kl => new
                {
                    kl.id_klaim_medis,
                    kl.id_tipe,
                    kl.NIP,
                    kl.nama_pasien,
                    kl.hubungan_pasien,
                    kl.nama_dokter,
                    kl.alamat_rs,
                    value = kl.tgl_perawatan.Value.ToString("dd/MM/yyyy"),
                    kl.no_tlp,
                    kl.status,
                    kl.alasan_reject,
                    kl.total_tagihan,
                    kl.payout
                });
            return Json(detail, JsonRequestBehavior.AllowGet);
        }
         
        [HttpGet]
        [Route("GetListClaim")]
        public JsonResult getDataListClaim(string status)
        {
            try
            {
                cekSession();
                var list_klaim = sopace.klaim_medis.ToList().Where(e => e.status == status).Select(e =>
                          new {
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
                          });
                return Json(list_klaim, JsonRequestBehavior.AllowGet);
            }
            catch (NullReferenceException)
            {
                return Json("");
            }


        }

        [HttpPost]
        [Route("AddClaim")]
        public JsonResult AddClaim(klaim_medis addClaim)
        {
            //===================== create id
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(
                                            s => s[random.Next(s.Length)]).ToArray());
            try
            {
                addClaim.id_klaim_medis = "CLMD-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                //==========================================
                addClaim.status = "Requested";
                addClaim.tgl_perawatan.ToString().Trim();

                sopace.klaim_medis.Add(addClaim);
                sopace.SaveChanges();
                return Json("Insert Claim Medical Success..", JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json("Nip Tidak Terdaftar", JsonRequestBehavior.AllowGet);
            }

        }
        //UpdateSaldoTunjangan(addClaim.NIP, addClaim.id_tipe, addClaim.total_tagihan);
        [HttpPost]
        [Route("GetNama")]
        public JsonResult GetNama(string id)
        {
            DateTime joinDate, dateNow;
            double date;
            string message = "";
            var cari = sopace.personal_information.Where(e => e.NIP == id).FirstOrDefault();
            if (cari != null)
            {
                joinDate = cari.tgl_masuk.Value;
                dateNow = DateTime.Now;
                date = dateNow.Subtract(joinDate).TotalDays;
                if (date < 90)
                {
                    message = "false";
                }
                else
                {
                    message = "true";
                }
                //string nama = sopace.personal_information.Where(e => e.NIP == id).Select(e => e.nama_pegawai).FirstOrDefault();
                return Json(message);
            }
            else
            {
                return Json(0);
            }
        }

        [ActionName("updatePlafond"), HttpGet]
        public ActionResult UpdateSaldoTunjangan(string idClaim)
        {
            string json = "";
            var klaimMedis = sopace.klaim_medis.Where(k => k.id_klaim_medis.Equals(idClaim)).First();
            decimal saldo = 0, saldoBaru = 0, payout = 0;
            var entity = sopace.tunjangan_medical.Where(e => e.NIP.Equals(klaimMedis.NIP)).First();
            if (entity != null)
            {
                switch (klaimMedis.id_tipe)
                {
                    case "Kacamata":
                        saldo = entity.saldo_kacamata.Value;
                        break;
                    case "Rawat Inap":
                        saldo = entity.saldo_rawat_inap.Value;
                        break;
                    case "Rawat Jalan":
                        saldo = entity.saldo_rawat_jalan.Value;
                        break;
                }
                if ((klaimMedis.total_tagihan * 8 / 10) < saldo) // jika total tagihan yang masih bisa diclaim kurang dari saldo
                {
                    if (klaimMedis.id_tipe == "Kacamata")
                    {
                        payout = (klaimMedis.total_tagihan * 10 / 10);
                        saldoBaru = saldo - payout;
                    }
                    else
                    {
                        payout = (klaimMedis.total_tagihan * 8 / 10);
                        saldoBaru = saldo - payout; // maka saldo akan dikurangi dengan total tagihan
                    }
                }
                else // jika total tagihan yg bisa diclaim lebih dari saldo
                {
                    payout = saldo;
                    saldoBaru = 0; // maka saldo tidak berkurang
                }
                switch (klaimMedis.id_tipe)
                {
                    case "Kacamata":
                        entity.saldo_kacamata = saldoBaru;
                        break;
                    case "Rawat Inap":
                        entity.saldo_rawat_inap = saldoBaru;
                        break;
                    case "Rawat Jalan":
                        entity.saldo_rawat_jalan = saldoBaru;
                        break;
                }
                json = "Payout Sejumlah " + payout;
            }
            else
            {
                json = "Claim Failed";
            }

            klaimMedis.payout = payout;
            sopace.Entry(klaimMedis).State = EntityState.Modified;
            sopace.Entry(entity).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ActionName("EDITCLAIM")]
        [Route("EditClaim/{Id}")]
        public ActionResult EditClaimMedical(string Id)
        {
            klaim_medis insertClaim = new klaim_medis();
            klaim_medis claim = sopace.klaim_medis.Where(p =>
                    p.id_klaim_medis == Id).FirstOrDefault();
            insertClaim = claim;
            TempData["tgl_perawatan"] = claim.tgl_perawatan.Value.ToString("yyyy-MM-dd");
            return View("EditClaimMedical", insertClaim);
        }

        [HttpPost, Route("UpdateClaim"), ActionName("UpdateClaim")]
        public ActionResult UpdateClaimMedical(klaim_medis klaim)
        {
            klaim_medis Km = klaim;
            var entityUser = sopace.klaim_medis.Where(e
                    => e.id_klaim_medis == klaim.id_klaim_medis).FirstOrDefault();
            if (entityUser != null)
            {
                sopace.Entry(entityUser).CurrentValues.SetValues(Km);
            }
            sopace.SaveChanges();
            return RedirectToAction("ViewClaimMedical");
        }

        [ActionName("DELETECLAIM")]
        [Route("DeleteClaim/{Id}")]
        public ActionResult DeleteClaimMedical(string Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            klaim_medis Km = new klaim_medis();
            klaim_medis Klaim = sopace.klaim_medis.Where
                                  (p => p.id_klaim_medis == Id).FirstOrDefault();
            if (Klaim == null)
            {
                return HttpNotFound();
            }
            else
            {
                sopace.klaim_medis.Remove(Klaim);
                sopace.SaveChanges();
            }
            return RedirectToAction("ViewClaimMedical");
        }
    }
}