using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo
{
    public class EsAlmacenCaratulaVoucherRecibo : EsEstructura
    {
        public EsAlmacenCaratulaVoucherRecibo()
        {
            Recibo = new EnAlmacenCaratulaVoucherRecibo();
            Recibos = new HashSet<EnAlmacenCaratulaVoucherRecibo>();
        }

        public EnAlmacenCaratulaVoucherRecibo Recibo { get; set; }
        public ICollection<EnAlmacenCaratulaVoucherRecibo> Recibos { get; set; }
        public bool ConsultarArchivo { get; set; }
    }
}
