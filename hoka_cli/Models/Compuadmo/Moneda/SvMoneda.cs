using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    public class SvMoneda
    {
        private readonly Dictionary<string, Action<EsMoneda>> _acciones;

        public SvMoneda()
        {
            _acciones = new Dictionary<string, Action<EsMoneda>>()
            {
                { "Consultar", Consultar }
            };
            Estructura = new EsMoneda()
            {
                Moneda = null,
                Monedas = null
            };
        }

        public EsMoneda Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsMoneda estructura)
        {
            try
            {
                if (estructura == null)
                    new SvMonedaException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura.Monedas?.Count ?? 0;

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

        public void Consultar(EsMoneda estructura)
        {
            RpMoneda rpMoneda = SvMonedaIniciarRepositorio.IniciarRepositorio();
            SvMonedaConsultar servicio =
                new SvMonedaConsultar
                (
                    repositorio: rpMoneda,
                    esMoneda: estructura
                );
            Estructura.Monedas = servicio.Consultar();
        }
    }
}
