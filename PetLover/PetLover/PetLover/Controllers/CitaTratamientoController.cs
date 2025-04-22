using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class CitaTratamientoController : Controller
    {
        RegistroErrores error = new RegistroErrores();

        #region Ver Citas Tratamientos
        [HttpGet]
        public ActionResult ConsultarCitasTratamientos()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var citas = context.Database.SqlQuery<CitaTratamientoViewModel>(
                        "EXEC ConsultarCitasTratamientos").ToList();

                    return View(citas);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarCitasTratamientos");
                return View("Error");
            }
        }

        #endregion

        #region Registrar Cita Tratamiento
        [HttpGet]
        public ActionResult RegistrarCitaTratamiento()
        {
            try
            {
                CargarCitas();
                CargarTratamientos();
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get RegistrarCitaTratamiento");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RegistrarCitaTratamiento(CitaTratamientoModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    if (model.TratamientosSeleccionados != null && model.TratamientosSeleccionados.Any())
                    {
                        foreach (var tratamientoID in model.TratamientosSeleccionados)
                        {
                            context.RegistrarCitaTratamiento(model.CitaID, tratamientoID);
                        }

                        TempData["MensajeExito"] = "Los tratamientos fueron registrados exitosamente.";
                        return RedirectToAction("ConsultarCitasTratamientos", "CitaTratamiento");
                    }
                    else
                    {
                        ViewBag.Mensaje = "Debe seleccionar al menos un tratamiento.";
                    }

                    CargarTratamientos();
                    CargarCitas();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post RegistrarCitaTratamiento");
                return View("Error");
            }
        }
        #endregion

        #region Actualizar Cita Tratamiento
        [HttpGet]
        public ActionResult ActualizarCitaTratamiento(int q)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var tratamientosActuales = context.CitaTratamientos
                        .Where(x => x.CitaID == q)
                        .Select(x => x.TratamientoID)
                        .ToList();

                    var todosLosTratamientos = context.Tratamientos.ToList();

                    var model = new CitaTratamientoModel
                    {
                        CitaID = q,
                        TratamientosSeleccionados = tratamientosActuales,
                        Tratamientos = todosLosTratamientos
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ActualizarCitaTratamiento");
                return View("Error");
            }
        }



        [HttpPost]
        public ActionResult ActualizarCitaTratamiento(CitaTratamientoModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var anteriores = context.CitaTratamientos
                        .Where(x => x.CitaID == model.CitaID)
                        .Select(x => x.TratamientoID)
                        .ToList();

                    var nuevos = model.TratamientosSeleccionados ?? new List<int>();

                    foreach (var anterior in anteriores)
                    {
                        if (!nuevos.Contains(anterior))
                        {
                            var resultado = context.EliminarCitaTratamiento(model.CitaID, anterior);
                        }
                    }
                    foreach (var nuevo in nuevos)
                    {
                        if (!anteriores.Contains(nuevo))
                        {
                            var resultado = context.ActualizarCitaTratamiento(model.CitaID, 0, nuevo); 
                        }
                    }

                    TempData["MensajeExito"] = "Los tratamientos fueron actualizados correctamente.";
                    return RedirectToAction("ConsultarCitasTratamientos", "CitaTratamiento");
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarCitaTratamiento");
                return View("Error");
            }
        }
        #endregion

        #region Cargar Tratamiento
        private void CargarTratamientos()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarTratamientos().ToList();

                var lista = info.Select(t => new SelectListItem
                {
                    Value = t.TratamientoID.ToString(),
                    Text = t.Nombre
                }).ToList();

                ViewBag.TratamientosLista = lista;
            }
        }
        #endregion

        #region Cargar Cita
        private void CargarCitas()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarCitas().ToList();

                var citacombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    citacombo.Add(new SelectListItem
                    {
                        Value = item.CitaID.ToString(),
                        Text = item.Descripcion
                    });
                }

                ViewBag.CitaCombo = citacombo;
            }
        }
        #endregion
    }
}