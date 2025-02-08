using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Catalogo.Almacen
{
    [Table("VW_CatalogoAlmacen")]
    public class EnCatalogoAlmacen
    {
        [Key]
        public int AlmacenId { get; set; }

        [MinLength(3), MaxLength(50)]
        public string Nombre { get; set; }

        public string Prefijo { get; set; }
        public string SucursalPrefijo { get; set; }
        public string IPServer { get; set; }
        public string B_TieneSistema { get; set; }
    }
}
