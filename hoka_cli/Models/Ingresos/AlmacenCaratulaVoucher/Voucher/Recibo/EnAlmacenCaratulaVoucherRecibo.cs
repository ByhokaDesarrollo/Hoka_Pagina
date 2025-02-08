using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo
{
    [Table("AlmacenCaratulaVoucherRecibo")]
    public class EnAlmacenCaratulaVoucherRecibo
    {
        [Key]
        public int AlmacenCaratulaVoucherReciboId { get; set; }
        public int AlmacenCaratulaVoucherVoucherId { get; set; }
        public Int16 Consecutivo { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteMXN { get; set; }

        [NotMapped]
        public bool B_Eliminar { get; set; }
    }
}
