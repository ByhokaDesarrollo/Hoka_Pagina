using hoka.HokaCli.Models.Compuadmo.Moneda;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaEfectivo.Servicios
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