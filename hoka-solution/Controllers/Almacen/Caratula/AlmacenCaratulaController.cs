using hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios;
using hoka.Hoka.Models.Almacen.Caratula.Rutas;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaController : Controller
    {
        private readonly string _textoCaratulaVenta = RtAlmacenCaratulaRutas.TextoCaratulaVenta;
        private readonly string _textoCaratulaEfectivo = RtAlmacenCaratulaRutas.TextoCaratulaEfectivo;
        private readonly string _textoCaratulaVoucher = RtAlmacenCaratulaRutas.TextoCaratulaVoucher;
        private readonly string _textoCaratulaReporte = RtAlmacenCaratulaRutas.TextoCaratulaReporte;

        // GET: AlmacenCaratula
        [HttpGet]
        public ActionResult Index()
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            // Default
            //DateTime fechaActual = SvConsultarFechaActual.Consultar(-1);
            DateTime fechaActual = SvConsultarFechaActual.Consultar(-13);
            ICollection<EnAlmacenCaratula> Caratulas =
                SvAppAlmacenCaratulaConsultarCaratulasDelDia.Consultar(fechaActual);
            Caratulas = SvAlmacenCaratulaFiltrarPorUsuario.Filtrar(
                Usuario.Permiso.PermisosAlmacenCaratula,
                Caratulas);
            ViewBag.Usuario = Usuario;
            ViewBag.FechaActual = fechaActual;
            ViewBag.TextoCaratulaVenta = _textoCaratulaVenta;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.TextoCaratulaVoucher = _textoCaratulaVoucher;
            ViewBag.TextoCaratulaReporte = _textoCaratulaReporte;
            return View(Caratulas);
        }
    }
}