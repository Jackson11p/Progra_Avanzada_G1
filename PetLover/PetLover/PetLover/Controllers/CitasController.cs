using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class CitasController : Controller
    {
        RegistroErrores error = new RegistroErrores();
        Utilitarios util = new Utilitarios();

        #region Ver Citas
        [HttpGet]
        public ActionResult ConsultarCitas()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarCitas().ToList();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarCitas");
                return View("Error");
            }
        }
        #endregion

        #region Registrar Cita
        [HttpGet]
        public ActionResult RegistrarCita()
        {
            try
            {
                CargarVeterinarios();
                CargarMascotas();
                CargarEstadosCita();
                CargarHorarios();
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get RegistrarCita");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RegistrarCita(CitaModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    DateTime fechaHoraCompleta = model.Fecha.Date + TimeSpan.Parse(model.Hora);
                    var result = context.RegistrarCita(fechaHoraCompleta, model.MascotaID, model.VeterinarioID, model.Descripcion, model.Estado);

                    if (result > 0)
                    {
                        TempData["MensajeExito"] = "La cita fue registrada exitosamente.";
                        return RedirectToAction("ConsultarCitas", "Citas");
                    }
                    else if (result == -1)
                    {
                        ViewBag.Mensaje = "El veterinario ya tiene una cita asignada en esa fecha y hora.";
                    }
                    else
                    {
                        ViewBag.Mensaje = "No se pudo registrar la cita.";
                    }

                    CargarVeterinarios();
                    CargarMascotas();
                    CargarEstadosCita();
                    CargarHorarios();
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post RegistrarCita");
                return View("Error");
            }
        }
        #endregion

        #region Cargar Veterinarios
        private void CargarVeterinarios()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarVeterinarios().ToList();

                var veterinarioCombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    veterinarioCombo.Add(new SelectListItem { Value = item.UsuarioID.ToString(), Text = item.Nombre });
                }

                ViewBag.VeterinarioCombo = veterinarioCombo;
            }
        }
        #endregion

        #region Cargar Mascotas
        private void CargarMascotas()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarMascotas().ToList();

                var mascotaCombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    mascotaCombo.Add(new SelectListItem { Value = item.MascotaID.ToString(), Text = item.Nombre });
                }

                ViewBag.MascotaCombo = mascotaCombo;
            }
        }
        #endregion

        #region Cargar EstadosCita
        private void CargarEstadosCita()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarEstadosCita().ToList();

                var estadosCitaCombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    estadosCitaCombo.Add(new SelectListItem { Value = item.EstadoID.ToString(), Text = item.Nombre });
                }

                ViewBag.EstadosCitaCombo = estadosCitaCombo;
            }
        }
        #endregion

        #region Cargar Horarios
        private void CargarHorarios()
        {
            List<SelectListItem> horarios = new List<SelectListItem>();
            DateTime horaInicio = DateTime.Today.AddHours(8);
            DateTime horaFin = DateTime.Today.AddHours(17);  

            for (var hora = horaInicio; hora <= horaFin; hora = hora.AddMinutes(30))
            {
                horarios.Add(new SelectListItem
                {
                    Text = hora.ToString("HH:mm"),
                    Value = hora.ToString("HH:mm")
                });
            }

            ViewBag.Horarios = horarios;

        }
        #endregion
    }
}