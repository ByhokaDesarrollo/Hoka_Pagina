using hoka_cli.Models.Compuadmo.Almacen;
using hoka_cli.Models.Compuadmo.Usuario;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratula
{
    [Table("AlmacenCaratula")]
    public class EnAlmacenCaratula
    {
        [Key]
        public int AlmacenCaratulaId { get; set; }

        [NotMapped]
        public EnAlmacen Almacen { get; set; }
        public int AlmacenId { get; set; }

        [NotMapped]
        public EnUsuario Usuario { get; set; }
        public int UsuarioId { get; set; }

        [NotMapped]
        public EnAlmacenCaratulaEstatus Estatus { get; set; }
        public int AlmacenCaratulaEstatusId { get; set; }

        public string Folio { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public byte EstatusCaratulaVenta { get; set; }
        public byte EstatusCaratulaEfectivo { get; set; }
        public byte EstatusCaratulaVoucher { get; set; }
        public bool B_ConsultarCaratula { get; set; }

        [NotMapped]
        public EnAlmacenCaratulaVenta CaratulaVenta { get; set; }

        [NotMapped]
        public EnAlmacenCaratulaEfectivo CaratulaEfectivo { get; set; }

        [NotMapped]
        public EnAlmacenCaratulaVoucher CaratulaVoucher { get; set; }

        [NotMapped]
        public ICollection<EnAlmacenCaratulaReporte> CaratulaReporte { get; set; }

        [NotMapped]
        public string FechaRegistroFormatoFecha
        {
            get
            {
                return FechaRegistro?.ToString("yyyyMMdd") ?? "";
            }
        }
    }
}
