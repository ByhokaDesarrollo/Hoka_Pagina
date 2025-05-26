using hoka.AppServicios.Ingresos.AlmacenCaratulaVenta;
using hoka.AppServicios.Ingresos.AlmacenCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using hoka.AppServicios.Ingresos.AlmacenCaratula;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaVentaController : Controller
    {
        private readonly string _textoCaratula = RtAlmacenCaratulaRutas.TextoCaratula;
        private readonly string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        private readonly string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        private readonly string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;

        // GET: AlmacenCaratulaVenta
        [HttpGet]
        public ActionResult Index(int almacenCaratulaId)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula = SvAppAlmacenCaratulaConsultar
                .Consultar(almacenCaratulaId);
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            ICollection<EnAlmacenCategoriaVenta> CategoriasVenta =
                SvAppAlmacenCaratulaVentaConsultarCategoriaVentaDelDia.Consultar(Caratula);
            
            ViewBag.Usuario = Usuario;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.CategoriasVenta = CategoriasVenta;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlAlmacenCaratulaVentaCrear = Url.Action("AlmacenCaratulaVentaCrear", _textoCaratulaVenta);
            return View();
        }

        [HttpPost]
        public string AlmacenCaratulaVentaCrear(EnAlmacenCaratulaVenta parametroCaratulaVenta)
        {
            var respuesta = "error";
            if (parametroCaratulaVenta == null)
                return respuesta;
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            parametroCaratulaVenta.UsuarioId = Usuario.UsuarioId;
            bool B_CrearRegistro = SvAppAlmacenCaratulaVentaCrear.Crear(parametroCaratulaVenta);
            if (B_CrearRegistro)
            {
                EnAlmacenCaratula Caratula =
                    SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratulaVenta.AlmacenCaratulaId);
                EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                    .Permiso
                    .PermisosAlmacenCaratula
                    .Where(
                        x =>
                        x.AlmacenId == Caratula.AlmacenId)
                    .Single();
                string controlador = PermisoCaratula.B_CaratulaEfectivoCrear
                    ? _textoCaratulaEfectivo
                    : _textoCaratula;
                respuesta = Url.Action("Index", controlador, Caratula);
            }
            return respuesta;
        }

        [HttpGet]
        public ActionResult Actualizar(int almacenCaratulaId)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(almacenCaratulaId);
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            EnAlmacenCaratulaVenta AlmacenCaratulaVenta =
                SvAppAlmacenCaratulaVentaConsultar.Consultar(Caratula.AlmacenCaratulaId);
            ICollection<EnAlmacenCategoriaVenta> AlmacenCategoriasVenta =
                SvAppAlmacenCaratulaVentaConsultarCategoriaVentaDelDia.Consultar(Caratula);

            ViewBag.Usuario = Usuario;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaVenta = AlmacenCaratulaVenta;
            ViewBag.AlmacenCategoriasVenta = AlmacenCategoriasVenta;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            ViewBag.UrlAlmacenCaratulaVentaActualizar = Url.Action("AlmacenCaratulaVentaActualizar", _textoCaratulaVenta);
            ViewBag.UrlAlmacenCaratulaVentaGrabar = Url.Action("AlmacenCaratulaVentaGrabar", _textoCaratulaVenta);
            return View();
        }

        [HttpPost]
        public string AlmacenCaratulaVentaActualizar(EnAlmacenCaratulaVenta parametroCaratulaVenta)
        {
            var respuesta = "error";
            if (parametroCaratulaVenta == null)
                return respuesta;
            bool B_ActualizarRegistro = SvAppAlmacenCaratulaVentaActualizar.Actualizar(parametroCaratulaVenta);
            if (B_ActualizarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpPost]
        public string AlmacenCaratulaVentaGrabar(EnAlmacenCaratulaVenta parametroCaratulaVenta)
        {
            var respuesta = "error";
            if (parametroCaratulaVenta == null)
                return respuesta;
            bool B_GrabarRegistro = SvAppAlmacenCaratulaVentaGrabar.Grabar(parametroCaratulaVenta);
            if (B_GrabarRegistro)
                respuesta = Url.Action("Index", _textoCaratula);
            return respuesta;
        }

        [HttpGet]
        public ActionResult Consultar(int almacenCaratulaId)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(almacenCaratulaId);
            EnUsuarioPermisoAlmacenCaratula PermisoCaratula = Usuario
                .Permiso
                .PermisosAlmacenCaratula
                .Where(
                    x =>
                    x.AlmacenId == Caratula.AlmacenId)
                .Single();
            EnAlmacenCaratulaVenta AlmacenCaratulaVenta =
                SvAppAlmacenCaratulaVentaConsultar.Consultar(Caratula.AlmacenCaratulaId);
            ICollection<EnAlmacenCategoriaVenta> AlmacenCategoriasVenta =
                SvAppAlmacenCaratulaVentaConsultarCategoriaVentaDelDia.Consultar(Caratula);

            ViewBag.Usuario = Usuario;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.Caratula = Caratula;
            ViewBag.PermisoCaratula = PermisoCaratula;
            ViewBag.AlmacenCaratulaVenta = AlmacenCaratulaVenta;
            ViewBag.AlmacenCategoriasVenta = AlmacenCategoriasVenta;
            ViewBag.UrlIndexAlmacenCaratula = Url.Action("Index", _textoCaratula);
            return View();
        }
    }
}