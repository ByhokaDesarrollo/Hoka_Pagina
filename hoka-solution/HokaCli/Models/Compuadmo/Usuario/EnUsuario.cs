using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso;
using hoka.HokaCli.Models.Compuadmo.UsuarioPerfil;
using hoka.HokaCli.Models.Compuadmo.UsuarioRol;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Usuario
{
    [Table("VW_Usuario")]
    public class EnUsuario
    {
        [Key]
        public int UsuarioId { get; set; }

        [NotMapped]
        public EnUsuarioRol UsuarioRol { get; set; }
        public int UsuarioRolId { get; set; }

        [NotMapped]
        public EnUsuarioPerfil UsuarioPerfil { get; set; }
        public int UsuarioPerfilId { get; set; }

        [MinLength(10)]
        [MaxLength(100)]
        public string Nombre { get; set; }

        [MinLength(10)]
        [MaxLength(100)]
        public string Clave { get; set; }

        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }

        [MinLength(10)]
        [MaxLength(100)]
        public string Correo { get; set; }

        public DateTime? FechaRegistro { get; set; }
        public bool B_Activo { get; set; }

        [NotMapped]
        public string FechaRegistroFormatoFecha
        {
            get
            {
                return FechaRegistro?.ToString("yyyyMMdd") ?? "";
            }
        }

        [NotMapped]
        public EnUsuarioPermiso Permiso { get; set; }
    }
}