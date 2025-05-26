using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVoucher.Moneda.Recibo
{
    [Table("AlmacenCaratulaVoucherRecibo")]
    public class EnAlmacenCaratulaVoucherRecibo
    {
        [Key]
        public int AlmacenCaratulaVoucherReciboId { get; set; }
        public int AlmacenCaratulaVoucherVoucherId { get; set; }
        public int Consecutivo { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteMXN { get; set; }

        [NotMapped]
        public bool B_Eliminar { get; set; }

        [NotMapped] public string Archivo { get; set; } // Base64
        [NotMapped] public string ArchivoNombre { get; set; }
        [NotMapped] public string ArchivoTipo { get; set; }
        [NotMapped] public string ArchivoNombreConsulta
        {
            get
            {
                return $"{AlmacenCaratulaVoucherReciboId}_{ArchivoNombre}";
            }
        }
    }
}
