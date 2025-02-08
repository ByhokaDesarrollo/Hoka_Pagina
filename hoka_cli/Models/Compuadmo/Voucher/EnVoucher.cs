using hoka_cli.Models.Compuadmo.Moneda;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    [Table("Voucher")]
    public class EnVoucher
    {
        [Key]
        public int VoucherId { get; set; }

        [NotMapped]
        public EnMoneda Moneda { get; set; }
        public int MonedaId { get; set; }

        [MinLength(3), MaxLength(50)]
        public string Nombre { get; set; }

        [MinLength(1), MaxLength(5)]
        public string Prefijo { get; set; }

        public bool B_Activo { get; set; }
    }
}
