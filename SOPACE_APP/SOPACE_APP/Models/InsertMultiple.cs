using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;

namespace SOPACE_MVC.Models
{
    public class InsertMultiple
    {
    }
    public class InsertPersonUser
    {
        public personal_information Personal_Information { get; set; }
        public user User { get; set; }
        //[Required, Microsoft.Web.Mvc.FileExtensions(Extensions = "csv",
        //     ErrorMessage = "Specify a CSV file. (Comma-separated values)")]
        public file_dokumen File_Dokumen { get; set; }
    }
}