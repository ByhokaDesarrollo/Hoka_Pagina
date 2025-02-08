using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.UsuarioRol
{
    public class EsUsuarioRol : EsEstructura
    {
        public EsUsuarioRol()
        {
            UsuarioRol = new EnUsuarioRol();
            UsuarioRoles = new HashSet<EnUsuarioRol>();
        }

        public EnUsuarioRol UsuarioRol { get; set; }
        public ICollection<EnUsuarioRol> UsuarioRoles { get; set; }
    }
}
