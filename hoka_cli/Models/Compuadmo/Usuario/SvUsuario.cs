using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public class SvUsuario
    {
        private readonly Dictionary<string, Action<EsUsuario>> _acciones;

        public SvUsuario()
        {
            _acciones = new Dictionary<string, Action<EsUsuario>>()
            {
                { "IniciarSesion", IniciarSesion },
                { "Consultar", Consultar }
            };
            Estructura = new EsUsuario()
            {
                Usuario = null,
                Usuarios = null
            };
        }

        public EsUsuario Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsUsuario estructura)
        {
            try
            {
                if (estructura == null)
                    new SvUsuarioException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.Usuarios?.Count ?? 0;
                Estructura.EstablecerObjetoConsultaNulo();
                Estructura.EstablecerObjetoErrorNulo();
            }
            catch (Exception e)
            {
                Estructura.Propiedad.Id = 0;
                Estructura.Propiedad.Mensaje = "Error: Check error object for more detail";
                Estructura.Propiedad.TotalRegistros = 0;
                Estructura.Error.Mensaje = e.Message;
                Estructura.Error.Fuente = e.Source;
                Estructura.Error.ExcepcionInterna = e.InnerException?.Message ?? "";
                Estructura.Error.SeguimientoPila = e.StackTrace;
            }
        }

        public void IniciarSesion(EsUsuario estructura)
        {
            RpUsuario rpUsuario =
                SvUsuarioIniciarRepositorio.IniciarRepositorio();
            SvUsuarioIniciarSesion servicio =
                new SvUsuarioIniciarSesion
                (
                    repositorio: rpUsuario,
                    esUsuario: estructura
                );
            Estructura.Usuario = servicio.IniciarSesion();
        }

        public void Consultar(EsUsuario estructura)
        {
            RpUsuario rpUsuario =
                SvUsuarioIniciarRepositorio.IniciarRepositorio();
            SvUsuarioConsultar servicio =
                new SvUsuarioConsultar
                (
                    repositorio: rpUsuario,
                    esUsuario: estructura
                );
            Estructura.Usuarios = servicio.Consultar();
        }
    }
}
