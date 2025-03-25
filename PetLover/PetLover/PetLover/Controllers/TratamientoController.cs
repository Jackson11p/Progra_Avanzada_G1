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

        // Consultar tratamientos
        public ActionResult Index()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var tratamientos = context.ConsultarTratamientos().ToList();
                    return View(tratamientos);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Index Tratamiento");
                return View("Error");
            }
        }

        
        public ActionResult Crear()
        {
            return View();
        }

        // Crear tratamienti
        [HttpPost]
        public ActionResult Crear(Tratamientos model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new PetLoverEntities())
                    {
                        context.RegistrarTratamiento(model.Nombre, model.Descripcion, model.Costo);
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post Crear Tratamiento");
                return View("Error");
            }
        }

        // Editar el tratamiento
        public ActionResult Editar(int id)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var tratamiento = context.Tratamientos.Find(id);
                    if (tratamiento == null)
                        return HttpNotFound();
                    return View(tratamiento);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Editar Tratamiento");
                return View("Error");
            }
        }

        // Editar tratamiento 
        [HttpPost]
        public ActionResult Editar(Tratamientos model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var context = new PetLoverEntities())
                    {
                        context.ActualizarTratamiento(model.TratamientoID, model.Nombre, model.Descripcion, model.Costo);
                    }
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post Editar Tratamiento");
                return View("Error");
            }
        }

        // Eliminar tratamiento
        public ActionResult Eliminar(int id)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var tratamiento = context.Tratamientos.Find(id);
                    if (tratamiento == null)
                        return HttpNotFound();
                    return View(tratamiento);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Eliminar Tratamiento");
                return View("Error");
            }
        }

        // Eliminar tratamiento
        [HttpPost, ActionName("Eliminar")]
        public ActionResult ConfirmarEliminar(int id)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    context.EliminarTratamiento(id);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post Eliminar Tratamiento");
                return View("Error");
            }
        }
    }
}
    

