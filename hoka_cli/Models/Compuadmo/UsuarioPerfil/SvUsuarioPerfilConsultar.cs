using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.UsuarioRol;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.UsuarioPerfil
{
    public class SvUsuarioPerfilConsultar
    {
        private RpUsuarioPerfil _rpUsuarioPerfil;
        private EsUsuarioPerfil _estructuraEntidad;
        private ICollection<EnUsuarioPerfil> _entidades;

        public SvUsuarioPerfilConsultar(RpUsuarioPerfil repositorio, EsUsuarioPerfil esUsuarioPerfil)
        {
            _rpUsuarioPerfil = repositorio;
            _estructuraEntidad = esUsuarioPerfil;
            _entidades = new HashSet<EnUsuarioPerfil>();
        }

        public ICollection<EnUsuarioPerfil> Consultar()
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
            EnUsuarioPerfil FiltroEntidad = _estructuraEntidad.UsuarioPerfil;
            IEnumerable<EnUsuarioPerfil> Query = _rpUsuarioPerfil._dbSet;
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
                    Query = Query.Where(x => x.UsuarioPerfilId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.UsuarioPerfilId > 0)
                    Query = Query.Where(q => q.UsuarioPerfilId == FiltroEntidad.UsuarioPerfilId);
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
                    Query = Query.OrderByDescending(q => q.UsuarioPerfilId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarRol)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.UsuarioRol = ConsultarUsuarioRol(entidad.UsuarioRolId);
            }
        }

        private EnUsuarioRol ConsultarUsuarioRol(int usuarioRolId)
        {
            RpUsuarioRol rpUsuarioRol = SvUsuarioRolIniciarRepositorio.IniciarRepositorio();
            EsUsuarioRol esUsuarioRol = new EsUsuarioRol()
            {
                UsuarioRol = new EnUsuarioRol()
                {
                    UsuarioRolId = usuarioRolId
                }
            };
            SvUsuarioRolConsultar servicio = new SvUsuarioRolConsultar(rpUsuarioRol, esUsuarioRol);
            EnUsuarioRol UsuarioRol = servicio.Consultar().Single();
            return UsuarioRol;
        }
    }
}
