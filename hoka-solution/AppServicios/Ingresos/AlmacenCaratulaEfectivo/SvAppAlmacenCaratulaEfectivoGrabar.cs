using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaEfectivo = hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.EnAlmacenCaratulaEfectivo;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAppAlmacenCaratulaEfectivoGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaEfectivo entidad)
        {
            HokaCli_EnAlmacenCaratulaEfectivo ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaEfectivoGrabar.Grabar(ObjetoConvertido);
            return resultado;
        }

        private static HokaCli_EnAlmacenCaratulaEfectivo ConvertirParametro(EnAlmacenCaratulaEfectivo entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacenCaratulaEfectivo ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCaratulaEfectivo>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}