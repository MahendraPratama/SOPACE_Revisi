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
    
    public partial class klaim_medis
    {
        public string id_klaim_medis { get; set; }
        public string NIP { get; set; }
        public string nama_pasien { get; set; }
        public string hubungan_pasien { get; set; }
        public string nama_dokter { get; set; }
        public Nullable<System.DateTime> tgl_perawatan { get; set; }
        public string alamat_rs { get; set; }
        public string no_tlp { get; set; }
        public string diagnosa { get; set; }
        public decimal total_tagihan { get; set; }
        public string status { get; set; }
        public string id_tipe { get; set; }
        public Nullable<decimal> payout { get; set; }
    
        public virtual personal_information personal_information { get; set; }
    }
}
