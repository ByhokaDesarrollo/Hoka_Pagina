using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Ingresos.BancoCaratulaEfectivo;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaEfectivo = hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.EnAlmacenCaratulaEfectivo;

namespace hoka.AppServicios.Ingresos.BancoCaratulaEfectivo
{
    public static class SvAppBancoCaratulaEfectivoActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaEfectivo entidad)
        {
            HokaCli_EnAlmacenCaratulaEfectivo ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvBancoCaratulaEfectivoActualizar.Actualizar(ObjetoConvertido);
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