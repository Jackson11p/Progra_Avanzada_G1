using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetLover.BaseDatos;

namespace PetLover.Models
{
    public class CitaTratamientoModel
    {
        public int CitaID { get; set; }

        public List<int> TratamientosSeleccionados { get; set; }

        public virtual Cita Cita { get; set; }
        public virtual List<Tratamiento> Tratamientos { get; set; }
    }

}