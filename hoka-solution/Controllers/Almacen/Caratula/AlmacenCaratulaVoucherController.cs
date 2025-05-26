using hoka.AppServicios.Ingresos.AlmacenCaratulaVoucher;
using hoka.AppServicios.Ingresos.AlmacenCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Compuadmo.Voucher;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using hoka.AppServicios.Ingresos.AlmacenCaratula;
using System.Configuration;
using System.Net.Mime;
using System.IO;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaVoucherController : Controller
    {
        private readonly string _textoCaratula = RtAlmacenCaratulaRutas.TextoCaratula;
        private readonly string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        private readonly string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        private readonly string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;
        private readonly string _rutaBase = ConfigurationManager.AppSettings["RutaAlmacenCaratulaVoucherRecibo"];

        // GET: AlmacenCaratulaVoucher
        [HttpGet]
        public ActionResult Index(EnAlmacenCaratula parametroCaratula)
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
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
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
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

        [HttpGet]
        public ActionResult DescargarArchivo(int id)
        {
            string archivoNombreEnServidor = Directory.GetFiles(_rutaBase, $"{id}_*").FirstOrDefault();

            if (string.IsNullOrEmpty(archivoNombreEnServidor))
            {
                return HttpNotFound("Archivo no encontrado.");
            }

            string archivoRutaCompleta = Path.Combine(_rutaBase, archivoNombreEnServidor);
            string nombreParaDescarga = Path.GetFileName(archivoNombreEnServidor).Substring(id.ToString().Length + 1);

            // Determinar el tipo de contenido (MIME type)
            string contentType;
            string extension = Path.GetExtension(archivoNombreEnServidor).ToLowerInvariant();
            switch (extension)
            {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                case ".jpg":
                case ".jpeg":
                    contentType = "image/jpeg";
                    break;
                case ".png":
                    contentType = "image/png";
                    break;
                case ".gif":
                    contentType = "image/gif";
                    break;
                default:
                    contentType = "application/octet-stream"; // Tipo genérico para otros archivos
                    break;
            }

            return File(archivoRutaCompleta, contentType, nombreParaDescarga);
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
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;
            EnAlmacenCaratula Caratula =
                SvAppAlmacenCaratulaConsultar.Consultar(parametroCaratula.AlmacenCaratulaId);
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