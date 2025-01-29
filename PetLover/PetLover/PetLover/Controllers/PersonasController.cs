using System;
using System.Web.Mvc;
using PetLover.Models;

namespace PetLover.Controllers
{
    public class PersonasController : Controller
    {
        [HttpGet]
        public ActionResult RegistrarPersona()
        {
            var personasModelo = new PersonasModel();
            personasModelo.Nombre = "EDUARDO"; 

            return View(personasModelo);
        }

        [HttpPost]
        public ActionResult RegistrarPersona(string Identificacion, string Nombre)
        {
            return RedirectToAction("Index", "Home");
        }
    }
}