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
    
    public partial class kuota_cuti
    {
        public string NIP { get; set; }
        public Nullable<int> sisa_cuti { get; set; }
        public Nullable<System.DateTime> exp_sisa_cuti { get; set; }
        public Nullable<int> cuti_baru { get; set; }
        public Nullable<System.DateTime> exp_cuti_baru { get; set; }
        public string status { get; set; }
    
        public virtual personal_information personal_information { get; set; }
    }
}
