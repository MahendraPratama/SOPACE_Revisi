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
    
    public partial class cuti
    {
        public string id_cuti { get; set; }
        public string NIP { get; set; }
        public Nullable<System.DateTime> tgl_mulai_cuti { get; set; }
        public Nullable<System.DateTime> tgl_selesai_cuti { get; set; }
        public string alasan_cuti { get; set; }
        public string no_tlp { get; set; }
        public string alamat { get; set; }
    
        public virtual personal_information personal_information { get; set; }
    }
}
