using hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios;
using hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaEfectivo.Servicios;
using hoka.Hoka.Models.Almacen.Caratula.Rutas;
using hoka.Hoka.Models.Banco.Caratula.Rutas;
using hoka.HokaCli.Models.Compuadmo.Moneda;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace hoka.Controllers.Finanza.Administracion.BancoEfectivo
{
    public class BancoCaratulaEfectivoController : Controller
    {
        private readonly string _textoCaratula = RtBancoCaratulaRutas.TextoBancoCaratula;
        private readonly string _textoCaratulaEfectivo = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;
        private readonly string _textoCaratulaReporte = RtBancoCaratulaRutas.TextoBancoCaratulaReporte;

        [HttpGet]
        public ActionResult Actualizar(EnAlmacenCaratula parametroCaratula)
        {
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            //EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
            //    .Permiso
            //    .PermisosAlmacenCaratula
            //    .Where(
            //        x =>
            //        x.AlmacenId == Caratula.AlmacenId)
            //    .Single();
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo =
                SvAppAlmacenCaratulaEfectivoConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            ICollection<EnMoneda> MonedasDenominacion =
                SvAppMonedaDenominacionConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            //ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaEfectivo = AlmacenCaratulaEfectivo;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlActualizarAlmacenCaratulaEfectivo = Url.Action("ActualizarBancoCaratulaEfectivo", _textoCaratulaEfectivo);
            ViewBag.UrlGrabarAlmacenCaratulaEfectivo = Url.Action("GrabarBancoCaratulaEfectivo", _textoCaratulaEfectivo);
            return View();
        }

        [HttpPost]
        public string ActualizarBancoCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
        {
            var respuesta = "error";
            if (parametroCaratulaEfectivo == null)
                return respuesta;
            bool B_ActualizarRegistro = SvAppAlmacenCaratulaEfectivoActualizar.Actualizar(parametroCaratulaEfectivo);
            if (B_ActualizarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpPost]
        public string GrabarBancoCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
        {
            var respuesta = "error";
            if (parametroCaratulaEfectivo == null)
                return respuesta;
            bool B_GrabarRegistro = SvAppAlmacenCaratulaEfectivoGrabar.Grabar(parametroCaratulaEfectivo);
            if (B_GrabarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpGet]
        public ActionResult Consultar(EnAlmacenCaratula parametroCaratula)
        {
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo =
                SvAppAlmacenCaratulaEfectivoConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            ICollection<EnMoneda> MonedasDenominacion =
                SvAppMonedaDenominacionConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaEfectivo = AlmacenCaratulaEfectivo;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            return View();
        }
    }
}