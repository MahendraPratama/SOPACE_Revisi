using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;
using System.Data.Entity;
using System.IO;
using System.Diagnostics;
using System.Net;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("PersonInfo")]
    public class Personal_InformationController : Controller
    {
        static private Dictionary<string, HttpPostedFileBase> TempFileDok = new Dictionary<string, HttpPostedFileBase>();
        private ACEEntities sopace = new ACEEntities();
        Encryptor encryptor = new Encryptor();
        // GET: Personal_Information
        [ActionName("AddPInfo"),Route("AddPinfo")]
        public ActionResult ViewAddPInfo()
        {
            TempData["MasterLayout"] = "~/Views/MasterLayout/MasterAdmin.cshtml";
            return View("FormPInfo");
        }

        [HttpGet,Route("ViewAll"),ActionName("ViewAll")]
        public ActionResult ViewListAllPersonalInformation()
        {
            int empint = sopace.personal_information.Where
                        (it => it.penempatan.Contains("ACE") && it.status_aktif.Equals("aktif")).Count();
            int empeks = sopace.personal_information.Where
                        (ek => ek.NIP.Contains("ACE") && ek.penempatan != "ACE" && ek.status_aktif.Equals("aktif")).Count();
            TempData["EmpInt"] = empint;
            TempData["EmpEks"] = empeks;
            return View("ViewAllPInfo");
        }

        [HttpGet, Route("getDataPInfo"), ActionName("getDataPInfo")]
        public JsonResult getAllPInfo(string penempatan, string status)
        {
            var list_pInfo = sopace.personal_information.ToList().Where(li => li.NIP.Contains("ACE"))
                                    .Join(sopace.users, li=> li.NIP, us=>us.NIP,
                                    (li,us) => new {
                                        usrnm = us.username,
                                        li.penempatan,
                                        li.status_aktif,
                                        us.role,
                                        li.NIP,
                                        li.nama_pegawai,
                                        li.posisi,
                                        tglMasuk = li.tgl_masuk.Value.ToString("dd/MM/yyyy"),
                                        tglLahir = li.tgl_lahir.Value.ToString("dd/MM/yyyy")
                                    });
            var pinfo = list_pInfo.Where(li =>                                        
                                        (penempatan == "ACE" ? li.penempatan.Contains("ACE") : li.penempatan != "ACE") &&
                                        li.status_aktif.Equals(status) &&
                                        li.role == "user"
                                    ).ToList();
            return Json(pinfo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost,Route("UbahStatus")]
        public JsonResult ubahStatus(string nip_pegawai, string status_aktif, DateTime tanggal_resign, string alasan_resign)
        {
            var ubahStatus = sopace.personal_information.Where(e => e.NIP == nip_pegawai).FirstOrDefault();
            if (ubahStatus != null)
            {
                ubahStatus.status_aktif = status_aktif;
                ubahStatus.tgl_resign = tanggal_resign;
                ubahStatus.alasan_resign = alasan_resign;
                sopace.Entry(ubahStatus).State = EntityState.Modified;
                sopace.SaveChanges();
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(nip_pegawai + " Data not Found", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [Route("Add")]
        public JsonResult InsertPersonalInformation(InsertPersonUser insertPersonUser)
        {           
            file_dokumen fd = new file_dokumen();
            DateTime joinDate = insertPersonUser.Personal_Information.tgl_masuk.Value;
            string jsonResult = "", personNIP = insertPersonUser.Personal_Information.NIP;
            int cekCount = sopace.pr_cekNipAlready(personNIP).ElementAt(0).Value;
            if (cekCount > 0)
            {
                jsonResult = "NIP sudah ada dalam database";                
            }
            else
            {
                insertPersonUser.Personal_Information.status_aktif = "aktif";
                sopace.personal_information.Add(insertPersonUser.Personal_Information);
                insertPersonUser.User.password = encryptor.Encrypt(insertPersonUser.User.password);
                sopace.users.Add(insertPersonUser.User);
                sopace.file_dokumen.Add(insertPersonUser.File_Dokumen);
                sopace.SaveChanges();

                insertTableElse(personNIP, joinDate);
                jsonResult = "Insert Data Success";
            }            
            return Json(jsonResult, JsonRequestBehavior.AllowGet);
        }

        public void insertTableElse(string nip, DateTime tgl_masuk)
        {
            tunjangan_medical tunjangan = new tunjangan_medical();
            kuota_cuti kuota_Cuti = new kuota_cuti();
            //======================================= insert data to tunjangan_medical
            tunjangan.NIP = nip;
            tunjangan.saldo_kacamata = 300000;
            tunjangan.saldo_rawat_inap = 3000000;
            tunjangan.saldo_rawat_jalan = 3000000;
            tunjangan.status = "update";
            int year = DateTime.Now.Year - tgl_masuk.Year;
            tunjangan.renewalDate = tgl_masuk.AddYears(year);
            //======================================= insert data to kuota_cuti
            kuota_Cuti.NIP = nip;
            kuota_Cuti.cuti_baru = 0;
            kuota_Cuti.exp_cuti_baru = tgl_masuk;
            kuota_Cuti.exp_sisa_cuti = tgl_masuk;
            kuota_Cuti.sisa_cuti = 0;
            kuota_Cuti.status = "update";

            sopace.kuota_cuti.Add(kuota_Cuti);
            sopace.tunjangan_medical.Add(tunjangan);
            sopace.SaveChanges();
        }

        [HttpPost]
        [Route("Upload")]
        public JsonResult UploadToDirectory()
        {
            HttpPostedFileBase file = Request.Files[0];
            string tipe = Request["tipeFile"];
            string nip = Request["nip"];
            string mapPath = "~/Content/uploads/" + nip;
            if (!Directory.Exists(Server.MapPath(mapPath)))
            {
                Directory.CreateDirectory(Server.MapPath(mapPath));
            }            
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var path = Path.Combine(Server.MapPath(mapPath), fileName);
                Debug.WriteLine(path);
                if (!TempFileDok.ContainsKey(tipe))
                {                    
                    TempFileDok.Add(tipe, file);
                    Debug.WriteLine("TempDok Add "+tipe+" = "+file.FileName);
                }
                else
                {
                    string pathDelete = Path.Combine(Server.MapPath(mapPath), TempFileDok[tipe].FileName);                    
                    Debug.WriteLine("pathdelete : "+pathDelete);
                    if (System.IO.File.Exists(pathDelete))
                    {
                        Debug.WriteLine("Path Exist");
                        System.IO.File.Delete(pathDelete);
                    }
                    TempFileDok[tipe] = file;
                    Debug.WriteLine("TempDok Update " + tipe + " = " + file.FileName);
                }
                file.SaveAs(path);
            }
            Debug.WriteLine("TempDok Add Count = " + TempFileDok.Count);            
            return Json("Success");
        }

        [Route("DownloadFile")]
        public ActionResult DonwloadFile(string fileName, string nip)
        {
            string mapPath = "~/Content/uploads/" + nip;            
            var path = Server.MapPath(mapPath);
           
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Server.MapPath(mapPath), fileName));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);            
        }

        [ActionName("EditPInfoFromUser"),Route("EditPInfoFromUser")]
        public ActionResult EditPInfoFromUser()
        {
            TempData["MasterLayout"] = "~/Views/MasterLayout/MasterUser.cshtml";
            string nip = Session["user_nip"].ToString();
            string username = sopace.users.Where(u => u.NIP == nip).Select(u => u.username).FirstOrDefault();            
            return View("FormPInfo", DataForEdit(username));
        }

        public InsertPersonUser DataForEdit(string username)
        {
            InsertPersonUser insertPersonUser = new InsertPersonUser();
            personal_information pInfo = sopace.users.Where(u => u.username == username)
                    .Join(sopace.personal_information, u=> u.NIP, p=>p.NIP, (u,p)=> p).FirstOrDefault();
            user user = sopace.users.Where(e => e.username == username).FirstOrDefault();
            file_dokumen fd = sopace.users.Where(u => u.username == username).Join(sopace.file_dokumen,
                                        u => u.NIP,
                                        fdo => fdo.NIP,
                                        (u, fdo) => fdo).FirstOrDefault();
            user.password = encryptor.Decrypt(user.password);
            insertPersonUser.User = user;
            insertPersonUser.Personal_Information = pInfo;
            TempData["tglLahir"] = pInfo.tgl_lahir.Value.ToString("yyyy-MM-dd");
            TempData["tglMasuk"] = pInfo.tgl_masuk.Value.ToString("yyyy-MM-dd");
            
            if (fd != null)
            {
                if (fd.foto_profil != null)
                {
                    TempData["fotoProfil"] = fd.foto_profil;
                }
                if (fd.foto_buku_rekening != null)
                {
                    TempData["fotoBkRek"] = fd.foto_buku_rekening.ToString();
                }
                if (fd.cv != null)
                {
                    TempData["CV"] = fd.cv.ToString();
                }
                TempData["nip"] = fd.NIP;
            }
            return insertPersonUser;
        }

        [ActionName("EDIT")]
        [Route("{username}/edit")]
        public ActionResult EditPersonInfo(string username)
        {
            TempData["MasterLayout"] = "~/Views/MasterLayout/MasterAdmin.cshtml";
            return View("FormPInfo", DataForEdit(username));
        }

        [HttpPost, Route("Update")]
        public ActionResult UpdatePersonalInformation(InsertPersonUser insertPersonUser)
        {
            var entity = sopace.personal_information.Where(e =>
                                e.NIP == insertPersonUser.Personal_Information.NIP).FirstOrDefault();
            if (entity != null)
            {
                personal_information pInfo = insertPersonUser.Personal_Information;
                user User = insertPersonUser.User;
                sopace.Entry(entity).CurrentValues.SetValues(pInfo);

                var entityUser = sopace.users.Where(e => e.NIP == insertPersonUser.User.NIP).FirstOrDefault();
                if (entityUser != null)
                {
                    file_dokumen fd = insertPersonUser.File_Dokumen;
                    sopace.users.Remove(entityUser);
                    User.password = encryptor.Encrypt(User.password);
                    sopace.users.Add(User);

                    var entityFileDok = sopace.file_dokumen.Where(e=>e.NIP == insertPersonUser.User.NIP).FirstOrDefault();
                    if(entityFileDok != null)
                    {
                        sopace.Entry(entityFileDok).CurrentValues.SetValues(fd);
                    }
                }

                sopace.SaveChanges();
            }            
            return RedirectToAction("ViewAll");
        }

        [ActionName("DELETE")]
        [Route("DELETE/{username}")]
        public ActionResult DeletePersonInfo(string username)
        {
            if (username == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsertPersonUser insertPersonUser = new InsertPersonUser();
            personal_information pInfo = sopace.users.Where
                                  (u => u.username == username).
                                  Join(sopace.personal_information, u=>u.NIP, p=>p.NIP, (u,p)=> p).FirstOrDefault();
            if (pInfo == null)
            {
                return HttpNotFound();
            }
            else
            {
                DeleteDirectory(pInfo.NIP);
                sopace.personal_information.Remove(pInfo);                
                sopace.SaveChanges();
            }
            return RedirectToAction("ViewAll");
        }

        public void DeleteDirectory(string folderNIP)
        {
            string mapPath = "~/Content/uploads/" + folderNIP;
            if (Directory.Exists(Server.MapPath(mapPath)))
            {
                DirectoryInfo directory = new DirectoryInfo(Server.MapPath(mapPath));
                foreach (FileInfo file in directory.GetFiles())
                {
                    file.Delete();
                }
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    dir.Delete(true);
                }
            }
        }

        [HttpGet]
        [Route("GenerateUsrnm/{name}")]
        public JsonResult generateUsernameByFullName(string name)
        {
            string[] letters = name.Split(null);
            string usrnm = letters[0];
            return Json(letters,JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        [ActionName("CekNIP")]
        [Route("CekNIP")]
        public JsonResult CekNIP()
        {
            string ret = sopace.pr_getLastNIP().ElementAt(0);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }
       
        [HttpGet,Route("CekUsername"), ActionName("CekUsername")]
        public JsonResult CekUsernameAlreadyUsed(string username, string nip)
        {
            string ret="";
            int counter = sopace.users.Where(c => c.username == username).Count();
            int nipAda = sopace.users.Where(n => n.NIP == nip).Count();
            if(nipAda > 0)
            {
                if(sopace.users.Where(n => n.NIP == nip && n.username == username).Count() > 0)
                {
                    ret = "true";
                }
                else if(counter > 0)
                {
                    ret = "false";
                }
                else
                {
                    ret = "true";
                }
            }
            else
            {
                
                if (counter > 0)
                {
                    ret = "false";
                }
                else
                {
                    ret = "true";
                }
            }            
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        [HttpGet, Route("GetInfo/{username}"), ActionName("GetInfo")]
        public JsonResult getInfo(string username)
        {            
            var list_Info = sopace.users.ToList().Where(li => li.username == username
                                ).Join(sopace.personal_information, li=>li.NIP, pi=>pi.NIP,
                                    (li,pi) => new {
                                        usrnm = li.username,
                                        pi.NIP,
                                        pi.nama_pegawai,
                                        pi.posisi,
                                        tglMasuk = pi.tgl_masuk.Value.ToString("dd/MM/yyyy"),
                                        pi.no_ktp,
                                        pi.alamat_identitas,
                                        pi.no_hp,
                                        pi.email,
                                        pi.no_rek
                                    });
            return Json(list_Info, JsonRequestBehavior.AllowGet);
        }
    }
}
