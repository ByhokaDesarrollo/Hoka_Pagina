using hoka_cli.Models.Compuadmo.Moneda;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo
{
    public static class SvMonedaDenominacionConsultar
    {
        public static string Consultar()
        {
            SvMoneda svMoneda = new SvMoneda();
            EsMoneda esMoneda = new EsMoneda()
            {
                Moneda = new EnMoneda()
                {
                    MonedaTipoId = 1
                },
                B_ConsultarMonedaTipo = true,
                B_ConsultarDenominacion = true
            };
            svMoneda.ServicioMaestro("Consultar", esMoneda);
            ICollection<EnMoneda> MonedasDenominacion = svMoneda.Estructura.Monedas;
            string jsonMonedasDenominacion = JsonConvert.SerializeObject(MonedasDenominacion);
            return jsonMonedasDenominacion;
        }
    }
}
