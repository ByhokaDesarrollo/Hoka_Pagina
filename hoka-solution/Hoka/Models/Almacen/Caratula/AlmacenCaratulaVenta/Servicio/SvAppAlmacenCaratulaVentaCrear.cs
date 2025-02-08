using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVenta;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaVenta = hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.EnAlmacenCaratulaVenta;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVenta.Servicio
{
    public static class SvAppAlmacenCaratulaVentaCrear
    {
        public static bool Crear(EnAlmacenCaratulaVenta entidad)
        {
            HokaCli_EnAlmacenCaratulaVenta ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaVentaCrear.Crear(ObjetoConvertido);
            return resultado;
        }

        private static HokaCli_EnAlmacenCaratulaVenta ConvertirParametro(EnAlmacenCaratulaVenta entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacenCaratulaVenta ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCaratulaVenta>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}