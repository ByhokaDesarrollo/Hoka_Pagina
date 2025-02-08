using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Models.Compuadmo.UsuarioRol
{
    [Table("VW_UsuarioRol")]
    public class EnUsuarioRol
    {
        [Key]
        public int UsuarioRolId { get; set; }
        public string Nombre { get; set; }
        public string Prefijo { get; set; }
        public bool B_Activo { get; set; }
    }
}