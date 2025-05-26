using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Voucher;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher
{
    public class SvAlmacenCaratulaVoucherVoucherConsultar
    {
        private RpAlmacenCaratulaVoucherVoucher _rpAlmacenCaratulaVoucherVoucher;
        private EsAlmacenCaratulaVoucherVoucher _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaVoucherVoucher> _entidades;

        public SvAlmacenCaratulaVoucherVoucherConsultar(
            RpAlmacenCaratulaVoucherVoucher repositorio,
            EsAlmacenCaratulaVoucherVoucher esAlmacenCaratulaVoucherVoucher)
        {
            _rpAlmacenCaratulaVoucherVoucher = repositorio;
            _estructuraEntidad = esAlmacenCaratulaVoucherVoucher;
            _entidades = new HashSet<EnAlmacenCaratulaVoucherVoucher>();
        }

        public ICollection<EnAlmacenCaratulaVoucherVoucher> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad?.Consulta ?? null;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaVoucherVoucher FiltroEntidad = _estructuraEntidad?.Voucher ?? null;
            IEnumerable<EnAlmacenCaratulaVoucherVoucher> Query = _rpAlmacenCaratulaVoucherVoucher._dbSet;
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaVoucherVoucherId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaVoucherVoucherId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVoucherVoucherId == FiltroEntidad.AlmacenCaratulaVoucherVoucherId);
                if (FiltroEntidad.AlmacenCaratulaVoucherId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVoucherId == FiltroEntidad.AlmacenCaratulaVoucherId);
                if (FiltroEntidad.VoucherId > 0)
                    Query = Query.Where(q => q.VoucherId == FiltroEntidad.VoucherId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaVoucherVoucherId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarVoucher ||
                _estructuraEntidad.B_ConsultarVoucherRecibo)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.Voucher = _estructuraEntidad.B_ConsultarVoucher
                    ? ConsultarVoucher(entidad.VoucherId)
                    : null;
                entidad.Recibos = _estructuraEntidad.B_ConsultarVoucherRecibo
                    ? ConsultarRecibos(entidad.AlmacenCaratulaVoucherVoucherId)
                    : null;
            }
        }

        private EnVoucher ConsultarVoucher(int VoucherId)
        {
            RpVoucher rpVoucher = SvVoucherIniciarRepositorio.IniciarRepositorio();
            EsVoucher esVoucher = new EsVoucher()
            {
                Voucher = new EnVoucher()
                {
                    VoucherId = VoucherId
                },
                B_ConsultarMoneda = true
            };
            SvVoucherConsultar servicio = new SvVoucherConsultar(rpVoucher, esVoucher);
            EnVoucher Voucher = servicio.Consultar().Single();
            return Voucher;
        }

        private ICollection<EnAlmacenCaratulaVoucherRecibo> ConsultarRecibos(
            int almacenCaratulaVoucherVoucherId)
        {
            RpAlmacenCaratulaVoucherRecibo rpAlmacenCaratulaVoucherRecibo =
                SvAlmacenCaratulaVoucherReciboIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaVoucherRecibo esAlmacenCaratulaVoucherRecibo =
                new EsAlmacenCaratulaVoucherRecibo()
                {
                    ConsultarArchivo = true,
                    Recibo = new EnAlmacenCaratulaVoucherRecibo()
                    {
                        AlmacenCaratulaVoucherVoucherId = almacenCaratulaVoucherVoucherId
                    }
                };
            SvAlmacenCaratulaVoucherReciboConsultar servicio =
                new SvAlmacenCaratulaVoucherReciboConsultar(rpAlmacenCaratulaVoucherRecibo, esAlmacenCaratulaVoucherRecibo);
            ICollection<EnAlmacenCaratulaVoucherRecibo> Recibos = servicio.Consultar().ToHashSet();
            return Recibos;
        }
    }
}
