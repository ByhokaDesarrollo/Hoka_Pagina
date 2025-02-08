using hoka_cli.Models.Compuadmo.Categoria;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria
{
    [Table("AlmacenCaratulaVentaCategoria")]
    public class EnAlmacenCaratulaVentaCategoria
    {
        [Key]
        public int AlmacenCaratulaVentaCategoriaId { get; set; }
        public int AlmacenCaratulaVentaId { get; set; }

        [NotMapped]
        public EnCategoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        public decimal VentaSistema { get; set; }
        public decimal VentaNeta { get; set; }
        public decimal Comision { get; set; }
    }
}
