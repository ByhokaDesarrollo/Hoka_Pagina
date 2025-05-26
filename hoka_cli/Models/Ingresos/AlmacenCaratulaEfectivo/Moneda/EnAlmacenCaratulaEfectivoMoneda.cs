using hoka_cli.Models.Compuadmo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda
{
    [Table("AlmacenCaratulaEfectivoMoneda")]
    public class EnAlmacenCaratulaEfectivoMoneda
    {
        [Key]
        public int AlmacenCaratulaEfectivoMonedaId { get; set; }
        public int AlmacenCaratulaEfectivoId { get; set; }

        [NotMapped]
        public EnMoneda Moneda { get; set; }
        public int MonedaId { get; set; }

        public decimal TipoCambio { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal ImporteTotalMXN { get; set; }
        public decimal ImporteTotalBanco { get; set; }
        public decimal ImporteTotalMXNBanco { get; set; }
        public decimal ImporteTotalDiferencia { get; set; }
        public decimal ImporteTotalMXNDiferencia { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaEfectivoDenominacion> Denominaciones { get; set; }
    }
}
