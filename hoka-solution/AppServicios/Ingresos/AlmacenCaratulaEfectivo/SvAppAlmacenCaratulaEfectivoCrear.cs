using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaEfectivo = hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.EnAlmacenCaratulaEfectivo;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAppAlmacenCaratulaEfectivoCrear
    {
        public static bool Crear(EnAlmacenCaratulaEfectivo entidad)
        {
            HokaCli_EnAlmacenCaratulaEfectivo ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaEfectivoCrear.Crear(ObjetoConvertido);
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