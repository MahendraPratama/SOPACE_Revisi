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
            ViewBag.NamaPeg = new SelectList(sopace.personal_information.Where(e => e.status_aktif == "aktif"), "nama_pegawai", "nama_pegawai");
            return View("Cuti");
        }

        [HttpGet]
        [Route("CutiMassal")]
        public ActionResult CutiMassal()
        {
            return View();
        }

        [HttpGet]
        [Route("ListCuti")]
        public JsonResult ListCuti()
        {

            var list_cuti = sopace.cutis.ToList().Select(e => new
            {
                e.id_cuti,
                e.personal_information.nama_pegawai,
                value1 = e.tgl_mulai_cuti.Value.ToLongDateString(),
                value2 = e.tgl_selesai_cuti.Value.ToLongDateString(),
                e.alasan_cuti
            });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("FormListCuti")]
        public ActionResult FormListCuti()
        {
            return View("ListCutiKaryawan");
        }

        [HttpGet]
        [Route("CutiKaryawan")]
        public ActionResult CutiKaryawan()
        {
            return View();
        }

        [HttpPost, ActionName("GetNamaEmp"), Route("GetNamaEmp")]
        public JsonResult GetNama(string nama)
        {
            int cari = sopace.personal_information.Where(e => e.nama_pegawai == nama).Count();
            if (cari > 0)
            {
                string id = sopace.personal_information.Where(e => e.nama_pegawai == nama).Select(e => e.NIP).FirstOrDefault();
                return Json(id);
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
            var list_cuti = sopace.cutis.ToList().Where(e => e.NIP == nip).Select(e => new {
                e.id_cuti,
                value1 = e.tgl_mulai_cuti.Value.ToString("dd-MM-yyyy"),
                value2 = e.tgl_selesai_cuti.Value.ToString("dd-MM-yyyy"),
                e.alasan_cuti
            });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("getDetailcutikaryawan/{id}")]
        public JsonResult getDetailcutikaryawan(string id)
        {
            if (Session["user_role"].ToString() == "admin" || Session["user_role"].ToString() == "superadmin")
            {
                var list_cuti = sopace.cutis.ToList().Where(e => e.id_cuti == id).Select(e => new
                {
                    e.id_cuti,
                    e.NIP,
                    value1 = e.tgl_mulai_cuti.Value.ToString("dd-MM-yyyy"),
                    value2 = e.tgl_selesai_cuti.Value.ToString("dd-MM-yyyy"),
                    e.no_tlp,
                    e.alamat,
                    e.alasan_cuti
                });

                return Json(list_cuti, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string nip = Session["user_nip"].ToString();
                var list_cuti = sopace.cutis.ToList().Where(e => e.NIP == nip && e.id_cuti == id).Select(e => new {
                    e.id_cuti,
                    e.NIP,
                    value1 = e.tgl_mulai_cuti.Value.ToString("dd-MM-yyyy"),
                    value2 = e.tgl_selesai_cuti.Value.ToString("dd-MM-yyyy"),
                    e.no_tlp,
                    e.alamat,
                    e.alasan_cuti
                });

                return Json(list_cuti, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        [Route("GetKuotaCutiKaryawan")]
        public JsonResult getcutikaryawanID()
        {

            string nip = Session["user_nip"].ToString();
            var list_cuti = sopace.kuota_cuti.Where(e => e.NIP == nip).ToList().Select(e => new
            {
                e.sisa_cuti,
                e.cuti_baru,
                value2 = e.exp_sisa_cuti.Value.ToString("dd/MM/yyyy"),
                value1 = e.exp_cuti_baru.Value.ToString("dd/MM/yyyy")
            });
            return Json(list_cuti, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetDataCutiKaryawan")]
        public JsonResult GetDataCuti()
        {
            string nip = Session["user_nip"].ToString();
            var list_cuti = sopace.personal_information.ToList().Where(e=> e.status_aktif=="aktif").Join(sopace.kuota_cuti, r => r.NIP,
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
            else
            {

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

            return Json("Cuti Baru : " + kuota_cuti_baru + " Sisa Cuti Tahun Lalu : " + kuota_sisa_cuti);
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
        public JsonResult UpdateKuota(string txtNIP, DateTime txtTglMulai, DateTime txtTglSelesai, int txtNum)
        {
            sopace.pr_DiffKuotaCuti(txtNIP, txtTglMulai, txtTglSelesai, txtNum);
            return Json("Update Kuota Sukses");
        }

        [HttpPost]
        [Route("UpdateKuotaMassal")]
        public JsonResult UpdateKuotaMassal(string[] CutiId, DateTime txtTglMulai, DateTime txtTglSelesai, int txtNum)
        {
            foreach (string id in CutiId)
            {

                sopace.pr_DiffKuotaCuti(id, txtTglMulai, txtTglSelesai, txtNum);
            }
            return Json("Update Kuota Sukses");
        }

        [HttpPost]
        [Route("InsertCutiMassal")]
        public JsonResult InsertCutiMassal(string[] CutiId, DateTime txtTglMulai, DateTime txtTglSelesai, string txtAlasan)
        {
            foreach (string id in CutiId)
            {
                personal_information pInfo = sopace.personal_information.Where(e => e.NIP == id).FirstOrDefault();
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                Random random = new Random();
                string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
                cuti ct = new cuti();
                ct.id_cuti = "Cuti-" + DateTime.Now.Year + "-" + unique;
                ct.NIP = id;
                ct.no_tlp = pInfo.no_hp;
                ct.tgl_mulai_cuti = txtTglMulai;
                ct.tgl_selesai_cuti = txtTglSelesai;
                ct.alasan_cuti = txtAlasan;
                ct.alamat = pInfo.alamat_identitas;
                sopace.cutis.Add(ct);
                sopace.SaveChanges();
            }
            return Json("Update Success");
        }
    }
}