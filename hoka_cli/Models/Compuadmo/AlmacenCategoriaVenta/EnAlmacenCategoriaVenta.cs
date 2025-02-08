using hoka_cli.Models.Compuadmo.Catalogo.Almacen;
using hoka_cli.Models.Compuadmo.Categoria;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    [Table("AlmacenCategoriaVenta")]
    public class EnAlmacenCategoriaVenta
    {
        [Key]
        public int AlmacenCategoriaVentaId { get; set; }

        [NotMapped]
        public EnCatalogoAlmacen Almacen { get; set; }
        public int AlmacenId { get; set; }

        [NotMapped]
        public EnCategoria Categoria { get; set; }
        public int CategoriaId { get; set; }

        public DateTime? Fecha { get; set; }
        public decimal ImporteTotal { get; set; }

        [NotMapped]
        public string FechaSoloFecha
        {
            get
            {
                return Fecha?.ToString("yyyyMMdd") ?? "NULL";
            }
        }
    }
}
