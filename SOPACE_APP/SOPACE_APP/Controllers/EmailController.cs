using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using SOPACE_MVC.Models;

namespace SOPACE_MVC.Controllers
{
    [RoutePrefix("Email")]
    public class EmailController : Controller
    {
        private ACEEntities sopace = new ACEEntities();
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost,ActionName("recover")]
        public JsonResult SendRecovery(string email)
        {            
            var recoverInfo = sopace.personal_information.Where(pi => pi.email == email)
                    .Join(sopace.users, pi => pi.NIP, us => us.NIP,
                        (pi, us) => new
                        {
                            us.username,
                            us.password
                        }
                    ).ToList();
            MailMessage mm = new MailMessage(/*alamat email*/"systemsopace@gmail.com", email);
            mm.Subject = "SOPACE Account Recovery";
            if(recoverInfo.Count() > 0)
            {
                mm.Body = string.Format("Your Username Is : {0} \nYour Password Is : {1}"
                    ,recoverInfo[0].username
                    ,recoverInfo[0].password);
            }
            else
            {
                mm.Body = "Your Email is not registered to our system. Please contact our admin";
            }            
            mm.IsBodyHtml = false;
            try
            {
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;

                NetworkCredential nc = new NetworkCredential(/*alamat email*/"systemsopace@gmail.com", /*password email*/"sopacesystem");
                //smtp.UseDefaultCredentials = false;
                smtp.Credentials = nc;
                smtp.Send(mm);
            }
            catch (SmtpException)
            {
                return Json("Connection Failed", JsonRequestBehavior.AllowGet);
            }
            
            
            return Json("Please Check Your Email", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SendEmail(EmailModel model)
        {
            MailMessage mm = new MailMessage(/*alamat email*/"sopacesystem@gmail.com", model.To);
            mm.Subject = model.Subject;
            mm.Body = model.Message;
            mm.IsBodyHtml = false;
            if (model.Attachment.ContentLength > 0)
            {
                string filename = Path.GetFileName(model.Attachment.FileName);
                mm.Attachments.Add(new Attachment(model.Attachment.InputStream, filename));
            }

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = true;

            NetworkCredential nc = new NetworkCredential(/*alamat email*/"sopacesystem@gmail.com", /*password email*/"sopacesystem");
            smtp.UseDefaultCredentials = true;
            smtp.Credentials = nc;
            smtp.Send(mm);
            ViewBag.Message = "Mail Has Been Sent Successfully";
            return View("Index");
        }
    }
}