using hoka.HokaCli.Models.Compuadmo.Voucher;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher.Moneda.Recibo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher.Moneda
{
    [Table("AlmacenCaratulaVoucherVoucher")]
    public class EnAlmacenCaratulaVoucherVoucher
    {
        [Key]
        public int AlmacenCaratulaVoucherVoucherId { get; set; }
        public int AlmacenCaratulaVoucherId { get; set; }

        [NotMapped]
        public EnVoucher Voucher { get; set; }
        public int VoucherId { get; set; }

        public decimal TipoCambio { get; set; } 
        public decimal ImporteTotal { get; set; }
        public decimal ImporteTotalMXN { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaVoucherRecibo> Recibos { get; set; }
    }
}
