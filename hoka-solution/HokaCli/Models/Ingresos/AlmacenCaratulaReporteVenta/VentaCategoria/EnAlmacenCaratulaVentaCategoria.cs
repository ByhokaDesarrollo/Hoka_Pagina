using hoka.HokaCli.Models.Compuadmo.Categoria;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaVenta.VentaCategoria
{
    [Table("AlmacenCaratulaVentaCategoria")]
    public class EnAlmacenCaratulaVentaCategoria
    {
        [Key]
        public int AlmacenCaratulaVentaCategoriaId { get; set; }
        public int AlmacenCaratulaVentaId { get; set; }

        public EnCategoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        public decimal VentaSistema { get; set; }
        public decimal VentaNeta { get; set; }
        public decimal Comision { get; set; }
    }
}
