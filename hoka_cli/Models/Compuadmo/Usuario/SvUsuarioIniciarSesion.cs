using hoka_cli.Models.Compuadmo.Usuario.Permiso;
using hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka_cli.Models.Compuadmo.UsuarioPerfil;
using hoka_cli.Models.Compuadmo.UsuarioRol;
using hoka_cli.Models.Utileria.Encriptar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public class SvUsuarioIniciarSesion
    {
        private RpUsuario _rpUsuario;
        private EsUsuario _estructuraEntidad;
        private EnUsuario _entidad;

        public SvUsuarioIniciarSesion(RpUsuario repositorio, EsUsuario esUsuario)
        {
            _rpUsuario = repositorio;
            _estructuraEntidad = esUsuario;
            _entidad = new EnUsuario();
        }

        public EnUsuario IniciarSesion()
        {
            ObtenerEntidad();
            ObtnerPropiedades();
            return _entidad;
        }

        private void ObtenerEntidad()
        {
            string Correo = _estructuraEntidad.Usuario.Correo;
            string Clave = _estructuraEntidad.Usuario.Clave;
            Clave = EncriptarSha256.Encriptar(Clave);
            Expression<Func<EnUsuario, bool>> predicate =
                x =>
                x.Correo == Correo &&
                x.Clave == Clave;
            EnUsuario Query = _rpUsuario.SingleOrDefault(predicate);
            _entidad = Query;
        }

        private void ObtnerPropiedades()
        {
            if (_entidad != null)
            { 
                if (_entidad.UsuarioRolId > 0)
                    _entidad.UsuarioRol = _estructuraEntidad.B_ConsultarRol
                        ? ConsultarUsuarioRol(_entidad.UsuarioRolId)
                        : null;
                if (_entidad.UsuarioPerfilId > 0)
                    _entidad.UsuarioPerfil = _estructuraEntidad.B_ConsultarPerfil
                        ? ConsultarUsuarioPerfil(_entidad.UsuarioPerfilId)
                        : null;
                _entidad.Permiso = new EnUsuarioPermiso();
                _entidad.Permiso.PermisosAlmacenCaratula = _estructuraEntidad.B_ConsultarPermisoAlmacenCaratula
                    ? ConsultarUsuarioPermisoAlmacenCaratula(_entidad.UsuarioId)
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

        private ICollection<EnUsuarioPermisoAlmacenCaratula> ConsultarUsuarioPermisoAlmacenCaratula(int usuarioId)
        {
            RpUsuarioPermisoAlmacenCaratula rpUsuarioPermisoAlmacenCaratula =
                SvUsuarioPermisoAlmacenCaratulaIniciarRepositorio.IniciarRepositorio();
            EsUsuarioPermisoAlmacenCaratula esUsuarioPermisoAlmacenCaratula =
                new EsUsuarioPermisoAlmacenCaratula()
                {
                    Permiso = new EnUsuarioPermisoAlmacenCaratula()
                    {
                        UsuarioId = usuarioId
                    }
                };
            SvUsuarioPermisoAlmacenCaratulaCaratulaConsultar servicio =
                new SvUsuarioPermisoAlmacenCaratulaCaratulaConsultar(
                    rpUsuarioPermisoAlmacenCaratula,
                    esUsuarioPermisoAlmacenCaratula);
            ICollection<EnUsuarioPermisoAlmacenCaratula> PermisosAlmacenCaratula = servicio.Consultar();
            return PermisosAlmacenCaratula;
        }
    }
}
