using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVenta;
using Newtonsoft.Json;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAppAlmacenCaratulaVentaConsultar
    {
        public static EnAlmacenCaratulaVenta Consultar(int almacenCaratulaId)
        {
            string jsonAlmacenCaratulaVenta = SvAlmacenCaratulaVentaConsultar
                .Consultar(almacenCaratulaId);
            EnAlmacenCaratulaVenta AlmacenCaratulaVenta = JsonConvert
                .DeserializeObject<EnAlmacenCaratulaVenta>(jsonAlmacenCaratulaVenta);
            return AlmacenCaratulaVenta;
        }
    }
}