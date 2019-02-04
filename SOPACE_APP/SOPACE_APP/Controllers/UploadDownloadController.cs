using SOPACE_MVC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("UD")]
    public class UploadDownloadController : Controller
    {        
        private ACEEntities sopace = new ACEEntities();
        // GET: UploadDownload
        public ActionResult Index()
        {
            return View("view");
        }

        //[HttpPost]        
        //public ActionResult UploadPDF(HttpPostedFileBase file)
        //{
        //    String FileExt = Path.GetExtension(file.FileName).ToUpper();
        //    // Verify that the user selected a file
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        // Get file info
        //        var fileName = Path.GetFileName(file.FileName);
        //        var contentLength = file.ContentLength;
        //        var contentType = file.ContentType;

        //        Stream str = file.InputStream;
        //        BinaryReader Br = new BinaryReader(str);
        //        Byte[] FileDet = Br.ReadBytes((Int32)str.Length);

        //        // Get file data
        //        byte[] dataDok = new byte[] { };
        //        using (var binaryReader = new BinaryReader(file.InputStream))
        //        {
        //            dataDok = binaryReader.ReadBytes(file.ContentLength);
        //        }

        //        int countId = sopace.tb_dok.Count();

        //        // Save to database
        //        tb_dok dok = new tb_dok()
        //        {
        //            nmfile = fileName,
        //            id = countId,
        //            data = FileDet
        //        };
        //        sopace.tb_dok.Add(dok);
        //        sopace.SaveChanges();


        //        //Document doc = new Document()
        //        //{
        //        //    FileName = fileName,
        //        //    Data = data,
        //        //    ContentType = contentType,
        //        //    ContentLength = contentLength,
        //        //};
        //        //dataLayer.SaveDocument(doc);

        //        // Show success ...
        //        Response.Write("Success");
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        // Show error ...
        //        Response.Write("Fail");
        //        return RedirectToAction("Index");
        //    }
        //}

        //[HttpGet]
        //public FileResult DownloadPDF(int id)
        //{

        //    Response.Write(id);
        //    List<tb_dok> ObjFiles = sopace.tb_dok.Select(r => r).ToList();

        //    var FileById = (from FC in sopace.tb_dok
        //                    where FC.id == id
        //                    select new { FC.nmfile, FC.data }).ToList().FirstOrDefault();

        //    return File(FileById.data, "application/doc", FileById.nmfile);

        //}

        //[HttpGet]
        //public ActionResult View1()
        //{
        //    List<tb_dok> ObjFiles = sopace.tb_dok.Select(r => r).ToList();
        //    return View(ObjFiles);
        //}

        //[HttpPost]
        ////[Route("UploadToDirectory")]
        //public ActionResult UploadToDirectory(HttpPostedFileBase file)
        //{
        //    int countId = sopace.tb_dok.Count();
        //    if (file != null && file.ContentLength > 0)
        //    {
        //        // extract only the filename
        //        var fileName = Path.GetFileName(file.FileName);
        //        // store the file inside ~/App_Data/uploads folder
        //        var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
        //        file.SaveAs(path);
        //        tb_dok dok = new tb_dok()
        //        {
        //            nmfile = fileName,
        //            id = countId
        //        };
        //        sopace.tb_dok.Add(dok);
        //        sopace.SaveChanges();
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpGet]
        //public ActionResult DeleteFromDir()
        //{
        //    //HttpContext.Current.Server.MapPath("~/Employee/uploads/" + fileName)
        //    return RedirectToAction("Index");
        //}
            

}
}