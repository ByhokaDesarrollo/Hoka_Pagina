using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVoucher;
using Newtonsoft.Json;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaVoucher
{
    public static class SvAppAlmacenCaratulaVoucherConsultar
    {
        public static EnAlmacenCaratulaVoucher Consultar(int almacenCaratulaId)
        {
            string jsonAlmacenCaratulaVoucher = SvAlmacenCaratulaVoucherConsultar
                .Consultar(almacenCaratulaId);
            EnAlmacenCaratulaVoucher AlmacenCaratulaVoucher = JsonConvert
                .DeserializeObject<EnAlmacenCaratulaVoucher>(jsonAlmacenCaratulaVoucher);
            return AlmacenCaratulaVoucher;
        }
    }
}