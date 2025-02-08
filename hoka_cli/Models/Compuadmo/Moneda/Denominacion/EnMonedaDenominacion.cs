using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Compuadmo.Moneda.Denominacion
{
    [Table("MonedaDenominacion")]
    public class EnMonedaDenominacion
    {
        [Key]
        public int MonedaDenominacionId { get; set; }
        public int MonedaId { get; set; }
        public int Denominacion { get; set; }
        public bool B_Activo { get; set; }
    }
}
