using hoka.AppServicios.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaraturlaGastosController : Controller
    {
        // GET: AlmacenCaraturlaGastos
        public ActionResult Index(int caratulaId)
        {
            #region Entidades Base
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula = SvAppAlmacenCaratulaConsultar.Consultar(caratulaId);
            ViewBag.Caratula = Caratula;
            #endregion

            #region Catálogos Base
            
            #endregion
            
            return View();
        }
    }
}