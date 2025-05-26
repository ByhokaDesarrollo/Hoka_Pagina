using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVenta;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaVenta = hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.EnAlmacenCaratulaVenta;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAppAlmacenCaratulaVentaGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaVenta entidad)
        {
            HokaCli_EnAlmacenCaratulaVenta ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaVentaGrabar.Grabar(ObjetoConvertido);
            return resultado;
        }

        private static HokaCli_EnAlmacenCaratulaVenta ConvertirParametro(EnAlmacenCaratulaVenta entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacenCaratulaVenta ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCaratulaVenta>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}