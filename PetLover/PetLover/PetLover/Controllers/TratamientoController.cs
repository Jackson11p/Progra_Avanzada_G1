using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class TratamientoController : Controller
    {
        RegistroErrores error = new RegistroErrores();

        #region Ver tratamientos
        [HttpGet]
        public ActionResult ConsultarTratamientos()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarTratamientos().ToList();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarTratamientos");
                return View("Error");
            }
        }
        #endregion

        #region Ver tratamientos inactivos
        [HttpGet]
        public ActionResult ConsultarTratamientosInactivos()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarTratamientosInactivos().ToList();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarTratamientos");
                return View("Error");
            }
        }
        #endregion

        #region Registrar Tratamientos
        [HttpGet]
        public ActionResult RegistrarTratamiento()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get RegistrarTratamiento");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RegistrarTratamiento(TratamientoModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {

                    var result = context.RegistrarTratamiento(model.Nombre, model.Descripcion, model.Costo, model.Estado);

                    if (result > 0)
                        return RedirectToAction("ConsultarTratamientos", "Tratamiento");
                    else
                    {
                        TempData["MensajeExito"] = "El tratamiento se registró correctamente.";
                        ViewBag.Mensaje = "Su información no se ha podido registrar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post RegistrarTratamiento");
                return View("Error");
            }

        }
        #endregion

        #region Actualizar Tratamiento
        [HttpGet]
        public ActionResult ActualizarTratamiento(long q)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Tratamientos.Where(x => x.TratamientoID == q).FirstOrDefault();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ActualizarTratamiento");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ActualizarTratamiento(Tratamientos model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Tratamientos.Where(x => x.TratamientoID == model.TratamientoID).FirstOrDefault();
                    var result = context.ActualizarTratamiento(model.TratamientoID, model.Nombre, model.Descripcion, model.Costo, model.Estado);
                    if (result > 0)
                    {
                        TempData["MensajeExito"] = "El tratamiento se actualizo correctamente.";
                        return RedirectToAction("ConsultarTratamientos", "Tratamiento");
                    }
                    else
                    {
                        ViewBag.Mensaje = "La información no se ha podido actualizar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarTratamiento");
                return View("Error");
            }
        }
        #endregion
    }
}
    

