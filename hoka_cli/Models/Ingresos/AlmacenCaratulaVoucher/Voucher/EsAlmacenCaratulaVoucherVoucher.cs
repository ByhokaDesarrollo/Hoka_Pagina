using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher
{
    public class EsAlmacenCaratulaVoucherVoucher : EsEstructura
    {
        public EsAlmacenCaratulaVoucherVoucher()
        {
            Voucher = new EnAlmacenCaratulaVoucherVoucher();
            Vouchers = new HashSet<EnAlmacenCaratulaVoucherVoucher>();
        }

        public EnAlmacenCaratulaVoucherVoucher Voucher { get; set; }
        public ICollection<EnAlmacenCaratulaVoucherVoucher> Vouchers { get; set; }

        #region
        public bool B_ConsultarVoucher { get; set; }
        public bool B_ConsultarVoucherRecibo { get; set; }
        #endregion
    }
}
