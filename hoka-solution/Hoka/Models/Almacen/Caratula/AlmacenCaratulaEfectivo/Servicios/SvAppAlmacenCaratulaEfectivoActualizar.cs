using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaEfectivo = hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.EnAlmacenCaratulaEfectivo;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaEfectivo.Servicios
{
    public static class SvAppAlmacenCaratulaEfectivoActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaEfectivo entidad)
        {
            HokaCli_EnAlmacenCaratulaEfectivo ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaEfectivoActualizar.Actualizar(ObjetoConvertido);
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