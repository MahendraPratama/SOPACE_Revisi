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
    public class Insert
    {
    }

    public class InsertAdmin
    {
        public user User { get; set; }
    }
}