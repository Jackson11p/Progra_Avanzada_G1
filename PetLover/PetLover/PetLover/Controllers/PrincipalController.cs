using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class PrincipalController : Controller
    {
        RegistroErrores error = new RegistroErrores();
        Utilitarios util = new Utilitarios();
        public ActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Index");
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Nosotros()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Nosotros");
                return View("Error");
            }
        }

        [HttpGet]
        public ActionResult Productos()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Nosotros");
                return View("Error");
            }
        }
      
        #region RegistrarCuenta
        [HttpGet]
        public ActionResult Register()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Register");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Register(UsuariosModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {

                    var result = context.RegistrarCuenta(model.Identificacion, model.Contrasenna, model.Nombre, model.Correo, model.Telefono);

                    if (result > 0)
                        return RedirectToAction("Login", "Principal");
                    else
                    {
                        ViewBag.Mensaje = "Su información no se ha podido registrar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post Register");
                return View("Error");
            }
            
        }
        #endregion

        #region Iniciar sesion
        [HttpGet]
        public ActionResult Login()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get Login");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Login(UsuariosModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.IniciarSesion(model.Correo, model.Contrasenna).FirstOrDefault();
                    if (info != null)
                    {
                        Session["IdUsuario"] = info.UsuarioID;
                        Session["NombreUsuario"] = info.NombreUsuario;
                        Session["NombrePerfilUsuario"] = info.NombrePerfil;
                        Session["IdPerfilUsuario"] = info.IdPerfil;
                        return RedirectToAction("Index", "Principal");
                    }
                    else
                    {
                        ViewBag.Mensaje = "Su información no se ha podido validar correctamente";
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post Login");
                return View("Error");
            }
            
        }
        #endregion
                     
        #region Recuperar

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get ForgotPassword");
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult ForgotPassword(UsuariosModel model)
        {
            try
            {
                using (var context = new PetLoverEntities())
                {
                    var info = context.Usuarios.FirstOrDefault(x => x.Correo == model.Correo && x.Estado == true);

                    if (info != null)
                    {
                        var codigoTemporal = CrearCodigo();
                        context.ActualizarContrasenna(model.Correo, codigoTemporal);

                        string mensaje = util.MensajeRecuperacion(info, codigoTemporal);
                        bool notificacion = util.EnviarCorreo(info, mensaje, "Acceso al sistema PetLover");

                        if (notificacion)
                            return RedirectToAction("Login", "Principal");
                    }

                    ViewBag.Mensaje = "Su acceso no se ha podido restablecer correctamente";
                    return View();
                }
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Post ForgotPassword");
                return View("Error");
            }
        }
        #endregion

        #region Cerrar Sesion
        [HttpGet]
        public ActionResult Logout()
        {
            try
            {
                Session.Clear();
                return RedirectToAction("Index", "Principal");
            }
            catch (Exception ex)
            {
                error.RegistrarError(ex.Message, "Get CerrarSesion");
                return View("Error");
            }
        }
        #endregion

        #region Crea Codigo
        private string CrearCodigo()
        {
            int length = 5;
            const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }
        #endregion
    }
}