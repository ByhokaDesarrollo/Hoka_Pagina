using hoka.AppServicios.Ingresos.AlmacenCaratula;
using hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo;
using hoka.AppServicios.Ingresos.AlmacenCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.Moneda;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaEfectivoController : Controller
    {
        private readonly string _textoCaratula = RtAlmacenCaratulaRutas.TextoCaratula;
        private readonly string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        private readonly string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        private readonly string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;

        // GET: AlmacenCaratulaEfectivo
        [HttpGet]
        public ActionResult Index(EnAlmacenCaratula parametroCaratula)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula = SvAppAlmacenCaratulaConsultar
                .Consultar(parametroCaratula.AlmacenCaratulaId);
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            ICollection<EnMoneda> MonedasDenominacion =
                SvAppMonedaDenominacionConsultar.Consultar();
            ViewBag.Usuario = Usuario;
            ViewBag.TextoCaratulaVoucher = _textoCaratulaVoucher;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlCrearAlmacenCaratulaEfectivo = Url.Action("CrearAlmacenCaratulaEfectivo", _textoCaratulaEfectivo);
            return View();
        }

        [HttpPost]
        public string CrearAlmacenCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
        {
            var respuesta = "error";
            if (parametroCaratulaEfectivo == null)
                return respuesta;
            bool B_CrearRegistro = SvAppAlmacenCaratulaEfectivoCrear.Crear(parametroCaratulaEfectivo);
            if (B_CrearRegistro)
            {
                EnAlmacenCaratula Caratula =
                    SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratulaEfectivo.AlmacenCaratulaId);
                EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
                EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                    .Permiso
                    .PermisosAlmacenCaratula
                    .Where(
                        x =>
                        x.AlmacenId == Caratula.AlmacenId)
                    .Single();
                string controlador = PermisoCaratula.B_CaratulaVoucherCrear
                    ? _textoCaratulaVoucher
                    : _textoCaratula;
                respuesta = Url.Action("Index", controlador, Caratula);
            }
            return respuesta;
        }

        [HttpGet]
        public ActionResult Actualizar(EnAlmacenCaratula parametroCaratula)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
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
            ViewBag.Usuario = Usuario;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaEfectivo = AlmacenCaratulaEfectivo;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlActualizarAlmacenCaratulaEfectivo = Url.Action("ActualizarAlmacenCaratulaEfectivo", _textoCaratulaEfectivo);
            ViewBag.UrlGrabarAlmacenCaratulaEfectivo = Url.Action("GrabarAlmacenCaratulaEfectivo", _textoCaratulaEfectivo);
            return View();
        }

        [HttpPost]
        public string ActualizarAlmacenCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
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
        public string GrabarAlmacenCaratulaEfectivo(EnAlmacenCaratulaEfectivo parametroCaratulaEfectivo)
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
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
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
            ViewBag.Usuario = Usuario;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaEfectivo = AlmacenCaratulaEfectivo;
            ViewBag.MonedasDenominacion = MonedasDenominacion;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            return View();
        }
    }
}