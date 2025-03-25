using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class PetsController : Controller
    {
        RegistroErrores error = new RegistroErrores();
        Utilitarios util = new Utilitarios();

        [HttpGet]
        public ActionResult ConsultarMascotas()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarMascotas().ToList();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarMascotas");
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult AgregarMascota()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get AgregarMascota");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult AgregarMascota(PetsModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {

                    var result = context.AgregarMascota(model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.ClienteID);

                    if (result > 0)
                        return RedirectToAction("ConsultarMascotas", "Pets");
                    else
                    {
                        ViewBag.Mensaje  = "La información de la mascota no se ha podido agregar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post AgregarMascota");
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult ActualizarMascota(int q)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Pets.Where(x => x.MascotaID == q).FirstOrDefault();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ActualizarMascota");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ActualizarMascota(PetsModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var result = context.ActualizarMascota(model.MascotaID, model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.ClienteID);

                    if (result > 0)
                        return RedirectToAction("ConsultarMascotas", "Pets");
                    else
                    {
                        ViewBag.Mensaje = "La información de la mascota no se ha podido actualizar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarMascota");
                return View("Error");
            }
        }

    }
}