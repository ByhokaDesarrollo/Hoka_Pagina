using hoka.HokaCli.Models.Compuadmo.Voucher;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVoucher.Servicio
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