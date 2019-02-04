using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPACE_MVC.Models
{
    public class PersonUser
    {
        public string NIP { get; set; }
        public string nama_pegawai { get; set; }
        public Nullable<System.DateTime> tgl_lahir { get; set; }
        public string alamat_identitas { get; set; }
        public string alamat_domisili { get; set; }
        public string NPWP { get; set; }
        public string no_ktp { get; set; }
        public string no_hp { get; set; }
        public string agama { get; set; }
        public string email { get; set; }
        public string pendidikan { get; set; }
        public string jurusan { get; set; }
        public Nullable<System.DateTime> tgl_masuk { get; set; }
        public string status_pajak { get; set; }
        public string jenis_kelamin { get; set; }        
        public string username { get; set; }
        public string password { get; set; }
        public string role { get; set; }
    }
}