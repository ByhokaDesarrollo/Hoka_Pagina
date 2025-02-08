using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Moneda.Denominacion;
using hoka_cli.Models.Compuadmo.Moneda.Tipo;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    public class SvMonedaConsultar
    {
        private RpMoneda _rpMoneda;
        private EsMoneda _estructuraEntidad;
        private ICollection<EnMoneda> _entidades;

        public SvMonedaConsultar(RpMoneda repositorio, EsMoneda esMoneda)
        {
            _rpMoneda = repositorio;
            _estructuraEntidad = esMoneda;
            _entidades = new HashSet<EnMoneda>();
        }

        public ICollection<EnMoneda> Consultar()
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
            EnMoneda FiltroEntidad = _estructuraEntidad?.Moneda ?? null;
            string query = $"EXECUTE SP_Moneda_GenerarTabla";
            IEnumerable<EnMoneda> Query = _rpMoneda.FindByQuery(query);
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.TablaRegistroEstatusId == 1)
                    Query = Query.Where(q => q.B_Activo == true);
                if (FiltroConsulta.TablaRegistroEstatusId == 2)
                    Query = Query.Where(q => q.B_Activo == false);
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.MonedaId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.MonedaPK > 0)
                    Query = Query.Where(q => q.MonedaPK == FiltroEntidad.MonedaPK);
                if (FiltroEntidad.MonedaId >= 0 && _estructuraEntidad.B_ConsultarMonedaCero)
                    Query = Query.Where(q => q.MonedaId == FiltroEntidad.MonedaId);
                if (FiltroEntidad.MonedaId > 0)
                    Query = Query.Where(q => q.MonedaId == FiltroEntidad.MonedaId);
                if (FiltroEntidad.MonedaTipoId > 0)
                    Query = Query.Where(q => q.MonedaTipoId == FiltroEntidad.MonedaTipoId);
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.MonedaId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarDenominacion)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.MonedaTipo = _estructuraEntidad.B_ConsultarMonedaTipo
                    ? ConsultarMonedaTipo(entidad.MonedaTipoId)
                    : null;
                entidad.Denominaciones = _estructuraEntidad.B_ConsultarDenominacion
                    ? ConsultarDenominaciones(entidad.MonedaId)
                    : null;
            }
        }

        private EnMonedaTipo ConsultarMonedaTipo(int monedaTipoId)
        {
            RpMonedaTipo rpMonedaTipo = SvMonedaTipoIniciarRepositorio.IniciarRepositorio();
            EsMonedaTipo esMonedaTipo = new EsMonedaTipo()
            {
                MonedaTipo = new EnMonedaTipo()
                {
                    MonedaTipoId = monedaTipoId
                }
            };
            SvMonedaTipoConsultar servicio = new SvMonedaTipoConsultar(rpMonedaTipo, esMonedaTipo);
            EnMonedaTipo MonedaTipo = servicio.Consultar().Single();
            return MonedaTipo;
        }

        private ICollection<EnMonedaDenominacion> ConsultarDenominaciones(int monedaId)
        {
            RpMonedaDenominacion rpMonedaDenominacion = SvMonedaDenominacionIniciarRepositorio.IniciarRepositorio();
            EsMonedaDenominacion esMonedaDenominacion = new EsMonedaDenominacion()
            {
                MonedaDenominacion = new EnMonedaDenominacion()
                {
                    MonedaId = monedaId
                },
                B_ConsultarMonedaCero = true
            };
            SvMonedaDenominacionConsultar servicio = new SvMonedaDenominacionConsultar(
                rpMonedaDenominacion,
                esMonedaDenominacion);
            ICollection< EnMonedaDenominacion> MonedaDenominaciones = servicio.Consultar().ToHashSet();
            return MonedaDenominaciones;
        }
    }
}
