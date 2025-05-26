using hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher.Moneda;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher
{
    [Table("AlmacenCaratulaVoucher")]
    public class EnAlmacenCaratulaVoucher
    {
        [Key]
        public int AlmacenCaratulaVoucherId { get; set; }
        public int AlmacenCaratulaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public decimal ImporteTotal { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaVoucherVoucher> Vouchers { get; set; }

        [NotMapped]
        public string FechaCapturaFormatoFecha
        {
            get
            {
                return FechaCaptura?.ToString("yyyyMMdd") ?? "";
            }
        }

    }
}
