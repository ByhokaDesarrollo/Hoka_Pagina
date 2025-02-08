using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class SvAlmacenCaratulaVoucher
    {
        private readonly Dictionary<string, Action<EsAlmacenCaratulaVoucher>> _acciones;

        public SvAlmacenCaratulaVoucher()
        {
            _acciones = new Dictionary<string, Action<EsAlmacenCaratulaVoucher>>()
            {
                { "Crear", Crear },
                { "Consultar", Consultar },
                { "Actualizar", Actualizar },
                { "Grabar", Grabar },
            };
            Estructura = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = null,
                CaratulasVoucher = null
            };
        }

        public EsAlmacenCaratulaVoucher Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsAlmacenCaratulaVoucher estructura)
        {
            try
            {
                if (estructura == null)
                    new SvAlmacenCaratulaVoucherException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.CaratulasVoucher?.Count ?? 0;
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

        public void Crear(EsAlmacenCaratulaVoucher estructura)
        {
            SvAlmacenCaratulaVoucherCrear servicio = new SvAlmacenCaratulaVoucherCrear(estructura);
            servicio.Crear();
            Estructura.CaratulaVoucher = servicio.Entidad;
        }

        public void Consultar(EsAlmacenCaratulaVoucher estructura)
        {
            RpAlmacenCaratulaVoucher rpAlmacenCaratulaVoucher =
                SvAlmacenCaratulaVoucherIniciarRepositorio.IniciarRepositorio();
            SvAlmacenCaratulaVoucherConsultar servicio =
                new SvAlmacenCaratulaVoucherConsultar
                (
                    repositorio: rpAlmacenCaratulaVoucher,
                    esAlmacenCaratulaVoucher: estructura
                );
            Estructura.CaratulasVoucher = servicio.Consultar();
        }

        public void Actualizar(EsAlmacenCaratulaVoucher estructura)
        {
            SvAlmacenCaratulaVoucherActualizar servicio =
                new SvAlmacenCaratulaVoucherActualizar(estructura);
            servicio.Actualizar();
            Estructura.CaratulaVoucher = servicio.EntidadBaseDatos;
        }

        public void Grabar(EsAlmacenCaratulaVoucher estructura)
        {
            SvAlmacenCaratulaVoucherGrabar servicio =
                new SvAlmacenCaratulaVoucherGrabar(estructura);
            servicio.Grabar();
            Estructura.CaratulaVoucher = servicio.Entidad;
        }
    }
}
