using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class CitaTratamientoViewModel
    {
        public int CitaID { get; set; }
        public string Mascota { get; set; }
        public string Veterinario { get; set; }
        public DateTime FechaHora { get; set; }
        public string Tratamiento { get; set; }
        public decimal Costo { get; set; }
    }

}