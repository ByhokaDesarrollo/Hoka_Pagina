using hoka.Hoka.Models.Banco.Caratula.Servicios;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using HokaCli_EsConsulta = hoka_cli.Struct.EsConsulta;
using HokaCli_EsAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EsAlmacenCaratula;
using HokaCli_EnAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;
using hoka.Hoka.Models.Banco.Caratula.Rutas;

namespace hoka.Controllers.Finanza.Administracion.BancoEfectivo
{
    public class BancoCaratulaController : Controller
    {
        private readonly string _textoCaratulaEfectivo = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;
        private readonly string _textoCaratulaReporte = RtBancoCaratulaRutas.TextoBancoCaratulaEfectivo;

        // GET: AlmacenCaratula
        [HttpGet]
        public ActionResult Index()
        {
            EnUsuario Usuario = (EnUsuario)Session["SsUsuario"];
            DateTime fechaActual = SvConsultarFechaActual.Consultar(-18);

            HokaCli_EsAlmacenCaratula esAlmacenCaratula = new HokaCli_EsAlmacenCaratula()
            {
                Consulta = new HokaCli_EsConsulta()
                {
                    UsuarioId = Usuario.UsuarioId,
                    FechaInicio = fechaActual,
                    FechaFin = fechaActual,
                },
                Caratula = new HokaCli_EnAlmacenCaratula()
                {
                    FechaRegistro = fechaActual
                },
                B_ConsultarAlmacen = true,
                B_ConsultarUsuario = true,
                B_ConsultarEstatus = true
            };

            ICollection<EnAlmacenCaratula> Caratulas =
                SvAppBancoCaratulaConsultarPorUsuario.Consultar(esAlmacenCaratula);
            //Caratulas = SvAlmacenCaratulaFiltrarPorUsuario.Filtrar(
            //    Usuario.Permiso.PermisosAlmacenCaratula,
            //    Caratulas);
            ViewBag.Usuario = Usuario;
            ViewBag.FechaActual = fechaActual;
            ViewBag.TextoCaratulaEfectivo = _textoCaratulaEfectivo;
            ViewBag.TextoCaratulaReporte = _textoCaratulaReporte;
            return View(Caratulas);
        }
    }
}