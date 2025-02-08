using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Almacen
{
    public class SvAlmacenConsultar
    {
        private RpAlmacen _rpAlmacen;
        private EsAlmacen _estructuraEntidad;
        private ICollection<EnAlmacen> _entidades;

        public SvAlmacenConsultar(RpAlmacen repositorio, EsAlmacen esAlmacen)
        {
            _rpAlmacen = repositorio;
            _estructuraEntidad = esAlmacen;
            _entidades = new HashSet<EnAlmacen>();
        }

        public ICollection<EnAlmacen> Consultar()
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
            EnAlmacen FiltroEntidad = _estructuraEntidad.Almacen;
            IEnumerable<EnAlmacen> Query = _rpAlmacen._dbSet;
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenId > 0)
                    Query = Query.Where(q => q.AlmacenId == FiltroEntidad.AlmacenId);
                if (!string.IsNullOrEmpty(FiltroEntidad.SucursalPrefijo))
                    Query = Query.Where(q => q.SucursalPrefijo.ToUpper().Contains(FiltroEntidad.SucursalPrefijo.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Prefijo))
                    Query = Query.Where(q => q.Prefijo.ToUpper().Contains(FiltroEntidad.Prefijo.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.RFC))
                    Query = Query.Where(q => q.RFC.ToUpper().Contains(FiltroEntidad.RFC.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.CP))
                    Query = Query.Where(q => q.CP.ToUpper().Contains(FiltroEntidad.CP.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Ciudad))
                    Query = Query.Where(q => q.Ciudad.ToUpper().Contains(FiltroEntidad.Ciudad.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Estado))
                    Query = Query.Where(q => q.Estado.ToUpper().Contains(FiltroEntidad.Estado.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenId);
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
