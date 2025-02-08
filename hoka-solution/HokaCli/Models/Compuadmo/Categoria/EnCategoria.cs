using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Categoria
{
    [Table("VW_Categoria")]
    public class EnCategoria
    {
        [Key]
        public int CategoriaId { get; set; }
        public string Nombre { get; set; }
        public string DepartamentoPrefijo { get; set; }
    }
}
