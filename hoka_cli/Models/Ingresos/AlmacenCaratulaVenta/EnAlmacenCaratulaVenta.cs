using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    [Table("AlmacenCaratulaVenta")]
    public class EnAlmacenCaratulaVenta
    {
        [Key]
        public int AlmacenCaratulaVentaId { get; set; }
        public int AlmacenCaratulaId { get; set; }
        public int UsuarioId { get; set; }
        public DateTime? FechaCaptura { get; set; }
        public decimal VentaSistemaTotal { get; set; }
        public decimal VentaNetaTotal { get; set; }
        public decimal VentaDiferencia { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaVentaCategoria> Categorias { get; set; }

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
