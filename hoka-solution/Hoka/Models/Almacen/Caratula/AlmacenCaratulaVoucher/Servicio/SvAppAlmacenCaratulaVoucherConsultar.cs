using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher;
using Newtonsoft.Json;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVoucher.Servicio
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