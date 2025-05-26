using System.Collections.Generic;
using System;
using hoka_cli.Models.Ingresos.AlmacenCaratula;

namespace hoka_cli.Models.Ingresos.BancoCaratula
{
    public class SvBancoCaratula
    {
        private readonly Dictionary<string, Action<EsAlmacenCaratula>> _acciones;

        public SvBancoCaratula()
        {
            _acciones = new Dictionary<string, Action<EsAlmacenCaratula>>()
            {
                { "Consultar", Consultar },
                { "Grabar", Grabar },
                { "Desbloquear", Desbloquear },
            };
            Estructura = new EsAlmacenCaratula()
            {
                Caratula = null,
                Caratulas = null
            };
        }

        public EsAlmacenCaratula Estructura { get; set; }

        public void ServicioMaestro(string nombreMetodo, EsAlmacenCaratula estructura)
        {
            try
            {
                if (estructura == null)
                    new SvAlmacenCaratulaException("Error");
                _acciones[nombreMetodo](estructura);
                Estructura.Propiedad.Id = 1;
                Estructura.Propiedad.Mensaje = "OK";
                Estructura.Propiedad.TotalRegistros = Estructura?.Caratulas?.Count ?? 0;
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

        public void Consultar(EsAlmacenCaratula estructura)
        {
            RpAlmacenCaratula rpAlmacenCaratula =
                SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio();
            SvBancoCaratulaConsultar servicio =
                new SvBancoCaratulaConsultar
                (
                    repositorio: rpAlmacenCaratula,
                    esAlmacenCaratula: estructura
                );
            Estructura.Caratulas = servicio.Consultar();
        }

        public void Grabar(EsAlmacenCaratula estructura)
        {
            SvAlmacenCaratulaGrabar servicio = new SvAlmacenCaratulaGrabar(estructura);
            Estructura.Caratula = servicio.Grabar();
        }

        public void Desbloquear(EsAlmacenCaratula estructura)
        {
            SvAlmacenCaratulaDesbloquear servicio = new SvAlmacenCaratulaDesbloquear(estructura);
            Estructura.Caratula = servicio.Desbloquear();
        }
    }
}
