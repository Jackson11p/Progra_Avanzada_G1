﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PetLover.Models
{
    public class UsuariosModel
    {
        public string Identificacion { get; set; }
        public string Contrasenna { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string ContrasennaAnterior { get; set; }
        public string ConfirmarContrasenna { get; set; }
      
    }
}