using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HokaCli_EsConsulta = hoka_cli.Struct.EsConsulta;
using HokaCli_EsAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EsAlmacenCaratula;
using HokaCli_EnAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;
using hoka.AppServicios.Ingresos.BancoCaratulaRutas;
using hoka.AppServicios.Ingresos.BancoCaratula;
using hoka.AppServicios.Compuadmo.Almacen;
using hoka.HokaCli.Models.Compuadmo.Almacen;
using System.Linq;
using hoka_cli.Models.Utileria.Validaciones;

namespace hoka.Controllers.Finanza.Administracion.BancoEfectivo
{
    public class BancoCaratulaController : Controller
    {
        private readonly string _textoCaratulaEfectivo = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;
        private readonly string _textoCaratulaReporte = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;

        #region Respaldo Index
        // GET: AlmacenCaratula
        //[HttpGet]
        //public ActionResult Index()
        //{
        //    EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
        //    DateTime fechaActual = SvConsultarFechaActual.Consultar(-18);

        //    HokaCli_EsAlmacenCaratula esAlmacenCaratula = new HokaCli_EsAlmacenCaratula()
        //    {
        //        Consulta = new HokaCli_EsConsulta()
        //        {
        //            UsuarioId = Usuario.UsuarioId,
        //            FechaInicio = fechaActual,
        //            FechaFin = fechaActual.AddDays(1),
        //        },
        //        Caratula = new HokaCli_EnAlmacenCaratula()
        //        {
        //            FechaRegistro = fechaActual
        //        },
        //        B_ConsultarAlmacen = true,
        //        B_ConsultarUsuario = true,
        //        B_ConsultarEstatus = true
        //    };

        //    ICollection<EnAlmacenCaratula> Caratulas =
        //        SvAppBancoCaratulaConsultarPorUsuario.Consultar(esAlmacenCaratula);
        //    ViewBag.Usuario = Usuario;
        //    ViewBag.FechaActual = fechaActual;
        //    ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
        //    ViewBag.TextoCaratulaReporte = _textoCaratulaReporte;
        //    return View(Caratulas);
        //}
        #endregion

        [HttpGet]
        public ActionResult Index()
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;

            DateTime fechaActual = SvConsultarFechaActual.Consultar(0);
            var validarAcceso = SvValidarAccesoPorFecha.Validar(fechaActual);
            ViewBag.FechaActual = fechaActual;
            ICollection<EnAlmacen> Almacenes = SvAppAlmacenConsultar.Consultar();
            ViewBag.Almacenes = Almacenes;

            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
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


            HokaCli_EsAlmacenCaratula esAlmacenCaratula = new HokaCli_EsAlmacenCaratula()
            {
                Consulta = new HokaCli_EsConsulta()
                {
                    UsuarioId = Usuario.UsuarioId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin,
                },
                Caratula = new HokaCli_EnAlmacenCaratula()
                {
                    AlmacenId = almacenId
                },
                B_ConsultarAlmacen = true,
                B_ConsultarUsuario = true,
                B_ConsultarEstatus = true,
                B_ConsultarCaratulaEfectivo = true
            };
            ICollection<EnAlmacenCaratula> Caratulas =
                SvAppBancoCaratulaConsultarPorUsuario.Consultar(esAlmacenCaratula);

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
                    CaratulaEfectivoFechaCapturaBanco = x.CaratulaEfectivo.FechaCapturaBanco,
                    CaratulaEfectivoFechaActualizacionBanco = x.CaratulaEfectivo.FechaActualizacionBanco,
                    EstatusCaratulaVoucher = x.EstatusCaratulaVoucher,
                    B_ConsultarCaratula = x.B_ConsultarCaratula,
                    AlmacenId = x.AlmacenId,
                    AlmacenCaratulaId = x.AlmacenCaratulaId
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