using hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios;
using hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVoucher.Servicio;
using hoka.Hoka.Models.Almacen.Caratula.Rutas;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Compuadmo.Voucher;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaVoucherController : Controller
    {
        string _textoCaratula = RtAlmacenCaratulaRutas.TextoCaratula;
        string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;

        // GET: AlmacenCaratulaVoucher
        [HttpGet]
        public ActionResult Index(EnAlmacenCaratula parametroCaratula)
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
            ICollection<EnVoucher> Vouchers =
                SvAppVoucherConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.Vouchers = Vouchers;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlCrearAlmacenCaratulaVoucher = Url.Action("CrearAlmacenCaratulaVoucher", _textoCaratulaVoucher);
            return View();
        }

        [HttpPost]
        public string CrearAlmacenCaratulaVoucher(EnAlmacenCaratulaVoucher parametroCaratulaVoucher)
        {
            var respuesta = "error";
            if (parametroCaratulaVoucher == null)
                return respuesta;
            bool B_CrearRegistro = SvAppAlmacenCaratulaVoucherCrear.Crear(parametroCaratulaVoucher);
            if (B_CrearRegistro)
            {
                EnAlmacenCaratula Caratula = SvAppAlmacenCaratulaConsultar
                    .Consultar(parametroCaratulaVoucher.AlmacenCaratulaId);
                EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
                EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                    .Permiso
                    .PermisosAlmacenCaratula
                    .Where(
                        x =>
                        x.AlmacenId == Caratula.AlmacenId)
                    .Single();
                string controlador = PermisoCaratula.B_CaratulaEfectivoCrear
                    ? _textoCaratulaReporte
                    : _textoCaratula;
                respuesta = Url.Action("Index", controlador, Caratula);
            }
            return respuesta;
        }

        [HttpGet]
        public ActionResult Actualizar(EnAlmacenCaratula parametroCaratula)
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
            EnAlmacenCaratulaVoucher AlmacenCaratulaVoucher =
                SvAppAlmacenCaratulaVoucherConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            ICollection<EnVoucher> Vouchers = SvAppVoucherConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaVoucher = AlmacenCaratulaVoucher;
            ViewBag.Vouchers = Vouchers;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlActualizarAlmacenCaratulaVoucher = Url.Action("ActualizarAlmacenCaratulaVoucher", _textoCaratulaVoucher);
            ViewBag.UrlGrabarAlmacenCaratulaVoucher = Url.Action("GrabarAlmacenCaratulaVoucher", _textoCaratulaVoucher);
            return View();
        }

        [HttpPost]
        public string ActualizarAlmacenCaratulaVoucher(EnAlmacenCaratulaVoucher parametroCaratulaVoucher)
        {
            var respuesta = "error";
            if (parametroCaratulaVoucher == null)
                return respuesta;
            bool B_ActualizarRegistro = SvAppAlmacenCaratulaVoucherActualizar.Actualizar(parametroCaratulaVoucher);
            if (B_ActualizarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpPost]
        public string GrabarAlmacenCaratulaVoucher(EnAlmacenCaratulaVoucher parametroCaratulaVoucher)
        {
            var respuesta = "error";
            if (parametroCaratulaVoucher == null)
                return respuesta;
            bool B_GrabarRegistro = SvAppAlmacenCaratulaVoucherGrabar.Grabar(parametroCaratulaVoucher);
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
            EnAlmacenCaratulaVoucher AlmacenCaratulaVoucher =
                SvAppAlmacenCaratulaVoucherConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
            ICollection<EnVoucher> Vouchers =
                SvAppVoucherConsultar.Consultar();
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaVoucher = AlmacenCaratulaVoucher;
            ViewBag.Vouchers = Vouchers;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            return View();
        }
    }
}