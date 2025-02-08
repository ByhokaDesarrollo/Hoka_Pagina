using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Moneda.Tipo
{
    [Table("MonedaTipo")]
    public class EnMonedaTipo
    {
        [Key]
        public int MonedaTipoId { get; set; }

        [MinLength(3), MaxLength(50)]
        public string Nombre { get; set; }

        [MinLength(1), MaxLength(5)]
        public string Prefijo { get; set; }

        public bool B_Activo { get; set; }
    }
}
