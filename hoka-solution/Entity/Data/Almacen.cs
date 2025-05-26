using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.Entity
{
    [Table("almacenes")] // Nombre de la tabla en la base de datos
    public class Almacen
    {
        [Key]
        [Column("Almacen")] // Ajusta según el nombre real de tu columna
        public int IdAlmacen { get; set; }

        [Column("Nombre")] // Nombre de la columna en la BD
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

    }
}