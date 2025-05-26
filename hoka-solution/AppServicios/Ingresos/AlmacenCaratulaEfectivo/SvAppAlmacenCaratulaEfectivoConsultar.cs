using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAppAlmacenCaratulaEfectivoConsultar
    {
        public static EnAlmacenCaratulaEfectivo Consultar(int almacenCaratulaId)
        {
            string jsonAlmacenCaratulaEfectivo = SvAlmacenCaratulaEfectivoConsultar
                .Consultar(almacenCaratulaId);
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo = JsonConvert
                .DeserializeObject<EnAlmacenCaratulaEfectivo>(jsonAlmacenCaratulaEfectivo);
            return AlmacenCaratulaEfectivo;
        }
    }
}