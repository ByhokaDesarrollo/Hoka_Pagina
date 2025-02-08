using hoka.HokaCli.Models.Compuadmo.UsuarioRol;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.UsuarioPerfil
{
    [Table("VW_UsuarioPerfil")]
    public class EnUsuarioPerfil
    {
        [Key]
        public int UsuarioPerfilId { get; set; }

        public EnUsuarioRol UsuarioRol { get; set; }
        public int UsuarioRolId { get; set; }

        public string Nombre { get; set; }
        public string Prefijo { get; set; }
        public bool B_Activo { get; set; }
    }
}