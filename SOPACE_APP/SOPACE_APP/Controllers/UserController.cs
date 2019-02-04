using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;
using System.Data.Entity;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("user")]
    public class UserController : Controller
    {
        // GET: User
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

        public ActionResult Index()
        {
            cekSession();
            if (Session["user_role"] == null)
            {
                return Redirect("/Home");
            }
            else if (Session["user_role"].ToString().Contains("admin"))
            {
                return Redirect("/Admin");
            }
            else
            {
                var ret = sopace.file_dokumen.Where(p => p.NIP == nip).FirstOrDefault();
                var nm = sopace.personal_information.Where(p => p.NIP == nip).FirstOrDefault();
                if(ret.foto_profil != null)
                {
                    TempData["dir_profil"] = "uploads/"+nip+"/"+ret.foto_profil;
                }
                else
                {
                    TempData["dir_profil"] = "/defaultImage/placeholder.png";
                }
                TempData["Name"] = nm.nama_pegawai;
                return View();
            }

        }

        [HttpGet]
        [Route("ViewNPWP")]
        public ActionResult ViewNPWP()
        {
            return View("View_NPWP");
        }

        [HttpGet]
        [Route("ViewPBTH")]
        public ActionResult ViewPBTH()
        {
            return View("View_PBTH");
        }

        [HttpGet]
        [Route("ViewKKRJ")]
        public ActionResult ViewKKRJ()
        {
            return View("View_KKRJ");
        }

        [HttpGet]
        [Route("ViewVisa")]
        public ActionResult ViewVisa()
        {
            return View("View_Visa");
        }

        [HttpGet]
        [Route("ViewSG")]
        public ActionResult ViewSG()
        {
            return View("View_SG");
        }

        [HttpGet]
        [Route("ViewBR")]
        public ActionResult ViewBR()
        {
            return View("View_BR");
        }

        [HttpGet]
        [Route("GetVisa")]
        public JsonResult ViewRequestVisa()
        {
            string nip = Session["user_nip"].ToString();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.visas, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.no_passport,
                                                            bt.keperluan,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetSG")]
        public JsonResult ViewRequestSG()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.slip_gaji, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.durasi_awal,
                                                            bt.durasi_akhir,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetKKRJ")]
        public JsonResult ViewRequestKKRJ()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.keterangan_kerja, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.jabatan,
                                                            bt.alamat,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetPBTH")]
        public JsonResult ViewRequestPBTH()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.pemberitahuans, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.tujuan,
                                                            bt.deskripsi,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetNPWP")]
        public JsonResult ViewRequestNPWP()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.NPWPs, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.no_tlp,
                                                            bt.alamat_kantor_pajak,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetNPWPCld")]
        public JsonResult ViewRequestNPWPCld()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip && e.status == "completed").OrderByDescending(e => e.tanggal_request).Join(sopace.NPWPs, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.no_tlp,
                                                            bt.alamat_kantor_pajak,
                                                            r.status
                                                        });
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("GetBR")]
        public JsonResult ViewRequestBR()
        {
            cekSession();
            var list_pInfo = sopace.requests.ToList().Where(e => e.NIP == nip).OrderByDescending(e => e.tanggal_request).Join(sopace.buku_rekening, r => r.id_req,
                                                        bt => bt.id_req,
                                                        (r, bt) => new
                                                        {
                                                            r.id_req,
                                                            value = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                            bt.nama_bank,
                                                            bt.keperluan,
                                                            r.status
                                                        });

            //var dt = sopace.requests.ToList().Select(p => p.tanggal_request.ToShortDateString());
            return Json(list_pInfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetNama")]
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
        //==================================================================================================================================

        [HttpGet]
        [Route("FormBR")]
        public ActionResult FormRequest_BK()
        {
            return View("Request_BR");
        }

        [HttpGet]
        [Route("FormSG")]
        public ActionResult FormRequest_SG()
        {
            return View("Request_SG");
        }

        [HttpGet]
        [Route("FormKKRJ")]
        public ActionResult FormRequest_KKRJ()
        {
            return View("Request_KKRJ");
        }

        [HttpGet]
        [Route("FormNPWP")]
        public ActionResult FormRequest_NPWP()
        {
            return View("Request_NPWP");
        }

        [HttpPost]
        [Route("AddPBTH")]
        public JsonResult AddReqPBTH(DateTime txtTgl, string txtTujuan, string txtDesc)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "PBTH-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                pemberitahuan pb = new pemberitahuan();
                pb.id_req = req.id_req;
                pb.tujuan = txtTujuan;
                pb.deskripsi = txtDesc;
                sopace.pemberitahuans.Add(pb);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("AddVisa")]
        public JsonResult AddReqVisa(DateTime txtTgl, string txtnopass, string txtKeperluan)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "VISA-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                visa vs = new visa();
                vs.id_req = req.id_req;
                vs.no_passport = txtnopass;
                vs.keperluan = txtKeperluan;
                sopace.visas.Add(vs);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("AddSG")]
        public JsonResult AddReqSG(DateTime txtTgl, string Durasi1, string Durasi2)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "GAJI-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                slip_gaji sg = new slip_gaji();
                sg.id_req = req.id_req;
                sg.durasi_awal = Durasi1;
                sg.durasi_akhir = Durasi2;
                sopace.slip_gaji.Add(sg);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("AddBR")]
        public JsonResult AddReqBR(DateTime txtTgl, string NamaBank, string txtKeperluan)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "BREK-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                buku_rekening bk = new buku_rekening();
                bk.id_req = req.id_req;
                bk.keperluan = txtKeperluan;
                bk.nama_bank = NamaBank;
                sopace.buku_rekening.Add(bk);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("AddKKRJ")]
        public JsonResult AddReqKKRJ(DateTime txtTgl, string txtjabatan, string txtAlamat)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "KKRJ-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                keterangan_kerja kk = new keterangan_kerja();
                kk.id_req = req.id_req;
                kk.jabatan = txtjabatan;
                kk.alamat = txtAlamat;
                sopace.keterangan_kerja.Add(kk);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("AddNPWP")]
        public JsonResult AddReqnpwp(DateTime txtTgl, string txtNoTlp, string txtAlamat)
        {
            cekSession();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            Random random = new Random();
            string unique = new string(Enumerable.Repeat(chars, 4).Select(s => s[random.Next(s.Length)]).ToArray());
            string nama = sopace.personal_information.Where(e => e.NIP == nip).Select(e => e.nama_pegawai).FirstOrDefault();
            if (nama != null)
            {
                request req = new request();
                req.id_req = "NPWP-" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                req.NIP = nip;
                req.nama_pegawai = nama;
                req.tanggal_request = txtTgl;
                req.status = "requested";
                sopace.requests.Add(req);
                sopace.SaveChanges();

                NPWP npwp = new NPWP();
                npwp.id_req = req.id_req;
                npwp.no_tlp = txtNoTlp;
                npwp.alamat_kantor_pajak = txtAlamat;
                sopace.NPWPs.Add(npwp);
                sopace.SaveChanges();

                notifikasi ntf = new notifikasi();
                ntf.id_req = req.id_req;
                ntf.id_notifikasi = "NTF" + DateTime.Now.ToString("yyyy").ToString() + "-" + unique;
                ntf.id_penerima = "superuser";
                ntf.status = "Delivery";
                sopace.notifikasis.Add(ntf);
                sopace.SaveChanges();
                return Json("Insert Request Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Insert Request Failed", JsonRequestBehavior.AllowGet);
            }
        }
        //======================================================================================================================================================
        [HttpPost]
        [Route("Update")]
        public JsonResult EditBT(buku_rekening bk1, string NamaBank)
        {
            buku_rekening bk = sopace.buku_rekening.Where(e => e.id_req == bk1.id_req).First();
            bk.keperluan = bk1.keperluan;
            bk.nama_bank = bk1.nama_bank;
            sopace.Entry(bk).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateSG")]
        public JsonResult EditSG(slip_gaji sg1, string Durasi1, string Durasi2)
        {
            slip_gaji sg = sopace.slip_gaji.Where(e => e.id_req == sg1.id_req).First();
            sg.durasi_awal = sg1.durasi_awal;
            sg.durasi_akhir = sg1.durasi_akhir;
            sopace.Entry(sg).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdatePBTH")]
        public JsonResult EditPBTH(pemberitahuan pb1)
        {
            pemberitahuan pb = sopace.pemberitahuans.Where(e => e.id_req == pb1.id_req).First();
            pb.tujuan = pb1.tujuan;
            pb.deskripsi = pb1.deskripsi;
            sopace.Entry(pb).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateKKRJ")]
        public JsonResult EditKKRJ(keterangan_kerja kk1)
        {
            keterangan_kerja kk = sopace.keterangan_kerja.Where(e => e.id_req == kk1.id_req).First();
            kk.jabatan = kk1.jabatan;
            kk.alamat = kk1.alamat;
            sopace.Entry(kk).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateNPWP")]
        public JsonResult EditNPWP(NPWP npwp1)
        {
            NPWP npwp = sopace.NPWPs.Where(e => e.id_req == npwp1.id_req).First();
            npwp.no_tlp = npwp1.no_tlp;
            npwp.alamat_kantor_pajak = npwp1.alamat_kantor_pajak;
            sopace.Entry(npwp).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("UpdateVisa")]
        public JsonResult EditVisa(visa vs1)
        {
            visa vs = sopace.visas.Where(e => e.id_req == vs1.id_req).First();
            vs.no_passport = vs1.no_passport;
            vs.keperluan = vs1.keperluan;
            sopace.Entry(vs).State = EntityState.Modified;
            sopace.SaveChanges();
            return Json("Update Data Request Success", JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("FormEditBR/{id}")]
        public ActionResult FormEditBR(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {

                return Redirect("/User/ViewBR");
            }
            else
            {

                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.buku_rekening.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        [HttpGet]
        [Route("FormEditPBTH/{id}")]
        public ActionResult FormEditPBTH(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {

                return Redirect("/User/ViewPBTH");
            }
            else
            {

                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.pemberitahuans.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        [HttpGet]
        [Route("FormEditSG/{id}")]
        public ActionResult FormEditSG(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Redirect("/User/ViewSG");
            }
            else
            {
                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.slip_gaji.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        [HttpGet]
        [Route("FormEditVisa/{id}")]
        public ActionResult FormEditVisa(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Redirect("/User/ViewVisa");
            }
            else
            {
                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.visas.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        [HttpGet]
        [Route("FormEditKKRJ/{id}")]
        public ActionResult FormEditKKRJ(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Redirect("/User/ViewKKRJ");
            }
            else
            {
                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.keterangan_kerja.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        [HttpGet]
        [Route("FormEditNPWP/{id}")]
        public ActionResult FormEditNPWP(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Redirect("/User/ViewNPWP");
            }
            else
            {
                TempData["ID"] = id;
                TempData.Keep();
                var list_pInfo = sopace.NPWPs.Where(e => e.id_req == id).First();
                return View(list_pInfo);
            }
        }

        //===============================================================================================================================

        [HttpGet]
        [Route("Delete/{id}")]
        public JsonResult DeleteBT(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("DeleteKKRJ/{id}")]
        public JsonResult DeleteKKRJ(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("DeleteNPWP/{id}")]
        public JsonResult DeleteNPWP(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [Route("DeleteSG/{id}")]
        public JsonResult DeleteSG(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("DeleteVisa/{id}")]
        public JsonResult DeleteVisa(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [Route("DeletePBTH/{id}")]
        public JsonResult DeletePBTH(string id)
        {
            string status = sopace.requests.Where(e => e.id_req == id).Select(e => e.status).FirstOrDefault();
            if (status != "requested")
            {
                return Json("Delete failed", JsonRequestBehavior.AllowGet);
            }
            else
            {
                request req = sopace.requests.Where(e => e.id_req == id).First();
                sopace.requests.Remove(req);
                sopace.SaveChanges();
                return Json("Delete Request Success", JsonRequestBehavior.AllowGet);
            }
        }
    }
}