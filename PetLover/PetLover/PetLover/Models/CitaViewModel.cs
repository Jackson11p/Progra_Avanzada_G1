using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class CitaViewModel
    {
        public int CitaID { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; } 
        public int MascotaID { get; set; }
        public int VeterinarioID { get; set; }
        public string Descripcion { get; set; }
        public int Estado { get; set; }
    }
}