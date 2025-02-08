using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    [Table("AlmacenCaratulaEfectivo")]
    public class EnAlmacenCaratulaEfectivo
    {
        [Key]
        public int AlmacenCaratulaEfectivoId { get; set; }
        public int AlmacenCaratulaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public decimal ImporteTotal { get; set; }
        public decimal ImporteTotalBanco { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaEfectivoMoneda> Monedas { get; set; }

        [NotMapped]
        public string FechaCapturaFormatoFecha
        {
            get
            {
                return FechaCaptura?.ToString("yyyyMMdd") ?? "";
            }
        }
    }
}
