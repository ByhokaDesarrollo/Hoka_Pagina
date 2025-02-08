using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Moneda.Denominacion
{
    public class SvMonedaDenominacionConsultar
    {
        private RpMonedaDenominacion _rpMonedaDenominacion;
        private EsMonedaDenominacion _estructuraEntidad;
        private ICollection<EnMonedaDenominacion> _entidades;

        public SvMonedaDenominacionConsultar(RpMonedaDenominacion repositorio, EsMonedaDenominacion esMonedaDenominacion)
        {
            _rpMonedaDenominacion = repositorio;
            _estructuraEntidad = esMonedaDenominacion;
            _entidades = new HashSet<EnMonedaDenominacion>();
        }

        public ICollection<EnMonedaDenominacion> Consultar()
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
            EnMonedaDenominacion FiltroEntidad = _estructuraEntidad.MonedaDenominacion;
            IEnumerable<EnMonedaDenominacion> Query = _rpMonedaDenominacion._dbSet;
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
                    Query = Query.Where(x => x.MonedaDenominacionId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.MonedaDenominacionId > 0)
                    Query = Query.Where(q => q.MonedaDenominacionId == FiltroEntidad.MonedaDenominacionId);
                if (_estructuraEntidad.B_ConsultarMonedaCero)
                    if (FiltroEntidad.MonedaId >= 0)
                        Query = Query.Where(q => q.MonedaId == FiltroEntidad.MonedaId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.MonedaDenominacionId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            // Insertar Codigo
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            //foreach (var entidad in _entidades)
            //{
            //    Insertar Codigo
            //}
        }
    }
}
