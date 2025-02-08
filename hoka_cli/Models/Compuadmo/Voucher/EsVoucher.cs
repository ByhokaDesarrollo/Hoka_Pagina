using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    public class EsVoucher : EsEstructura
    {
        public EsVoucher()
        {
            Voucher = new EnVoucher();
            Vouchers = new HashSet<EnVoucher>();
        }

        public EnVoucher Voucher { get; set; }
        public ICollection<EnVoucher> Vouchers { get; set; }

        #region Propiedades
        public bool B_ConsultarMoneda { get; set; }
        #endregion
    }
}
