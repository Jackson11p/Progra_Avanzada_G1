using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class HistorialMedicoViewModel
    {
        public int HistorialID { get; set; }
        public int CitaID { get; set; }
        public string NombreMascota { get; set; }
        public string NombreVeterinario { get; set; }
        public DateTime FechaCita { get; set; }
        public string Diagnostico { get; set; }
        public decimal MontoTotal { get; set; }
    }

}