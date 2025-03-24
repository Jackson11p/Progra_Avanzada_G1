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
    public class UsuarioController : Controller
    {
        RegistroErrores error = new RegistroErrores();
        Utilitarios util = new Utilitarios();

        #region Ver Usuario
        [HttpGet]
        public ActionResult ConsultarUsuarios()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.ConsultarUsuarios().ToList();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ConsultarUsuarios");
                return View("Error");
            }
        }
        #endregion

        //Actualizar usuario, del lado del administrador mayormente cambiarle el perfil
        #region Actualizar Usuario
        [HttpGet]
        public ActionResult ActualizarUsuario(long q)
        {
            try
            {
                CargarPerfiles();
                using (var context = new PetLoverEntities())
                {
                    var info = context.Usuarios.Where(x => x.UsuarioID == q).FirstOrDefault();
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ActualizarUsuario");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ActualizarUsuario(Usuario model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Usuarios.Where(x => x.UsuarioID == model.UsuarioID).FirstOrDefault();
                    var result = context.ActualizarUsuario(model.UsuarioID, model.Identificacion, model.Nombre, model.Correo, model.Telefono, model.Estado, model.IdPerfil);

                    if (result > 0)
                    {
                        Session["IdPerfilUsuario"] = model.IdPerfil;
                        //Session["NombrePerfilUsuario"] = model.Perfil;
                        if (model.IdPerfil == 2)
                        {
                            return RedirectToAction("Index", "Principal");
                        }
                        else if (model.IdPerfil == 1)
                        {
                            return RedirectToAction("ConsultarUsuarios", "Usuario");
                        }
                    }

                    ViewBag.Mensaje = "La información no se ha podido actualizar correctamente";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarUsuario");
                return View("Error");
            }
        }

        #endregion

        #region Actualizar Datos
        [HttpGet]
        public ActionResult ActualizarDatos()
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    long idSesion = long.Parse(Session["IdUsuario"].ToString());
                    var info = context.Usuarios.Where(x => x.UsuarioID == idSesion).FirstOrDefault();

                    return View(info);
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ActualizarDatos");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ActualizarDatos(Usuario model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    long idSesion = long.Parse(Session["IdUsuario"].ToString());
                    var info = context.Usuarios.Where(x => x.UsuarioID == idSesion).FirstOrDefault();

                    var infoCorreo = context.Usuarios.Where(x => x.Correo == model.Correo
                                                             && x.UsuarioID != idSesion).FirstOrDefault();

                    if (infoCorreo == null)
                    {
                        var result = context.ActualizarUsuario(model.UsuarioID, model.Identificacion, model.Nombre, model.Correo, model.Telefono, model.Estado, model.IdPerfil);

                        if (result > 0)
                        {
                            Session["NombreUsuario"] = model.Nombre;
                            return RedirectToAction("Index", "Principal");
                        }
                    }

                    ViewBag.Mensaje = "Su información no se ha podido actualizar correctamente";
                    return View();
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ActualizarDatos");
                return View("Error");
            }
        }
        #endregion

        #region Cambiar contraseña
        [HttpGet]
        public ActionResult CambiarAcceso()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get CambiarAcceso");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult CambiarAcceso(UsuariosModel model)
        {
            try
            {
                if (model.Contrasenna != model.ConfirmarContrasenna)
                {
                    ViewBag.Mensaje = "Las contraseñas no coinciden, asegurese que ambas sean Idénticas";
                    return View();
                }

                using (var context = new PetLoverEntities())
                {
                    long idSesion = long.Parse(Session["IdUsuario"].ToString());
                    var info = context.Usuarios.Where(x => x.UsuarioID == idSesion).FirstOrDefault();

                    if (info != null)
                    {
                        if (model.ContrasennaAnterior != info.Contrasenna)
                        {
                            ViewBag.Mensaje = "Su contraseña actual es la misma que la anterior, por favor utiliza una nueva";
                            return View();
                        }

                        context.ActualizarContrasenna(info.Correo, model.Contrasenna);

                        string mensaje = util.MensajeCambioAcceso(info);
                        bool notificacion = util.EnviarCorreo(info, mensaje, "Cambio de Contraseña - PetLover");

                        if (notificacion)
                            return RedirectToAction("Index", "Principal");
                    }

                    ViewBag.Mensaje = "Su contraseña no se ha podido actualizar correctamente";
                    return View();
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post CambiarAcceso");
                return View("Error");
            }
        }
        #endregion

        #region Cargar Perfil
        private void CargarPerfiles()
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.ConsultarPerfiles().ToList();

                var perfilCombo = new List<SelectListItem>
        {
            new SelectListItem { Value = "", Text = "Seleccione" }
        };

                foreach (var item in info)
                {
                    perfilCombo.Add(new SelectListItem { Value = item.PerfilID.ToString(), Text = item.Nombre });
                }

                ViewBag.PerfilCombo = perfilCombo;
            }
        }
        #endregion
    }
}