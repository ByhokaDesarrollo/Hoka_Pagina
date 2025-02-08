using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratula;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios
{
    public static class SvAppAlmacenCaratulaDesbloquear
    {
        public static bool Desbloquear(EnAlmacenCaratula entidad)
        {
            HokaCli_EnAlmacenCaratula ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaDesbloquear.Desbloquear(ObjetoConvertido);
            return resultado;
        }

        private static HokaCli_EnAlmacenCaratula ConvertirParametro(EnAlmacenCaratula entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacenCaratula ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCaratula>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}