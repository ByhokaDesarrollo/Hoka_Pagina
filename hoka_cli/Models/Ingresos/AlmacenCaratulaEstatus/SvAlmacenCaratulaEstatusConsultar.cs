using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus
{
    public class SvAlmacenCaratulaEstatusConsultar
    {
        private RpAlmacenCaratulaEstatus _rpAlmacenCaratulaEstatus;
        private EsAlmacenCaratulaEstatus _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaEstatus> _entidades;

        public SvAlmacenCaratulaEstatusConsultar(
            RpAlmacenCaratulaEstatus repositorio,
            EsAlmacenCaratulaEstatus esAlmacenCaratulaEstatus)
        {
            _rpAlmacenCaratulaEstatus = repositorio;
            _estructuraEntidad = esAlmacenCaratulaEstatus;
            _entidades = new HashSet<EnAlmacenCaratulaEstatus>();
        }

        public ICollection<EnAlmacenCaratulaEstatus> Consultar()
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
            EnAlmacenCaratulaEstatus FiltroEntidad = _estructuraEntidad.CaratulaEstatus;
            IEnumerable<EnAlmacenCaratulaEstatus> Query = _rpAlmacenCaratulaEstatus._dbSet;
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
                    Query = Query.Where(x => x.AlmacenCaratulaEstatusId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaEstatusId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEstatusId == FiltroEntidad.AlmacenCaratulaEstatusId);
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
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaEstatusId);
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
