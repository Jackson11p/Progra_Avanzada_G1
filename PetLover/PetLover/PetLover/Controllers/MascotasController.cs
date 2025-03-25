using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Parser.SyntaxTree;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class MascotasController : Controller
    {
        RegistroErrores error = new RegistroErrores();

        [HttpGet]
        public ActionResult MascotasDashboard(int? id)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    //si no se selecciona una mascota, mostrar el listado general
                    if (id == null)
                    {
                        var mascotas = context.Pets.Select(m => new MascotaDetalleModel
                        {
                            MascotaID = m.MascotaID,
                            Nombre = m.Nombre,
                            Especie = m.Especie,
                            Raza = m.Raza,
                            FechaNacimiento = m.FechaNacimiento,
                            ClienteNombre = m.Usuario.Nombre //relacion con Usuarios
                        }).ToList();
                        ViewBag.Mascotas = mascotas;
                        return View();
                    }
                    //si se selecciona una mascota, mostrar detalles y tratamientos
                    var mascota = context.Pets.Where(m => m.MascotaID == id).Select(m => new MascotaDetalleModel
                    {
                        MascotaID = m.MascotaID,
                        Nombre = m.Nombre,
                        Especie = m.Especie,
                        Raza = m.Raza,
                        FechaNacimiento = m.FechaNacimiento,
                        ClienteNombre = m.Usuario.Nombre,
                        Citas = m.Citas.Select(c => new CitaModel
                        {
                            FechaCita = c.FechaHora,
                            VeterinarioNombre = c.Veterinario.Nombre,
                            Descripcion = c.Descripcion
                        }).ToList(),
                        Tratamientos = m.Citas
                        .SelectMany(c => c.Tratamientos)
                        .Select(t => new TratamientoModel
                        {
                            NombreTratamiento = t.Nombre,
                            DescripcionTratamiento = t.Descripcion,
                            Costo = t.Costo
                        }).ToList()
                    }).FirstOrDefault();

                    if (mascota == null)
                    {
                        return HttpNotFound();
                    }
                    return View(mascota);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Error en MascotasDashboard");
                return View("Error");
            }
        }

    }
}