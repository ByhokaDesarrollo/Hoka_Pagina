using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Almacen;
using hoka_cli.Models.Compuadmo.Usuario;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Ingresos.BancoCaratula
{
    public class SvBancoCaratulaConsultar
    {
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private EsAlmacenCaratula _estructuraEntidad;
        private ICollection<EnAlmacenCaratula> _entidades;

        public SvBancoCaratulaConsultar(
            RpAlmacenCaratula repositorio,
            EsAlmacenCaratula esAlmacenCaratula)
        {
            _rpAlmacenCaratula = repositorio;
            _estructuraEntidad = esAlmacenCaratula;
            _entidades = new HashSet<EnAlmacenCaratula>();
        }

        public ICollection<EnAlmacenCaratula> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad?.Consulta;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratula FiltroEntidad = _estructuraEntidad?.Caratula;
            int UsuarioId = FiltroConsulta.UsuarioId;
            int AlmacenId = FiltroEntidad.AlmacenId;
	        string FechaIni = !string.IsNullOrEmpty(_estructuraEntidad.Consulta.FechaInicioFormatoFecha)
                ? $"'{_estructuraEntidad.Consulta.FechaInicioFormatoFecha}'"
                : "NULL";
            string FechaFin = !string.IsNullOrEmpty(_estructuraEntidad.Consulta.FechaFinFormatoFecha)
                ? $"'{_estructuraEntidad.Consulta.FechaFinFormatoFecha}'"
                : "NULL";
            string query = $"EXECUTE SP_AlmacenBancoConsultar " +
                $"{UsuarioId}, " +
                $"{AlmacenId}, " +
                $"{FechaIni}, " +
                $"{FechaFin}";
            IEnumerable<EnAlmacenCaratula> Query = _rpAlmacenCaratula.FindByQuery(query);
            //if (FiltroConsulta != null)
            //{
            //    if (FiltroConsulta.FechaInicio != null &&
            //        FiltroConsulta.FechaFin != null)
            //        Query = Query.Where(
            //            q =>
            //            q.FechaRegistro >= FiltroConsulta.FechaInicio &&
            //            q.FechaRegistro <= FiltroConsulta.FechaFin);
            //    if (FiltroConsulta.FechaInicio != null &&
            //        FiltroConsulta.FechaFin == null)
            //        Query = Query.Where(
            //            q =>
            //            q.FechaRegistro >= FiltroConsulta.FechaInicio &&
            //            q.FechaRegistro >= FiltroConsulta.FechaInicio);
            //}
            //if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            //{
            //    if (FiltroPaginacion.UltimoId > 0)
            //        Query = Query.Where(x => x.AlmacenCaratulaId > FiltroPaginacion.UltimoId);
            //    if (FiltroPaginacion.NumeroRegistros > 0)
            //        Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            //}
            //if (FiltroEntidad != null)
            //{
            //    if (FiltroEntidad.AlmacenCaratulaId > 0)
            //        Query = Query.Where(q => q.AlmacenCaratulaId == FiltroEntidad.AlmacenCaratulaId);
            //    if (FiltroEntidad.AlmacenId > 0)
            //        Query = Query.Where(q => q.AlmacenId == FiltroEntidad.AlmacenId);
            //    if (FiltroEntidad.UsuarioId > 0)
            //        Query = Query.Where(q => q.UsuarioId == FiltroEntidad.UsuarioId);
            //    if (FiltroEntidad.AlmacenCaratulaEstatusId > 0)
            //        Query = Query.Where(q => q.AlmacenCaratulaEstatusId == FiltroEntidad.AlmacenCaratulaEstatusId);
            //    if (!string.IsNullOrEmpty(FiltroEntidad.Folio))
            //        Query = Query.Where(q => q.Folio.ToUpper().Contains(FiltroEntidad.Folio.ToUpper()));
            //    if (FiltroEntidad.FechaRegistro != null)
            //        Query = Query.Where(q => q.FechaRegistro?.ToString("yyyyMMdd") == FiltroEntidad.FechaRegistroFormatoFecha);
            //}
            //if (FiltroConsulta != null)
            //{
            //    if (FiltroConsulta.NumeroRegistros > 0)
            //        Query = Query.Take(FiltroConsulta.NumeroRegistros);
            //    if (FiltroConsulta.B_TablaRegistroDescendente)
            //        Query = Query.OrderByDescending(q => q.AlmacenCaratulaId);
            //}
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarAlmacen ||
                _estructuraEntidad.B_ConsultarUsuario ||
                _estructuraEntidad.B_ConsultarEstatus ||
                _estructuraEntidad.B_ConsultarCaratulaVenta ||
                _estructuraEntidad.B_ConsultarCaratulaEfectivo ||
                _estructuraEntidad.B_ConsultarCaratulaVoucher ||
                _estructuraEntidad.B_ConsultarCaratula)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                if (_estructuraEntidad.B_ConsultarAlmacen &&
                    entidad.AlmacenId > 0)
                    entidad.Almacen = ConsultarAlmacen(entidad.AlmacenId);
                if (_estructuraEntidad.B_ConsultarUsuario &&
                    entidad.UsuarioId > 0)
                    entidad.Usuario = ConsultarUsuario(entidad.UsuarioId);
                if (_estructuraEntidad.B_ConsultarEstatus &&
                    entidad.AlmacenCaratulaEstatusId > 0)
                    entidad.Estatus = ConsultarEstatus(entidad.AlmacenCaratulaEstatusId);
                if (_estructuraEntidad.B_ConsultarCaratulaVenta)
                    entidad.CaratulaVenta = ConsultarCaratulaVenta(entidad.AlmacenCaratulaId);
                if (_estructuraEntidad.B_ConsultarCaratulaEfectivo ||
                    _estructuraEntidad.B_ConsultarCaratula)
                    entidad.CaratulaEfectivo = ConsultarCaratulaEfectivo(entidad.AlmacenCaratulaId);
                if (_estructuraEntidad.B_ConsultarCaratulaVoucher ||
                    _estructuraEntidad.B_ConsultarCaratula)
                    entidad.CaratulaVoucher = ConsultarCaratulaVoucher(entidad.AlmacenCaratulaId);
                if (_estructuraEntidad.B_ConsultarCaratula)
                    entidad.CaratulaReporte = ConsultarCaratulaReporte(
                        caratulaEfectivo: entidad.CaratulaEfectivo,
                        caratulaVoucher: entidad.CaratulaVoucher);
            }
        }

        private EnAlmacen ConsultarAlmacen(int almacenId)
        {
            RpAlmacen rpAlmacen = SvAlmacenIniciarRepositorio.IniciarRepositorio();
            EsAlmacen esAlmacen = new EsAlmacen()
            {
                Almacen = new EnAlmacen()
                {
                    AlmacenId = almacenId
                }
            };
            SvAlmacenConsultar servicio = new SvAlmacenConsultar(rpAlmacen, esAlmacen);
            EnAlmacen Almacen = servicio.Consultar().Single();
            return Almacen;
        }

        private EnUsuario ConsultarUsuario(int usuarioId)
        {
            RpUsuario rpUsuario = SvUsuarioIniciarRepositorio.IniciarRepositorio();
            EsUsuario esUsuario = new EsUsuario()
            {
                Usuario = new EnUsuario()
                {
                    UsuarioId = usuarioId
                }
            };
            SvUsuarioConsultar servicio = new SvUsuarioConsultar(rpUsuario, esUsuario);
            EnUsuario Usuario = servicio.Consultar().Single();
            return Usuario;
        }

        private EnAlmacenCaratulaEstatus ConsultarEstatus(
            int almacenCaratulaEstatusId)
        {
            RpAlmacenCaratulaEstatus rpAlmacenCaratulaEstatus =
                SvAlmacenCaratulaEstatusIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaEstatus esAlmacenCaratulaEstatus = new EsAlmacenCaratulaEstatus()
            {
                CaratulaEstatus = new EnAlmacenCaratulaEstatus()
                {
                    AlmacenCaratulaEstatusId = almacenCaratulaEstatusId
                }
            };
            SvAlmacenCaratulaEstatusConsultar servicio =
                new SvAlmacenCaratulaEstatusConsultar(
                    rpAlmacenCaratulaEstatus,
                    esAlmacenCaratulaEstatus);
            EnAlmacenCaratulaEstatus AlmacenCaratulaEstatus = servicio.Consultar().Single();
            return AlmacenCaratulaEstatus;
        }

        private EnAlmacenCaratulaVenta ConsultarCaratulaVenta(int almacenCaratulaId)
        {
            RpAlmacenCaratulaVenta rpAlmacenCaratulaVenta =
                SvAlmacenCaratulaVentaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaVenta esAlmacenCaratulaVenta = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = new EnAlmacenCaratulaVenta()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarCategoria = true
            };
            SvAlmacenCaratulaVentaConsultar servicio =
                new SvAlmacenCaratulaVentaConsultar(
                    rpAlmacenCaratulaVenta,
                    esAlmacenCaratulaVenta);
            var caratulaVenta = servicio.Consultar();
            EnAlmacenCaratulaVenta AlmacenCaratulaVenta = caratulaVenta?.Count > 0
                ? caratulaVenta.Single()
                : null;
            return AlmacenCaratulaVenta;
        }

        private EnAlmacenCaratulaEfectivo ConsultarCaratulaEfectivo(int almacenCaratulaId)
        {
            RpAlmacenCaratulaEfectivo rpAlmacenCaratulaEfectivo =
                SvAlmacenCaratulaEfectivoIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaEfectivo esAlmacenCaratulaEfectivo = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = new EnAlmacenCaratulaEfectivo()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarMoneda = true
            };
            SvAlmacenCaratulaEfectivoConsultar servicio =
                new SvAlmacenCaratulaEfectivoConsultar(
                    rpAlmacenCaratulaEfectivo,
                    esAlmacenCaratulaEfectivo);
            var caratulaEfectivo = servicio.Consultar();
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo = caratulaEfectivo?.Count > 0
                ? caratulaEfectivo.Single()
                : null;
            return AlmacenCaratulaEfectivo;
        }

        private EnAlmacenCaratulaVoucher ConsultarCaratulaVoucher(int almacenCaratulaId)
        {
            RpAlmacenCaratulaVoucher rpAlmacenCaratulaVoucher =
                SvAlmacenCaratulaVoucherIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaVoucher esAlmacenCaratulaVoucher = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = new EnAlmacenCaratulaVoucher()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarVoucher = true
            };
            SvAlmacenCaratulaVoucherConsultar servicio =
                new SvAlmacenCaratulaVoucherConsultar(
                    rpAlmacenCaratulaVoucher,
                    esAlmacenCaratulaVoucher);
            var caratulaVoucher = servicio.Consultar();
            EnAlmacenCaratulaVoucher AlmacenCaratulaVoucher = caratulaVoucher?.Count > 0
                ? caratulaVoucher.Single()
                : null;
            return AlmacenCaratulaVoucher;
        }

        private ICollection<EnAlmacenCaratulaReporte> ConsultarCaratulaReporte(
            EnAlmacenCaratulaEfectivo caratulaEfectivo,
            EnAlmacenCaratulaVoucher caratulaVoucher)
        {
            ICollection<EnAlmacenCaratulaReporte> AlmacenCaratulaReporte =
                new HashSet<EnAlmacenCaratulaReporte>();
            // Efectivo
            if (caratulaEfectivo?.Monedas?.Count > 0)
            {
                foreach (var moneda in caratulaEfectivo.Monedas)
                {
                    EnAlmacenCaratulaReporte registro = new EnAlmacenCaratulaReporte()
                    {
                        Concepto = moneda.Moneda.Nombre,
                        TipoCambio = moneda.TipoCambio,
                        Importe = moneda.ImporteTotal,
                        ImporteMXN = moneda.ImporteTotalMXN
                    };
                    AlmacenCaratulaReporte.Add(registro);
                }
            }
            // Credito
            if (caratulaVoucher?.Vouchers?.Count > 0)
            {
                foreach (var voucher in caratulaVoucher.Vouchers)
                {
                    EnAlmacenCaratulaReporte registro = new EnAlmacenCaratulaReporte()
                    {
                        Concepto = voucher.Voucher.Nombre,
                        TipoCambio = voucher.TipoCambio,
                        Importe = voucher.ImporteTotal,
                        ImporteMXN = voucher.ImporteTotalMXN
                    };
                    AlmacenCaratulaReporte.Add(registro);
                }
            }
            return AlmacenCaratulaReporte;
        }
    }
}
