using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Moneda.Denominacion;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion
{
    public class SvAlmacenCaratulaEfectivoDenominacionConsultar
    {
        private RpAlmacenCaratulaEfectivoDenominacion _rpAlmacenCaratulaEfectivoDenominacion;
        private EsAlmacenCaratulaEfectivoDenominacion _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaEfectivoDenominacion> _entidades;

        public SvAlmacenCaratulaEfectivoDenominacionConsultar(
            RpAlmacenCaratulaEfectivoDenominacion repositorio,
            EsAlmacenCaratulaEfectivoDenominacion esAlmacenCaratulaEfectivoDenominacion)
        {
            _rpAlmacenCaratulaEfectivoDenominacion = repositorio;
            _estructuraEntidad = esAlmacenCaratulaEfectivoDenominacion;
            _entidades = new HashSet<EnAlmacenCaratulaEfectivoDenominacion>();
        }

        public ICollection<EnAlmacenCaratulaEfectivoDenominacion> Consultar()
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
            EnAlmacenCaratulaEfectivoDenominacion FiltroEntidad = _estructuraEntidad.Denominacion;
            IEnumerable<EnAlmacenCaratulaEfectivoDenominacion> Query =
                _rpAlmacenCaratulaEfectivoDenominacion._dbSet;
            if (FiltroConsulta != null)
            {
                // NA
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaEfectivoDenominacionId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaEfectivoDenominacionId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEfectivoDenominacionId == FiltroEntidad.AlmacenCaratulaEfectivoDenominacionId);
                if (FiltroEntidad.AlmacenCaratulaEfectivoMonedaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEfectivoMonedaId == FiltroEntidad.AlmacenCaratulaEfectivoMonedaId);
                if (FiltroEntidad.MonedaDenominacionId > 0)
                    Query = Query.Where(q => q.MonedaDenominacionId == FiltroEntidad.MonedaDenominacionId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaEfectivoDenominacionId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarMonedaDenominacion)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.MonedaDenominacion = _estructuraEntidad.B_ConsultarMonedaDenominacion
                    ? ConsultarMonedaDenominacion(entidad.MonedaDenominacionId)
                    : null;
            }
        }

        private EnMonedaDenominacion ConsultarMonedaDenominacion(int monedaDenominacionId)
        {
            RpMonedaDenominacion rpMonedaDenominacion = SvMonedaDenominacionIniciarRepositorio.IniciarRepositorio();
            EsMonedaDenominacion esMonedaDenominacion = new EsMonedaDenominacion()
            {
                MonedaDenominacion = new EnMonedaDenominacion()
                {
                    MonedaDenominacionId = monedaDenominacionId
                }
            };
            SvMonedaDenominacionConsultar servicio = new SvMonedaDenominacionConsultar(rpMonedaDenominacion, esMonedaDenominacion);
            EnMonedaDenominacion MonedaDenominacion = servicio.Consultar().Single();
            return MonedaDenominacion;
        }
    }
}
