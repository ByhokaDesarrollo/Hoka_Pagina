using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula
{
    public class SvUsuarioPermisoAlmacenCaratulaCaratulaConsultar
    {
        private RpUsuarioPermisoAlmacenCaratula _rpUsuarioPermisoAlmacenCaratula;
        private EsUsuarioPermisoAlmacenCaratula _estructuraEntidad;
        private ICollection<EnUsuarioPermisoAlmacenCaratula> _entidades;

        public SvUsuarioPermisoAlmacenCaratulaCaratulaConsultar(
            RpUsuarioPermisoAlmacenCaratula repositorio,
            EsUsuarioPermisoAlmacenCaratula esUsuarioPermisoAlmacenCaratula)
        {
            _rpUsuarioPermisoAlmacenCaratula = repositorio;
            _estructuraEntidad = esUsuarioPermisoAlmacenCaratula;
            _entidades = new HashSet<EnUsuarioPermisoAlmacenCaratula>();
        }

        public ICollection<EnUsuarioPermisoAlmacenCaratula> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            return _entidades;
        }

        public void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad.Consulta;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnUsuarioPermisoAlmacenCaratula FiltroEntidad = _estructuraEntidad.Permiso;
            IEnumerable<EnUsuarioPermisoAlmacenCaratula> Query = _rpUsuarioPermisoAlmacenCaratula._dbSet;
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
                    Query = Query.Where(x => x.UsuarioPermisoId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.UsuarioPermisoId > 0)
                    Query = Query.Where(q => q.UsuarioPermisoId == FiltroEntidad.UsuarioPermisoId);
                if (FiltroEntidad.UsuarioId > 0)
                    Query = Query.Where(q => q.UsuarioId == FiltroEntidad.UsuarioId);
                if (FiltroEntidad.AlmacenId > 0)
                    Query = Query.Where(q => q.AlmacenId == FiltroEntidad.AlmacenId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.UsuarioPermisoId);
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
