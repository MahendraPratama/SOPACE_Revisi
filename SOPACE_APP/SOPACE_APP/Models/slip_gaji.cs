//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SOPACE_MVC.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class slip_gaji
    {
        public string id_req { get; set; }
        public string durasi_awal { get; set; }
        public string durasi_akhir { get; set; }
    
        public virtual request request { get; set; }
    }
}
