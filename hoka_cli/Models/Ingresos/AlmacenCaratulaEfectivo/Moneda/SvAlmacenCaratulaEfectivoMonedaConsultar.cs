using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda
{
    public class SvAlmacenCaratulaEfectivoMonedaConsultar
    {
        private RpAlmacenCaratulaEfectivoMoneda _rpAlmacenCaratulaEfectivoMoneda;
        private EsAlmacenCaratulaEfectivoMoneda _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaEfectivoMoneda> _entidades;

        public SvAlmacenCaratulaEfectivoMonedaConsultar(
            RpAlmacenCaratulaEfectivoMoneda repositorio,
            EsAlmacenCaratulaEfectivoMoneda esAlmacenCaratulaEfectivoMoneda)
        {
            _rpAlmacenCaratulaEfectivoMoneda = repositorio;
            _estructuraEntidad = esAlmacenCaratulaEfectivoMoneda;
            _entidades = new HashSet<EnAlmacenCaratulaEfectivoMoneda>();
        }

        public ICollection<EnAlmacenCaratulaEfectivoMoneda> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad.Consulta;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaEfectivoMoneda FiltroEntidad = _estructuraEntidad.Moneda;
            IEnumerable<EnAlmacenCaratulaEfectivoMoneda> Query = _rpAlmacenCaratulaEfectivoMoneda._dbSet;
            if (FiltroConsulta != null)
            {
                // NA
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaEfectivoMonedaId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaEfectivoMonedaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEfectivoMonedaId == FiltroEntidad.AlmacenCaratulaEfectivoMonedaId);
                if (FiltroEntidad.AlmacenCaratulaEfectivoId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEfectivoId == FiltroEntidad.AlmacenCaratulaEfectivoId);
                if (FiltroEntidad.MonedaId > 0)
                    Query = Query.Where(q => q.MonedaId == FiltroEntidad.MonedaId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaEfectivoMonedaId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarMoneda ||
                _estructuraEntidad.B_ConsultarMonedaDenominacion)
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
                entidad.Denominaciones = _estructuraEntidad.B_ConsultarMonedaDenominacion
                    ? ConsultarMonedaDenominaciones(entidad.AlmacenCaratulaEfectivoMonedaId)
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

        private ICollection<EnAlmacenCaratulaEfectivoDenominacion> ConsultarMonedaDenominaciones(
            int almacenCaratulaEfectivoMonedaId)
        {
            RpAlmacenCaratulaEfectivoDenominacion rpAlmacenCaratulaEfectivoDenominacion =
                SvAlmacenCaratulaEfectivoDenominacionIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaEfectivoDenominacion esAlmacenCaratulaEfectivoDenominacion =
                new EsAlmacenCaratulaEfectivoDenominacion()
            {
                Denominacion = new EnAlmacenCaratulaEfectivoDenominacion()
                {
                    AlmacenCaratulaEfectivoMonedaId = almacenCaratulaEfectivoMonedaId
                },
                B_ConsultarMonedaDenominacion = true
            };
            SvAlmacenCaratulaEfectivoDenominacionConsultar servicio =
                new SvAlmacenCaratulaEfectivoDenominacionConsultar(rpAlmacenCaratulaEfectivoDenominacion, esAlmacenCaratulaEfectivoDenominacion);
            ICollection<EnAlmacenCaratulaEfectivoDenominacion> Denominaciones = servicio.Consultar().ToHashSet();
            return Denominaciones;
        }
    }
}
