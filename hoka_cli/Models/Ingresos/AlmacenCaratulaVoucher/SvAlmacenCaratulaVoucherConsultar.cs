using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class SvAlmacenCaratulaVoucherConsultar
    {
        private RpAlmacenCaratulaVoucher _rpAlmacenCaratulaVoucher;
        private EsAlmacenCaratulaVoucher _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaVoucher> _entidades;

        public SvAlmacenCaratulaVoucherConsultar(
            RpAlmacenCaratulaVoucher repositorio,
            EsAlmacenCaratulaVoucher esAlmacenCaratulaVoucher)
        {
            _rpAlmacenCaratulaVoucher = repositorio;
            _estructuraEntidad = esAlmacenCaratulaVoucher;
            _entidades = new HashSet<EnAlmacenCaratulaVoucher>();
        }

        public ICollection<EnAlmacenCaratulaVoucher> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (_entidades?.Count > 0)
                if (consultarPropiedades)
                    ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad?.Consulta ?? null;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaVoucher FiltroEntidad = _estructuraEntidad?.CaratulaVoucher ?? null;
            IEnumerable<EnAlmacenCaratulaVoucher> Query = _rpAlmacenCaratulaVoucher._dbSet;
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.FechaInicio != null)
                    Query = Query.Where(q => q.FechaCaptura >= FiltroConsulta.FechaInicio);
                if (FiltroConsulta.FechaFin != null)
                    Query = Query.Where(q => q.FechaCaptura >= FiltroConsulta.FechaFin);
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaVoucherId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaVoucherId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVoucherId == FiltroEntidad.AlmacenCaratulaVoucherId);
                if (FiltroEntidad.AlmacenCaratulaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaId == FiltroEntidad.AlmacenCaratulaId);
                if (FiltroEntidad.UsuarioId > 0)
                    Query = Query.Where(q => q.UsuarioId == FiltroEntidad.UsuarioId);
                if (FiltroEntidad.FechaCaptura != null)
                    Query = Query.Where(q => q.FechaCaptura?.ToString("yyyyMMdd") == FiltroEntidad.FechaCapturaFormatoFecha);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaVoucherId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarVoucher)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                if (_estructuraEntidad.B_ConsultarVoucher)
                    entidad.Vouchers = ConsultarVouchers(entidad.AlmacenCaratulaVoucherId);
            }
        }

        private ICollection<EnAlmacenCaratulaVoucherVoucher> ConsultarVouchers(int AlmacenCaratulaVoucherId)
        {
            RpAlmacenCaratulaVoucherVoucher rpAlmacenCaratulaVoucherVoucher =
                SvAlmacenCaratulaVoucherVoucherIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaVoucherVoucher esAlmacenCaratulaVoucherVoucher =
                new EsAlmacenCaratulaVoucherVoucher()
            {
                Voucher = new EnAlmacenCaratulaVoucherVoucher()
                {
                    AlmacenCaratulaVoucherId = AlmacenCaratulaVoucherId
                },
                B_ConsultarVoucher = true,
                B_ConsultarVoucherRecibo = true
            };
            SvAlmacenCaratulaVoucherVoucherConsultar servicio =
                new SvAlmacenCaratulaVoucherVoucherConsultar(
                    rpAlmacenCaratulaVoucherVoucher,
                    esAlmacenCaratulaVoucherVoucher
                );
            ICollection<EnAlmacenCaratulaVoucherVoucher> AlmacenCaratulaVoucherVouchers = servicio.Consultar();
            return AlmacenCaratulaVoucherVouchers;
        }
    }
}
