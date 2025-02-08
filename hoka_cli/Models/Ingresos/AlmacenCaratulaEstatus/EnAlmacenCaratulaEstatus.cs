using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus
{
    [Table("AlmacenCaratulaEstatus")]
    public class EnAlmacenCaratulaEstatus
    {
        [Key]
        public int AlmacenCaratulaEstatusId { get; set; }
        public string Nombre { get; set; }
        public string Prefijo { get; set; }
        public string VistaTexto { get; set; }
        public bool B_Activo { get; set; }
    }
}
