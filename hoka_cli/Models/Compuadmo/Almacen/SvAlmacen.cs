using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Compuadmo.Almacen
{
    public class SvAlmacen
    {
        private readonly Dictionary<string, Action<EsAlmacen>> _acciones;

        public SvAlmacen()
        {
            _acciones = new Dictionary<string, Action<EsAlmacen>>()
            {
                { "Consultar", Consultar }
            };
            Estructura = new EsAlmacen()
            {
                Almacen = null,
                Almacenes = null
            };
        }

        public EsAlmacen Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsAlmacen estructura)
        {
            try
            {
                if (estructura == null)
                    new SvAlmacenException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.Almacenes?.Count ?? 0;
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
                Estructura.Error.ExcepcionInterna = e.InnerException != null ? e.InnerException.Message : "";
                Estructura.Error.SeguimientoPila = e.StackTrace;
            }
        }

        public void Consultar(EsAlmacen estructura)
        {
            RpAlmacen rpAlmacen =
                SvAlmacenIniciarRepositorio.IniciarRepositorio();
            SvAlmacenConsultar servicio =
                new SvAlmacenConsultar
                (
                    repositorio: rpAlmacen,
                    esAlmacen: estructura
                );
            Estructura.Almacenes = servicio.Consultar();
        }
    }
}
