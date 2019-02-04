using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("Request")]
    public class RequestSuratController : Controller
    {
        private ACEEntities sopace = new ACEEntities();
        // GET: RequestSurat
        [Route("ManageReq")]
        public ActionResult manageReq()
        {
            getReq();
            return View("RequestSurat");
        }

        public void getReq()
        {
            int gaji = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("GAJI")).Count();
            int npwp = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("NPWP")).Count();
            int pbth = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("PBTH")).Count();
            int visa = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("VISA")).Count();
            int brek = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("BREK")).Count();
            int kkrj = sopace.requests.Where(r => r.status.Equals("Requested") && r.id_req.Contains("KKRJ")).Count();

            TempData["GAJI"] = gaji;
            TempData["NPWP"] = npwp;
            TempData["PBTH"] = pbth;
            TempData["VISA"] = visa;
            TempData["BREK"] = brek;
            TempData["KKRJ"] = kkrj;
            
        }

        [HttpGet,ActionName("getListReq"),Route("getListReq/{jenis?}")]
        public JsonResult getListReq(string status, string jenis)
        {
            var listReq = sopace.requests.ToList().Where(r=>r.status.Contains(status) && r.id_req.Contains(jenis))
                                                     .Select(r => new
                                                        {
                                                            r.id_req,
                                                            r.nama_pegawai,
                                                            tglReq = r.tanggal_request.Value.ToString("dd/MM/yyyy"),                                                           
                                                            r.status
                                                        });

            return Json(listReq,JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("updateReq"), Route("updateReq")]
        public JsonResult updateReq(string idReq, string status)
        {
            var updReq = sopace.requests.Where(r => r.id_req.Contains(idReq)).FirstOrDefault();
            if (updReq != null)
            {
                request UpdatedRequest = new request();
                UpdatedRequest.id_req = updReq.id_req;
                UpdatedRequest.NIP = updReq.NIP;
                UpdatedRequest.nama_pegawai = updReq.nama_pegawai;
                UpdatedRequest.tanggal_request = updReq.tanggal_request;
                UpdatedRequest.status = status;
                sopace.Entry(updReq).CurrentValues.SetValues(UpdatedRequest);
                sopace.SaveChanges();
            }
            return Json("");
        }

        [HttpPost, ActionName("DetReq"), Route("DetReq")]
        public JsonResult getDetailReq(string idReq)
        {
            var listReq = sopace.requests.ToList().Where(r => r.id_req.Contains(idReq))
                                                     .Select(r => new
                                                     {
                                                         r.id_req,
                                                         r.nama_pegawai,
                                                         tglReq = r.tanggal_request.Value.ToString("dd/MM/yyyy"),
                                                         r.status,
                                                         det1 = (idReq.Contains("BREK") ? r.buku_rekening.nama_bank :
                                                                    ((idReq.Contains("KKRJ")) ? r.keterangan_kerja.jabatan :
                                                                        ((idReq.Contains("NPWP")) ? r.NPWP.no_tlp :
                                                                            ((idReq.Contains("GAJI")) ? r.slip_gaji.durasi_awal + "-" + r.slip_gaji.durasi_akhir :
                                                                                ((idReq.Contains("VISA")) ? r.visa.no_passport :
                                                                                    r.pemberitahuan.tujuan
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                                 )
                                                                 ,
                                                         det2 = (idReq.Contains("BREK") ? r.buku_rekening.keperluan :
                                                                    ((idReq.Contains("KKRJ")) ? r.keterangan_kerja.alamat :
                                                                        ((idReq.Contains("NPWP")) ? r.NPWP.alamat_kantor_pajak :
                                                                            ((idReq.Contains("VISA")) ? r.visa.keperluan :
                                                                                ((idReq.Contains("PBTH")) ? r.pemberitahuan.deskripsi :
                                                                                "none"
                                                                            )
                                                                        )
                                                                    )
                                                                )
                                                                ),
                                                         textDet1 = (idReq.Contains("BREK") ? "Nama Bank" :
                                                                    ((idReq.Contains("KKRJ")) ? "Jabatan" :
                                                                        ((idReq.Contains("NPWP")) ? "No Telp" :
                                                                            ((idReq.Contains("GAJI")) ? "Durasi" :
                                                                                ((idReq.Contains("VISA")) ? "Nomor Passport" :
                                                                                    "Tujuan"
                                                                                )
                                                                            )
                                                                        )
                                                                    )
                                                                ),
                                                         textDet2 = (idReq.Contains("BREK") ? "Nama Bank" :
                                                                    ((idReq.Contains("KKRJ")) ? "Jabatan" :
                                                                        ((idReq.Contains("NPWP")) ? "No Telp" :
                                                                            ((idReq.Contains("VISA")) ? "Keperluan" :
                                                                                ((idReq.Contains("PBTH")) ? "Deskripsi" :
                                                                                "none"
                                                                            )
                                                                        )
                                                                    )
                                                                ))
                                                     });

            return Json(listReq, JsonRequestBehavior.AllowGet);
        }
    }
}