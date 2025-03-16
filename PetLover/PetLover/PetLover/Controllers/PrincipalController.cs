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

        // GET: Principal
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
                    var info = context.Usuarios.Where(x => x.Correo == model.Correo
                                                       && x.Estado == true).FirstOrDefault();

                    if (info != null)
                    {
                        var codigoTemporal = CrearCodigo();
                        var result = context.ActualizarContrasenna(model.Correo, codigoTemporal);
                        var notificacion = EnviarCorreo(info, codigoTemporal, "Acceso al sistema PetLover");
                        if (notificacion)
                            return RedirectToAction("Login", "Principal");
                    }

                    ViewBag.Mensaje = "Su acceso no se ha podido reestablecer correctamente";
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

        #region Envio Correo
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

        private bool EnviarCorreo(Usuario info, string codigo, string titulo)
        {
            string cuenta = ConfigurationManager.AppSettings["CorreoNotificaciones"].ToString();
            string contrasenna = ConfigurationManager.AppSettings["ContrasennaNotificaciones"].ToString();

            MailMessage message = new MailMessage();
            message.From = new MailAddress(cuenta);
            message.To.Add(new MailAddress(info.Correo));
            message.Subject = titulo;

            // Cuerpo del correo en HTML
            string htmlBody = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f4f4f4;
                            color: #333;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            max-width: 600px;
                            margin: 20px auto;
                            padding: 20px;
                            background-color: #fff;
                            border-radius: 8px;
                            box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            font-size: 24px;
                            color: #007BFF;
                            margin-bottom: 20px;
                        }}
                        .code {{
                            font-size: 28px;
                            font-weight: bold;
                            color: #007BFF;
                            margin: 20px 0;
                            padding: 10px;
                            background-color: #e9f5ff;
                            border-radius: 4px;
                            text-align: center;
                        }}
                        .footer {{
                            margin-top: 20px;
                            font-size: 14px;
                            color: #777;
                            text-align: center;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>Hola {info.Nombre},</div>
                        <p>Por favor, utilice el siguiente código para ingresar al sistema:</p>
                        <div class='code'>{codigo}</div>
                        <p>Esta es una contraseña temporal, por favor al entrar al sistena cambiela.</p>
                        <p>Si no solicitó este código, por favor ignore este mensaje.</p>
                        <div class='footer'>
                            Este es un correo automático, por favor no responda a este mensaje.
                        </div>
                    </div>
                </body>
                </html>";

            message.Body = htmlBody;
            message.IsBodyHtml = true;
            message.Priority = MailPriority.Normal;

            SmtpClient client = new SmtpClient("smtp.office365.com", 587);
            client.Credentials = new System.Net.NetworkCredential(cuenta, contrasenna);
            client.EnableSsl = true;

            try
            {
                client.Send(message);
                return true;
            }
            catch (Exception ex)
            {
                // Registrar el error si es necesario
                error.RegistrarError(ex.Message, "EnviarCorreo");
                return false;
            }
        }

        #endregion
    }
}