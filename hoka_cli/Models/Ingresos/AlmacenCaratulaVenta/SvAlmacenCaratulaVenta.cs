using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class SvAlmacenCaratulaVenta
    {
        private readonly Dictionary<string, Action<EsAlmacenCaratulaVenta>> _acciones;

        public SvAlmacenCaratulaVenta()
        {
            _acciones = new Dictionary<string, Action<EsAlmacenCaratulaVenta>>()
            {
                { "Crear", Crear },
                { "Consultar", Consultar },
                { "Actualizar", Actualizar },
                { "Grabar", Grabar },
            };
            Estructura = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = null,
                CaratulasVenta = null
            };
        }

        public EsAlmacenCaratulaVenta Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsAlmacenCaratulaVenta estructura)
        {
            try
            {
                if (estructura == null)
                    new SvAlmacenCaratulaVentaException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.CaratulasVenta?.Count ?? 0;
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

        public void Crear(EsAlmacenCaratulaVenta estructura)
        {
            SvAlmacenCaratulaVentaCrear servicio = new SvAlmacenCaratulaVentaCrear(estructura);
            servicio.Crear();
            Estructura.CaratulaVenta = servicio.Entidad;
        }

        public void Consultar(EsAlmacenCaratulaVenta estructura)
        {
            RpAlmacenCaratulaVenta rpAlmacenCaratulaVenta =
                SvAlmacenCaratulaVentaIniciarRepositorio.IniciarRepositorio();
            SvAlmacenCaratulaVentaConsultar servicio =
                new SvAlmacenCaratulaVentaConsultar
                (
                    repositorio: rpAlmacenCaratulaVenta,
                    esAlmacenCaratulaVenta: estructura
                );
            Estructura.CaratulasVenta = servicio.Consultar();
        }

        public void Actualizar(EsAlmacenCaratulaVenta estructura)
        {
            SvAlmacenCaratulaVentaActualizar servicio = new SvAlmacenCaratulaVentaActualizar(estructura);
            servicio.Actualizar();
            Estructura.CaratulaVenta = servicio.EntidadBaseDatos;
        }

        public void Grabar(EsAlmacenCaratulaVenta estructura)
        {
            SvAlmacenCaratulaVentaGrabar servicio = new SvAlmacenCaratulaVentaGrabar(estructura);
            servicio.Grabar();
            Estructura.CaratulaVenta = servicio.Entidad;
        }
    }
}
