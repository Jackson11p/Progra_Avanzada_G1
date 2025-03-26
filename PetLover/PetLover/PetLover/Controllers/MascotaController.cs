using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class MascotaController : Controller
    {
        RegistroErrores error = new RegistroErrores();

        #region Ver usuarios
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
        #endregion

        #region Registrar Mascota
        [HttpGet]
        public ActionResult RegistrarMascota()
        {
            try
            {
                CargarUsuarios();
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get RegistrarMascota");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult RegistrarMascota(MascotaModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {

                    var result = context.RegistrarMascota(model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.Estado, model.IDUsuario);

                    if (result > 0)
                        return RedirectToAction("ConsultarMascotas", "Mascota");
                    else
                    {
                        ViewBag.Mensaje = "Su información no se ha podido registrar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post RegistrarMascota");
                return View("Error");
            }

        }
        #endregion

        #region Actualizar Mascota
        [HttpGet]
        public ActionResult ActualizarMascota(long q)
        {
            try
            {
                CargarUsuarios();
                using (var context = new PetLoverEntities())
                {
                    var info = context.Mascotas.Where(x => x.MascotaID == q).FirstOrDefault();
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
        public ActionResult ActualizarMascota(Mascota model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Mascotas.Where(x => x.MascotaID == model.MascotaID).FirstOrDefault();
                    var result = context.ActualizarMascota(model.MascotaID, model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.Estado, model.IDUsuario);

                    if (result > 0)
                    {          
                        return RedirectToAction("ConsultarMascotas", "Mascota");
                    }
                    else{
                        ViewBag.Mensaje = "La información no se ha podido actualizar correctamente";
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarMascota");
                return View("Error");
            }
        }
        #endregion

        #region Cargar Usuarios
        private void CargarUsuarios()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.CargarUsuarios().ToList();

                var usuarioCombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    usuarioCombo.Add(new SelectListItem { Value = item.UsuarioID.ToString(), Text = item.Nombre });
                }

                ViewBag.UsuarioCombo = usuarioCombo;
            }
        }
        #endregion

    }


}