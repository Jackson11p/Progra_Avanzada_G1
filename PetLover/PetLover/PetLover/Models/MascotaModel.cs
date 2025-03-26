using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class MascotaModel
    {
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Estado { get; set; }
        public int IDUsuario { get; set; }

    }
}