using hoka.AppServicios.Compuadmo.Almacen;
using hoka.AppServicios.Ingresos.AlmacenCaratula;
using hoka.AppServicios.Ingresos.AlmacenCaratulaRutas;
using hoka.HokaCli.Models.Compuadmo.Almacen;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Utileria.BaseDatos;
using hoka_cli.Models.Utileria.Validaciones;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaController : Controller
    {
        private readonly string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        private readonly string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        private readonly string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;
        private readonly string _urlAlmacenCaratulaGasto = ConfigurationManager.AppSettings["UrlAlmacenCaratulaGasto"];

        #region Respaldo Index
        // GET: AlmacenCaratula
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
        //    // Default
        //    //DateTime fechaActual = SvConsultarFechaActual.Consultar(-1);
        //    DateTime fechaActual = SvConsultarFechaActual.Consultar(-2);
        //    ICollection<EnAlmacenCaratula> Caratulas =
        //        SvAppAlmacenCaratulaConsultarCaratulasDelDia.Consultar(fechaActual);
        //    Caratulas = SvAppAlmacenCaratulaFiltrarPorUsuario.Filtrar(
        //        Usuario.Permiso.PermisosAlmacenCaratula,
        //        Caratulas);
        //    ViewBag.Usuario = Usuario;
        //    ViewBag.FechaActual = fechaActual;
        //    ViewBag.TextoCaratulaVenta = _textoCaratulaVenta;
        //    ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
        //    ViewBag.TextoCaratulaVoucher = _textoCaratulaVoucher;
        //    ViewBag.TextoCaratulaReporte = _textoCaratulaReporte;
        //    return View(Caratulas);
        //}
        #endregion

        [HttpGet]
        public ActionResult Index()
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            DateTime fechaActual = SvConsultarFechaActual.Consultar(0);
            ViewBag.Usuario = Usuario;
            ViewBag.FechaActual = fechaActual;
            ICollection<EnAlmacen> Almacenes = SvAppAlmacenConsultar.Consultar();
            ViewBag.Almacenes = Almacenes;
            
            ViewBag.TextoCaratulaVenta = _textoCaratulaVenta;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.TextoCaratulaVoucher = _textoCaratulaVoucher;
            ViewBag.TextoCaratulaReporte = _textoCaratulaReporte;
            return View();
        }

        [HttpPost]
        public ActionResult ObtenerAlmacenCaratula(
            DateTime fechaInicio,
            DateTime fechaFin,
            int almacenId,
            int draw, int start, int length)
        {
            fechaInicio = fechaInicio.AddHours(0).AddMinutes(0).AddSeconds(0);
            fechaFin = fechaFin.AddHours(23).AddMinutes(59).AddSeconds(59);
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ICollection<EnAlmacenCaratula> Caratulas =
                SvAppAlmacenCaratulaConsultarCaratulasDelDia.Consultar(
                    fechaInicio: fechaInicio,
                    fechaFin: fechaFin,
                    almacenId: almacenId);
            Caratulas = SvAppAlmacenCaratulaFiltrarPorUsuario.Filtrar(
                Usuario.Permiso.PermisosAlmacenCaratula,
                Caratulas);

            var recordsTotal = Caratulas.Count;

            var data = Caratulas
                .OrderBy(x => x.FechaRegistro)
                .Skip(start)
                .Take(length)
                .Select(x => new
                {
                    FechaRegistro = x.FechaRegistro?.ToString("yyyyMMdd") ?? "",
                    AlmacenEstado = x.Almacen.Estado,
                    AlmacenCiudad = x.Almacen.Ciudad,
                    AlmacenSucursalPrefijo = x.Almacen.SucursalPrefijo,
                    Folio = x.Folio,
                    EstatusCaratula = x.Estatus.Nombre,
                    UsuarioNombre = x.Usuario?.Nombre ?? "",
                    EstatusCaratulaVenta = x.EstatusCaratulaVenta,
                    EstatusCaratulaEfectivo = x.EstatusCaratulaEfectivo,
                    EstatusCaratulaVoucher = x.EstatusCaratulaVoucher,
                    B_ConsultarCaratula = x.B_ConsultarCaratula,
                    AlmacenId = x.AlmacenId,
                    AlmacenCaratulaId = x.AlmacenCaratulaId,
                    CaratulaGastos = _urlAlmacenCaratulaGasto
                })
                .ToList();

            return Json(
                new
                {
                    draw,
                    recordsTotal,
                    recordsFiltered = recordsTotal,
                    data
                },
                JsonRequestBehavior.AllowGet
            );
        }
    }
}