using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher;
using Newtonsoft.Json;
using HokaCli_EnAlmacenCaratulaVoucher = hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.EnAlmacenCaratulaVoucher;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVoucher.Servicio
{
    public static class SvAppAlmacenCaratulaVoucherActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaVoucher entidad)
        {
            HokaCli_EnAlmacenCaratulaVoucher ObjetoConvertido = ConvertirParametro(entidad);
            bool resultado = SvAlmacenCaratulaVoucherActualizar.Actualizar(ObjetoConvertido);
            return resultado;
        }

        private static HokaCli_EnAlmacenCaratulaVoucher ConvertirParametro(EnAlmacenCaratulaVoucher entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacenCaratulaVoucher ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCaratulaVoucher>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}