using hoka.HokaCli.Models.Compuadmo.Moneda.Denominacion;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion
{
    [Table("AlmacenCaratulaEfectivoDenominacion")]
    public class EnAlmacenCaratulaEfectivoDenominacion
    {
        [Key]
        public int AlmacenCaratulaEfectivoDenominacionId { get; set; }
        public int AlmacenCaratulaEfectivoMonedaId { get; set; }

        [NotMapped]
        public EnMonedaDenominacion MonedaDenominacion { get; set; }
        public int MonedaDenominacionId { get; set; }

        public int Cantidad { get; set; }
        public decimal Importe { get; set; }
        public decimal ImporteMXN { get; set; }
    }
}
