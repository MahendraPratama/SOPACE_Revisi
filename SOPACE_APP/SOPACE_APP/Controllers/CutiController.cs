using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;
using System.Data.Entity;   
namespace SOPACE_MVC.Controllers
{
    public class CutiController : Controller
    {
        private ACEEntities sopace = new ACEEntities();
        // GET: Cuti
        public ActionResult Index()
        {
                return View("Cuti");
        }

        [HttpGet]
        [Route("CutiMassal")]
        public ActionResult CutiMassal()
        {
            return View();
        }

        [HttpGet]
        [Route("CutiKaryawan")]
        public ActionResult CutiKaryawan()
        {
            return View();
        }

        [HttpPost,ActionName("GetNamaEmp"),Route("GetNamaEmp")]
        public JsonResult GetNama(string id)
        {
            int cari = sopace.personal_information.Where(e => e.NIP == id).Count();
            if (cari > 0)
            {
                string nama = sopace.personal_information.Where(e => e.NIP == id).Select(e => e.nama_pegawai).FirstOrDefault();
                return Json(nama);
            }
            else
            {
                return Json(0);
            }
        }

        [HttpGet]
        [Route("getcutikaryawan")]
        public JsonResult getcutikaryawan()
        {

            string nip = Session["user_nip"].ToString();
            var list_cuti = sopace.cutis.ToList().Where(e => e.NIP == nip).Join(sopace.personal_information, r => r.NIP,
                                                        bt => bt.NIP,
                                                        (r, bt) => new {
                                                            r.id_cuti,
                                                            r.NIP,
                                                            bt.nama_pegawai,
                                                            value1 = r.tgl_mulai_cuti.Value.ToString("dd-MM-yyyy"),
                                                            value2 = r.tgl_selesai_cuti.Value.ToString("dd-MM-yyyy"),
                                                            r.alasan_cuti,
                                                            r.no_tlp,
                                                            r.alamat
                                                        });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("getcutikaryawanID/{id}")]
        public JsonResult getcutikaryawanID(string id)
        {
            string nip = Session["user_nip"].ToString();
            var list_cuti = sopace.cutis.ToList().Where(e => e.NIP == nip && e.id_cuti == id).Join(sopace.personal_information, r => r.NIP,
                                                        bt => bt.NIP,
                                                        (r, bt) => new {
                                                            r.id_cuti,
                                                            r.NIP,
                                                            bt.nama_pegawai,
                                                            value1 = r.tgl_mulai_cuti.Value.ToString("dd-MM-yyyy"),
                                                            value2 = r.tgl_selesai_cuti.Value.ToString("dd-MM-yyyy"),
                                                            r.alasan_cuti,
                                                            r.no_tlp,
                                                            r.alamat
                                                        });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetDataCutiKaryawan")]
        public JsonResult GetDataCuti()
        {
            string nip = Session["user_nip"].ToString();
            var list_cuti = sopace.personal_information.ToList().Join(sopace.kuota_cuti, r => r.NIP,
                                                        bt => bt.NIP,
                                                        (r, bt) => new
                                                        {
                                                            r.NIP,
                                                            r.nama_pegawai,
                                                            bt.sisa_cuti,
                                                            bt.cuti_baru,
                                                            value2 = bt.exp_sisa_cuti.Value.ToString("dd/MM/yyyy"),
                                                            value1 = bt.exp_cuti_baru.Value.ToString("dd/MM/yyyy")
                                                        });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetJoinDate")]
        public JsonResult GetJoinDate(string id)
        {
            var join_date = sopace.personal_information.Where(e => e.NIP == id).Select(e => e.tgl_masuk).FirstOrDefault();
            if (join_date == null)
            {
                return Json("Data tidak ditemukan");
            }
            else {

                DateTime date1 = DateTime.Now;
                DateTime date2 = join_date.Value;
                double span = (date1 - date2).TotalDays;
                return Json(span);
            }
        }

        [HttpPost]
        [Route("GetCutiBaru")]
        public JsonResult GetCuti(string id)
        {
            var kuota_cuti_baru = sopace.kuota_cuti.Where(e => e.NIP == id).Select(e => e.cuti_baru).FirstOrDefault();
            var kuota_sisa_cuti = sopace.kuota_cuti.Where(e => e.NIP == id).Select(e => e.sisa_cuti).FirstOrDefault();

            return Json("Cuti Baru : "+ kuota_cuti_baru +" Sisa Cuti Tahun Lalu : " +kuota_sisa_cuti);
        }

        [HttpPost]
        [Route("AddCuti")]
        public JsonResult AddCuti(string txtNIP, string txtNoTlp, DateTime txtTglMulai, DateTime txtTglSelesai, string txtAlamat, string txtAlasan)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            cuti ct = new cuti();
            ct.id_cuti = "Cuti-" + DateTime.Now.Year + "-" + unique;
            ct.NIP = txtNIP;
            ct.no_tlp = txtNoTlp;
            ct.tgl_mulai_cuti = txtTglMulai;
            ct.tgl_selesai_cuti = txtTglSelesai;
            ct.alamat = txtAlamat;
            ct.alasan_cuti = txtAlasan;
            sopace.cutis.Add(ct);
            sopace.SaveChanges();
            return Json("Insert Cuti Success");
        }

        [HttpPost]
        [Route("UpdateKuota")]
        public JsonResult UpdateKuota(string txtNIP, DateTime txtTglMulai, DateTime txtTglSelesai)
        {
            kuota_cuti kt = sopace.kuota_cuti.Where(e => e.NIP == txtNIP).FirstOrDefault();
            var kuota_cuti_baru = sopace.kuota_cuti.Where(e => e.NIP == txtNIP).Select(e => e.cuti_baru).FirstOrDefault();
            var kuota_sisa_cuti = sopace.kuota_cuti.Where(e => e.NIP == txtNIP).Select(e => e.sisa_cuti).FirstOrDefault();
            var selisih = (txtTglSelesai.Day - txtTglMulai.Day) + 1;
            if (kuota_sisa_cuti > 0)
            {
                kuota_sisa_cuti = kuota_sisa_cuti - selisih;
                if (kuota_sisa_cuti < 0)
                {
                    kuota_cuti_baru = kuota_cuti_baru + kuota_sisa_cuti;
                    kuota_sisa_cuti = 0;
                    kt.sisa_cuti = kuota_sisa_cuti;
                    kt.cuti_baru = kuota_cuti_baru;
                    sopace.Entry(kt).State = EntityState.Modified;
                    sopace.SaveChanges();
                }

            }
            else {
                kuota_cuti_baru = kuota_cuti_baru - selisih;
                kt.cuti_baru = kuota_cuti_baru;
                sopace.Entry(kt).State = EntityState.Modified;
                sopace.SaveChanges();
            }

            return Json("Update Kuota Success");
        }

        [HttpPost]
        [Route("UpdateKuotaCutiMassal")]
        public JsonResult UpdateKuotaCutiMassal(string[] CutiId, DateTime txtTglMulai, DateTime txtTglSelesai)
        {
            var selisih = (txtTglSelesai.Day - txtTglMulai.Day) + 1;
            foreach (string id in CutiId)
            {
                kuota_cuti kt = sopace.kuota_cuti.Where(e => e.NIP == id).FirstOrDefault();
                var kuota_cuti_baru = sopace.kuota_cuti.Where(e => e.NIP == id).Select(e => e.cuti_baru).FirstOrDefault();
                var kuota_sisa_cuti = sopace.kuota_cuti.Where(e => e.NIP == id).Select(e => e.sisa_cuti).FirstOrDefault();
                if (kuota_sisa_cuti > 0)
                {
                    kuota_sisa_cuti = kuota_sisa_cuti - selisih;
                    if (kuota_sisa_cuti < 0)
                    {
                        kuota_cuti_baru = kuota_cuti_baru + kuota_sisa_cuti;
                        kuota_sisa_cuti = 0;
                        kt.sisa_cuti = kuota_sisa_cuti;
                        kt.cuti_baru = kuota_cuti_baru;
                        sopace.Entry(kt).State = EntityState.Modified;
                        sopace.SaveChanges();
                    }
                    else if(kuota_sisa_cuti > 0)
                    {
                        kt.sisa_cuti = kuota_sisa_cuti;
                        sopace.Entry(kt).State = EntityState.Modified;
                        sopace.SaveChanges();
                    }

                }
                else
                {
                    kuota_cuti_baru = kuota_cuti_baru - selisih;
                    kt.cuti_baru = kuota_cuti_baru;
                    sopace.Entry(kt).State = EntityState.Modified;
                    sopace.SaveChanges();
                }

            }


            return Json("Update Success");
        }
    }
}