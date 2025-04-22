using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class HistorialMedicoController : Controller
    {
        Utilitarios util = new Utilitarios();
        RegistroErrores error = new RegistroErrores();

        #region Ver Historial Medico
        [HttpGet]
        public ActionResult ConsultarHistorialMedico()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var historial = context.Database.SqlQuery<HistorialMedicoViewModel>(
                        "EXEC ConsultarHistorialMedico").ToList();

                    return View(historial);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarHistorialMedico");
                return View("Error");
            }
        }
        #endregion

        #region Registrar Cita Tratamiento
        [HttpGet]
        public ActionResult RegistrarHistorialMedico()
        {
            try
            {
                CargarCitasParaHistorial();
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get RegistrarHistorialMedico");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RegistrarHistorialMedico(HistorialMedicoViewModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var result = context.RegistrarHistorialMedico(model.CitaID, model.Diagnostico);
                    if (result > 0)
                    {
                        var cita = context.Citas.Find(model.CitaID);
                        var mascota = context.Mascotas.Find(cita.MascotaID);
                        var veterinario = context.Usuarios.Find(cita.VeterinarioID);
                        var duenno = context.Usuarios.Find(mascota.IDUsuario);


                        var tratamientos = (from ct in context.CitaTratamientos
                                            join t in context.Tratamientos on ct.TratamientoID equals t.TratamientoID
                                            where ct.CitaID == model.CitaID
                                            select t.Nombre).ToList();


                        string mensaje = util.MensajeHistorialMedicoRegistrado(
                                duenno,
                                cita.FechaHora,
                                mascota.Nombre,
                                veterinario.Nombre,
                                model.Diagnostico,
                                model.MontoTotal,
                                tratamientos
                            );

                        util.EnviarCorreo(duenno, mensaje, "Historial Médico Registrado - PetLover");

                        TempData["MensajeExito"] = "Historial médico registrado correctamente.";
                        return RedirectToAction("ConsultarHistorialMedico", "HistorialMedico");
                    }
                    else
                    {
                        ViewBag.Error = "Ocurrió un error al registrar el historial médico.";
                        CargarCitasParaHistorial();
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post RegistrarHistorialMedico");
                ViewBag.Error = "Ocurrió un error al registrar el historial médico.";
                return View(model);
            }
        }

        #endregion

        #region Obtener monto por cita
        [HttpGet]
        public JsonResult ObtenerMontoPorCita(int citaID)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var monto = context.ObtenerMontoTotalCita(citaID).FirstOrDefault();

                    return Json(new { monto = monto ?? 0 }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "ObtenerMontoPorCita");
                return Json(new { error = "Error al calcular el monto." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Cargar Cita
        private void CargarCitasParaHistorial()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarCitasParaHistorial().ToList();

                var citahistorialcombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    citahistorialcombo.Add(new SelectListItem
                    {
                        Value = item.CitaID.ToString(),
                        Text = item.Descripcion
                    });
                }

                ViewBag.Citahistorialcombo = citahistorialcombo;
            }
        }
        #endregion
    }
}