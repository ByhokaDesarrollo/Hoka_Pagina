using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaCaratula;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaReporte.Servicios
{
    public static class SvAppAlmacenCaratulaReporteConsultar
    {
        public static EnAlmacenCaratula Consultar(EnAlmacenCaratula parametro)
        {
            HokaCli_EnAlmacenCaratula ObjetoConvertido = ConvertirParametro(parametro);
            string jsonCaratula = SvAlmacenCaratulaReporteConsultar.Consultar(ObjetoConvertido);
            EnAlmacenCaratula Caratula = JsonConvert
                .DeserializeObject<EnAlmacenCaratula>(jsonCaratula);
            return Caratula;
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