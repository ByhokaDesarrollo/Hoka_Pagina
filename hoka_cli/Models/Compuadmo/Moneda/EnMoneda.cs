using hoka_cli.Models.Compuadmo.Moneda.Denominacion;
using hoka_cli.Models.Compuadmo.Moneda.Tipo;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    [Table("Moneda")]
    public class EnMoneda
    {
        [Key]
        public int MonedaPK { get; set; }
        public int MonedaId { get; set; }

        [NotMapped]
        public EnMonedaTipo MonedaTipo { get; set; }
        public int MonedaTipoId { get; set; }

        [MinLength(3), MaxLength(30)]
        public string Nombre { get; set; }

        public decimal Valor { get; set; }
        public bool B_Activo { get; set; }

        [NotMapped]
        public ICollection<EnMonedaDenominacion> Denominaciones { get; set; }
    }
}
