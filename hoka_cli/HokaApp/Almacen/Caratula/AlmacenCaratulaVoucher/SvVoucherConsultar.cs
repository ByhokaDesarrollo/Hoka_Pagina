using hoka_cli.Models.Compuadmo.Voucher;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher
{
    public static class SvVoucherConsultar
    {
        public static string Consultar()
        {
            SvVoucher svVoucher = new SvVoucher();
            EsVoucher esVoucher = new EsVoucher()
            {
                B_ConsultarMoneda = true
            };
            svVoucher.ServicioMaestro("Consultar", esVoucher);
            ICollection<EnVoucher> Vouchers = svVoucher.Estructura.Vouchers;
            string jsonVouchers = JsonConvert.SerializeObject(Vouchers);
            return jsonVouchers;
        }
    }
}
