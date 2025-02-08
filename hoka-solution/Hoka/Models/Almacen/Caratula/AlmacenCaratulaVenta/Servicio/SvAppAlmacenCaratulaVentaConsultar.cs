using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVenta;
using Newtonsoft.Json;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVenta.Servicio
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