using hoka.HokaCli.Models.Compuadmo.Moneda;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAppMonedaDenominacionConsultar
    {
        public static ICollection<EnMoneda> Consultar()
        {
            string jsonMonedasDenominacion = SvMonedaDenominacionConsultar.Consultar();
            ICollection<EnMoneda> MonedasDenominacion = JsonConvert.DeserializeObject<ICollection<EnMoneda>>(jsonMonedasDenominacion);
            return MonedasDenominacion;
        }
    }
}