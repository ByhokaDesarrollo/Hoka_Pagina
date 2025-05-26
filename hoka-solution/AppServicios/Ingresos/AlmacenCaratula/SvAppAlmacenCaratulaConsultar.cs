using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratula = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;

namespace hoka.AppServicios.Ingresos.AlmacenCaratula
{
    public static class SvAppAlmacenCaratulaConsultar
    {
        public static EnAlmacenCaratula Consultar(int almacenCaratulaId)
        {
            EnAlmacenCaratula CaratulaParametro = new EnAlmacenCaratula()
            {
                AlmacenCaratulaId = almacenCaratulaId
            };
            HokaCli_EnAlmacenCaratula ObjetoConvertido = ConvertirParametro(CaratulaParametro);
            string jsonCaratula = SvAlmacenCaratulaConsultar.Consultar(ObjetoConvertido);
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