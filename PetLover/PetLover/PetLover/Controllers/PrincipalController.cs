using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PetLover.BaseDatos;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class PrincipalController : Controller
    {
        // GET: Principal
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UsuariosModel model)
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
  
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UsuariosModel model)
        {
            using (var context = new PetLoverEntities())
            {
                var info = context.IniciarSesion(model.Identificacion, model.Contrasenna).FirstOrDefault();
                if (info != null)
                {
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
        

        [HttpGet]
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Principal");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

    }
}