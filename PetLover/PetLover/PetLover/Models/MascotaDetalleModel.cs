using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class MascotaDetalleModel
    {
        //info de la mascota
        public int MascotaID { get; set; }
        public string Nombre { get; set; }
        public string Especie { get; set; }
        public string Raza { get; set; }
        public DateTime? FechaNacimiento { get; set; }

        //info del dueño de la mascota
        public int ClienteID { get; set; }
        public string ClienteNombre { get; set; }

        //info de citas
        public List<CitaModel> Citas { get; set; } = new List<CitaModel>();

        //info de tratamientos
        public List<TratamientoModel> Tratamientos { get; set; } = new List<TratamientoModel>();
    }
    //Modelo para las citas
    public class CitaModel
    {
        public DateTime FechaCita { get; set; }
        public string VeterinarioNombre { get; set; }
        public string Descripcion { get; set; }
    }

    //modelo para los tratamientos
    public class TratamientoModel
    {
        public string NombreTratamiento { get; set; }
        public string DescripcionTratamiento { get; set; }
        public decimal Costo { get; set; }
    }
}