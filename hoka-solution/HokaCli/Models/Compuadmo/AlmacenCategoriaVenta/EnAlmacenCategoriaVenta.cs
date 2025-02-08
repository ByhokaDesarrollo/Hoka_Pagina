using hoka.HokaCli.Models.Compuadmo.Catalogo.Almacen;
using hoka.HokaCli.Models.Compuadmo.Categoria;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.AlmacenCategoriaVenta
{
    [Table("AlmacenCategoriaVenta")]
    public class EnAlmacenCategoriaVenta
    {
        [Key]
        public int AlmacenCategoriaVentaId { get; set; }

        public EnCatalogoAlmacen Almacen { get; set; }
        public int AlmacenId { get; set; }

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
