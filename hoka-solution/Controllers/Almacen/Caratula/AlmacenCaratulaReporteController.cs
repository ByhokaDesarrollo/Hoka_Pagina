using hoka.AppServicios.Ingresos.AlmacenCaratulaReporte;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using System.Web.Mvc;
using hoka.AppServicios.Ingresos.AlmacenCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using System.Linq;
using hoka.AppServicios.Ingresos.AlmacenCaratula;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaReporteController : Controller
    {
        private readonly string _textoCaratula = RtAlmacenCaratulaRutas.TextoCaratula;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;

        // GET: AlmacenCaratulaController
        public ActionResult Index(EnAlmacenCaratula parametroCaratula)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula = SvAppAlmacenCaratulaReporteConsultar.Consultar(parametroCaratula);
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.UrlAlmacenCaratulaGrabar = Url.Action("AlmacenCaratulaGrabar", _textoCaratulaReporte);
            ViewBag.UrlAlmacenCaratulaDesbloquear = Url.Action("AlmacenCaratulaDesbloquear", _textoCaratulaReporte);
            return View(Caratula);
        }

        [HttpPost]
        public string AlmacenCaratulaGrabar(EnAlmacenCaratula parametroCaratula)
        {
            var respuesta = "error";
            if (parametroCaratula == null)
                return respuesta;
            if (parametroCaratula.AlmacenCaratulaId <= 0)
                return respuesta;
            bool B_GrabarRegistro = SvAppAlmacenCaratulaGrabar.Grabar(parametroCaratula);
            if (B_GrabarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpPost]
        public string AlmacenCaratulaDesbloquear(EnAlmacenCaratula parametroCaratula)
        {
            var respuesta = "error";
            if (parametroCaratula == null)
                return respuesta;
            if (parametroCaratula.AlmacenCaratulaId <= 0)
                return respuesta;
            bool B_DesbloquearRegistro = SvAppAlmacenCaratulaDesbloquear.Desbloquear(parametroCaratula);
            if (B_DesbloquearRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }
    }
}