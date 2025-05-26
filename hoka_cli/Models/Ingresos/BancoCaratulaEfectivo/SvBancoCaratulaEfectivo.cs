using System.Collections.Generic;
using System;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;

namespace hoka_cli.Models.Ingresos.BancoCaratulaEfectivo
{
    public class SvBancoCaratulaEfectivo
    {
        private readonly Dictionary<string, Action<EsAlmacenCaratulaEfectivo>> _acciones;

        public SvBancoCaratulaEfectivo()
        {
            _acciones = new Dictionary<string, Action<EsAlmacenCaratulaEfectivo>>()
            {
                { "Actualizar", Actualizar },
                { "Grabar", Grabar },
            };
            Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = null,
                CaratulasEfectivo = null
            };
        }

        public EsAlmacenCaratulaEfectivo Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsAlmacenCaratulaEfectivo estructura)
        {
            try
            {
                if (estructura == null)
                    new SvBancoCaratulaEfectivoException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.CaratulasEfectivo?.Count ?? 0;
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

        public void Actualizar(EsAlmacenCaratulaEfectivo estructura)
        {
            SvBancoCaratulaEfectivoActualizar servicio = new SvBancoCaratulaEfectivoActualizar(estructura);
            servicio.Actualizar();
            Estructura.CaratulaEfectivo = servicio.EntidadBaseDatos;
        }

        public void Grabar(EsAlmacenCaratulaEfectivo estructura)
        {
            SvBancoCaratulaEfectivoGrabar servicio = new SvBancoCaratulaEfectivoGrabar(estructura);
            servicio.Grabar();
            Estructura.CaratulaEfectivo = servicio.Entidad;
        }
    }
}
