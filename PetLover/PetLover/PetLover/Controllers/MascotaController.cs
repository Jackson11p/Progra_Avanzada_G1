using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
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

        #region Ver Mascotas
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

        #region Ver Mascotas Inactivas
        [HttpGet]
        public ActionResult ConsultarMascotasInactivas()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarMascotasInactivas().ToList();
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

        #region Ver Mascotas por Usuario
        [HttpGet]
        public ActionResult MisMascotas()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    int idSesion = int.Parse(Session["IdUsuario"].ToString());
                    var info = context.ConsultarMascotasPorUsuario(idSesion).ToList();
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
        public ActionResult RegistrarMascota(MascotaModel model, HttpPostedFileBase ImagenMascota)
        {
            try
            {
                string rutaImagen = null;

                if (ImagenMascota != null && ImagenMascota.ContentLength > 0)
                {
                    string extension = Path.GetExtension(ImagenMascota.FileName).ToLower();
                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaFisica = Path.Combine(Server.MapPath("~/ImagenesMascota"), nombreArchivo);

                    Directory.CreateDirectory(Path.GetDirectoryName(rutaFisica));
                    ImagenMascota.SaveAs(rutaFisica);
                    rutaImagen = "/ImagenesMascota/" + nombreArchivo;
                }
                using (var context = new PetLoverEntities())
                {
                    var result = context.RegistrarMascota(model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.Estado, model.IDUsuario, rutaImagen);

                    if (result > 0)
                    {
                        TempData["MensajeExito"] = "La mascota se registró correctamente.";
                        return RedirectToAction("ConsultarMascotas", "Mascota");
                    }
                    else
                    {
                        ViewBag.Mensaje = "No se pudo registrar la mascota";
                        CargarUsuarios();
                        return View(model);
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
        public ActionResult ActualizarMascota(Mascota model, HttpPostedFileBase ImagenMascota)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    string rutaImagen = model.Imagen;

                    if (ImagenMascota != null && ImagenMascota.ContentLength > 0)
                    {
                        if (!string.IsNullOrEmpty(model.Imagen))
                        {
                            string rutaAntigua = Server.MapPath(model.Imagen);
                            if (System.IO.File.Exists(rutaAntigua))
                            {
                                System.IO.File.Delete(rutaAntigua);
                            }
                        }
                        string extension = Path.GetExtension(ImagenMascota.FileName).ToLower();

                        string nombreArchivo = $"{model.MascotaID}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                        string rutaFisica = Path.Combine(Server.MapPath("~/ImagenesMascota"), nombreArchivo);
                        Directory.CreateDirectory(Path.GetDirectoryName(rutaFisica));
                        ImagenMascota.SaveAs(rutaFisica);
                        rutaImagen = "/ImagenesMascota/" + nombreArchivo;
                    }

                    var result = context.ActualizarMascota(model.MascotaID, model.Nombre, model.Especie, model.Raza, model.FechaNacimiento, model.Estado, model.IDUsuario, rutaImagen);

                    if (result > 0)
                    {
                        TempData["MensajeExito"] = "La mascota se actualizó correctamente.";
                        return RedirectToAction("ConsultarMascotas", "Mascota");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo actualizar la mascota";
                        CargarUsuarios();
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