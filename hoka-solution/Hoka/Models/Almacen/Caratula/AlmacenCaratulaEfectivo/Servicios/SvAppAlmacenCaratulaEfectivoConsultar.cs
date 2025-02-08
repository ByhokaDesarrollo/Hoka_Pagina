using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaEfectivo.Servicios
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