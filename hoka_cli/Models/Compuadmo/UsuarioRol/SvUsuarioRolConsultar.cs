using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.UsuarioPerfil;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.UsuarioRol
{
    public class SvUsuarioRolConsultar
    {
        private RpUsuarioRol _rpUsuarioRol;
        private EsUsuarioRol _estructuraEntidad;
        private ICollection<EnUsuarioRol> _entidades;

        public SvUsuarioRolConsultar(RpUsuarioRol repositorio, EsUsuarioRol esUsuarioRol)
        {
            _rpUsuarioRol = repositorio;
            _estructuraEntidad = esUsuarioRol;
            _entidades = new HashSet<EnUsuarioRol>();
        }

        public ICollection<EnUsuarioRol> Consultar()
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
            EnUsuarioRol FiltroEntidad = _estructuraEntidad.UsuarioRol;
            IEnumerable<EnUsuarioRol> Query = _rpUsuarioRol._dbSet;
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
                    Query = Query.Where(x => x.UsuarioRolId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.UsuarioRolId > 0)
                    Query = Query.Where(q => q.UsuarioRolId == FiltroEntidad.UsuarioRolId);
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
                    Query = Query.OrderByDescending(q => q.UsuarioRolId);
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
