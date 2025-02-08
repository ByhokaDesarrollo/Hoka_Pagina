using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class SvAlmacenCaratulaEfectivo
    {
        private readonly Dictionary<string, Action<EsAlmacenCaratulaEfectivo>> _acciones;

        public SvAlmacenCaratulaEfectivo()
        {
            _acciones = new Dictionary<string, Action<EsAlmacenCaratulaEfectivo>>()
            {
                { "Crear", Crear },
                { "Consultar", Consultar },
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
                    new SvAlmacenCaratulaEfectivoException("Error");
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

        public void Crear(EsAlmacenCaratulaEfectivo estructura)
        {
            SvAlmacenCaratulaEfectivoCrear servicio = new SvAlmacenCaratulaEfectivoCrear(estructura);
            servicio.Crear();
            Estructura.CaratulaEfectivo = servicio.Entidad;
        }

        public void Consultar(EsAlmacenCaratulaEfectivo estructura)
        {
            RpAlmacenCaratulaEfectivo rpAlmacenCaratulaEfectivo =
                SvAlmacenCaratulaEfectivoIniciarRepositorio.IniciarRepositorio();
            SvAlmacenCaratulaEfectivoConsultar servicio =
                new SvAlmacenCaratulaEfectivoConsultar
                (
                    repositorio: rpAlmacenCaratulaEfectivo,
                    esAlmacenCaratulaEfectivo: estructura
                );
            Estructura.CaratulasEfectivo = servicio.Consultar();
        }

        public void Actualizar(EsAlmacenCaratulaEfectivo estructura)
        {
            SvAlmacenCaratulaEfectivoActualizar servicio = new SvAlmacenCaratulaEfectivoActualizar(estructura);
            servicio.Actualizar();
            Estructura.CaratulaEfectivo = servicio.EntidadBaseDatos;
        }

        public void Grabar(EsAlmacenCaratulaEfectivo estructura)
        {
            SvAlmacenCaratulaEfectivoGrabar servicio = new SvAlmacenCaratulaEfectivoGrabar(estructura);
            servicio.Grabar();
            Estructura.CaratulaEfectivo = servicio.Entidad;
        }
    }
}
