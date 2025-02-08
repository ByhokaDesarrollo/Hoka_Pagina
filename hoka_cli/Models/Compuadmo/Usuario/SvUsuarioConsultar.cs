using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.UsuarioPerfil;
using hoka_cli.Models.Compuadmo.UsuarioRol;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public class SvUsuarioConsultar
    {
        private RpUsuario _rpUsuario;
        private EsUsuario _estructuraEntidad;
        private ICollection<EnUsuario> _entidades;

        public SvUsuarioConsultar(RpUsuario repositorio, EsUsuario esUsuario)
        {
            _rpUsuario = repositorio;
            _estructuraEntidad = esUsuario;
            _entidades = new HashSet<EnUsuario>();
        }

        public ICollection<EnUsuario> Consultar()
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
            EnUsuario FiltroEntidad = _estructuraEntidad.Usuario;
            IEnumerable<EnUsuario> Query = _rpUsuario._dbSet;
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
                    Query = Query.Where(x => x.UsuarioId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.UsuarioId > 0)
                    Query = Query.Where(q => q.UsuarioId == FiltroEntidad.UsuarioId);
                if (FiltroEntidad.UsuarioRolId > 0)
                    Query = Query.Where(q => q.UsuarioRolId == FiltroEntidad.UsuarioRolId);
                if (FiltroEntidad.UsuarioPerfilId > 0)
                    Query = Query.Where(q => q.UsuarioPerfilId == FiltroEntidad.UsuarioPerfilId);
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Correo))
                    Query = Query.Where(q => q.Correo.ToUpper().Contains(FiltroEntidad.Correo.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.UsuarioId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarRol ||
                _estructuraEntidad.B_ConsultarPerfil)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.UsuarioRol = _estructuraEntidad.B_ConsultarRol
                    ? ConsultarUsuarioRol(entidad.UsuarioRolId)
                    : null;
                entidad.UsuarioPerfil = _estructuraEntidad.B_ConsultarPerfil
                    ? ConsultarUsuarioPerfil(entidad.UsuarioPerfilId)
                    : null;
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

        private EnUsuarioPerfil ConsultarUsuarioPerfil(int usuarioPerfilId)
        {
            RpUsuarioPerfil rpUsuarioPerfil = SvUsuarioPerfilIniciarRepositorio.IniciarRepositorio();
            EsUsuarioPerfil esUsuarioPerfil = new EsUsuarioPerfil()
            {
                UsuarioPerfil = new EnUsuarioPerfil()
                {
                    UsuarioPerfilId = usuarioPerfilId
                }
            };
            SvUsuarioPerfilConsultar servicio = new SvUsuarioPerfilConsultar(rpUsuarioPerfil, esUsuarioPerfil);
            EnUsuarioPerfil UsuarioPerfil = servicio.Consultar().Single();
            return UsuarioPerfil;
        }
    }
}
