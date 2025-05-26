using hoka.HokaCli.Models.Compuadmo.Voucher;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVoucher;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka.AppServicios.Ingresos.AlmacenCaratulaVoucher
{
    public static class SvAppVoucherConsultar
    {
        public static ICollection<EnVoucher> Consultar()
        {
            string jsonVouchers = SvVoucherConsultar.Consultar();
            ICollection<EnVoucher> Vouchers = JsonConvert.DeserializeObject<ICollection<EnVoucher>>(jsonVouchers);
            return Vouchers;
        }
    }
}