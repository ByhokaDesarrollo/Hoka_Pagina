using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Ingresos.AlmacenCaratulaEstatus
{
    [Table("AlmacenCaratulaReporteEstatus")]
    public class EnAlmacenCaratulaEstatus
    {
        [Key]
        public int AlmacenCaratulaReporteEstatusId { get; set; }
        public string Nombre { get; set; }
        public string Prefijo { get; set; }
        public string VistaTexto { get; set; }
        public bool B_Activo { get; set; }
    }
}
