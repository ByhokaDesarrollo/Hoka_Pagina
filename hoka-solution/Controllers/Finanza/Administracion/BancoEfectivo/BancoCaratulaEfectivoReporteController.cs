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
    public class BancoCaratulaEfectivoReporteController : Controller
    {
        private readonly string _textoCaratula = RtBancoCaratulaRutas.TextoBancoCaratula;
        private readonly string _textoCaratulaEfectivo = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;
        private readonly string _textoCaratulaReporte = RtBancoCaratulaRutas.TextoBancoCaratulaReporte;

        [HttpGet]
        public ActionResult Index()
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            ViewBag.Usuario = Usuario;

            DateTime fechaActual = SvConsultarFechaActual.Consultar(0);
            var validarAcceso = SvValidarAccesoPorFecha.Validar(fechaActual);
            if (!validarAcceso)
                return View();

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
                .Where(x => x.CaratulaEfectivo.FechaCapturaBanco != null)
                .Select( x => new
                {
                    CaratulaEfectivoFechaCaptura =
                        x.CaratulaEfectivo.FechaCaptura?.ToString("yyyyMMdd") ?? "",
                    CaratulaEfectivoFechaCapturaBanco =
                        x.CaratulaEfectivo.FechaCapturaBanco?.ToString("yyyyMMdd") ?? "",
                    CaratulaEfectivoFechaActualizacionBanco =
                        x.CaratulaEfectivo.FechaActualizacionBanco?.ToString("yyyyMMdd") ?? "",
                    AlmacenEstado = x.Almacen.Estado,
                    AlmacenCiudad = x.Almacen.Ciudad,
                    AlmacenSucursalPrefijo = x.Almacen.SucursalPrefijo,
                    Folio = x.Folio,
                    ImporteTotal = x.CaratulaEfectivo.ImporteTotal,
                    ImporteTotalBanco = x.CaratulaEfectivo.ImporteTotalBanco
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