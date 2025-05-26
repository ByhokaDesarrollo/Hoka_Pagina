using hoka.AppServicios.Ingresos.AlmacenCaratula;
using hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo;
using hoka.AppServicios.Ingresos.BancoCaratulaEfectivo;
using hoka.AppServicios.Ingresos.BancoCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.Moneda;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using System.Collections.Generic;
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
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo =
                SvAppAlmacenCaratulaEfectivoConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            ICollection<EnMoneda> MonedasDenominacion =
                SvAppMonedaDenominacionConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            //ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaEfectivo = AlmacenCaratulaEfectivo;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlActualizarBancoCaratulaEfectivo = Url.Action("ActualizarBancoCaratulaEfectivo", _textoCaratulaEfectivo);
            ViewBag.UrlGrabarBancoCaratulaEfectivo = Url.Action("GrabarBancoCaratulaEfectivo", _textoCaratulaEfectivo);
            return View();
        }

        [HttpPost]
        public string ActualizarBancoCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
        {
            var respuesta = "error";
            if (parametroCaratulaEfectivo == null)
                return respuesta;
            bool B_ActualizarRegistro = SvAppBancoCaratulaEfectivoActualizar.Actualizar(parametroCaratulaEfectivo);
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
            bool B_GrabarRegistro = SvAppBancoCaratulaEfectivoGrabar.Grabar(parametroCaratulaEfectivo);
            if (B_GrabarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }
    }
}