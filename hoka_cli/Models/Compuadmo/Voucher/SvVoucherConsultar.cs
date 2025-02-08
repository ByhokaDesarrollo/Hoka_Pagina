using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Moneda;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    public class SvVoucherConsultar
    {
        private RpVoucher _rpVoucher;
        private EsVoucher _estructuraEntidad;
        private ICollection<EnVoucher> _entidades;

        public SvVoucherConsultar(RpVoucher repositorio, EsVoucher esVoucher)
        {
            _rpVoucher = repositorio;
            _estructuraEntidad = esVoucher;
            _entidades = new HashSet<EnVoucher>();
        }

        public ICollection<EnVoucher> Consultar()
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
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion;
            EnVoucher FiltroEntidad = _estructuraEntidad?.Voucher;
            IEnumerable<EnVoucher> Query = _rpVoucher._dbSet;
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.VoucherId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.VoucherId > 0)
                    Query = Query.Where(q => q.VoucherId == FiltroEntidad.VoucherId);
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Prefijo))
                    Query = Query.Where(q => q.Prefijo.ToUpper().Contains(FiltroEntidad.Prefijo.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.VoucherId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarMoneda)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.Moneda = _estructuraEntidad.B_ConsultarMoneda
                    ? ConsultarMoneda(entidad.MonedaId)
                    : null;
            }
        }

        private EnMoneda ConsultarMoneda(int MonedaId)
        {
            RpMoneda rpMoneda = SvMonedaIniciarRepositorio.IniciarRepositorio();
            EsMoneda esMoneda = new EsMoneda()
            {
                Moneda = new EnMoneda()
                {
                    MonedaId = MonedaId
                },
                B_ConsultarMonedaCero = true
            };
            SvMonedaConsultar servicio = new SvMonedaConsultar(rpMoneda, esMoneda);
            EnMoneda Moneda = servicio.Consultar().Single();
            return Moneda;
        }
    }
}
