using System.Collections.Generic;
using System;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    public class SvVoucher
    {
        private readonly Dictionary<string, Action<EsVoucher>> _acciones;

        public SvVoucher()
        {
            _acciones = new Dictionary<string, Action<EsVoucher>>()
            {
                { "Consultar", Consultar }
            };
            Estructura = new EsVoucher()
            {
                Voucher = null,
                Vouchers = null
            };
        }

        public EsVoucher Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsVoucher estructura)
        {
            try
            {
                if (estructura == null)
                    new SvVoucherException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura.Vouchers?.Count ?? 0;

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

        public void Consultar(EsVoucher estructura)
        {
            RpVoucher rpVoucher = SvVoucherIniciarRepositorio.IniciarRepositorio();
            SvVoucherConsultar servicio =
                new SvVoucherConsultar
                (
                    repositorio: rpVoucher,
                    esVoucher: estructura
                );
            Estructura.Vouchers = servicio.Consultar();
        }
    }
}
