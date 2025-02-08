using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hoka.Permisos;

namespace hoka.Controllers
{
    
    public class HomeController : Controller
    {
        [ValidarSesion(idRol: 3)]
        public ActionResult Index()
        {
            return View();
        }
        [ValidarSesion(idRol: 3)]
        public ActionResult Build()
        {
            return View();
        }

        public ActionResult ErrorView()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

    }
}