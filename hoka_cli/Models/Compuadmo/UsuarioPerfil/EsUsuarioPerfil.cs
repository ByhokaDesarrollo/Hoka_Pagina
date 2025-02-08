using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.UsuarioPerfil
{
    public class EsUsuarioPerfil : EsEstructura
    {
        public EsUsuarioPerfil()
        {
            UsuarioPerfil = new EnUsuarioPerfil();
            UsuarioPerfiles = new HashSet<EnUsuarioPerfil>();
        }

        public EnUsuarioPerfil UsuarioPerfil { get; set; }
        public ICollection<EnUsuarioPerfil> UsuarioPerfiles { get; set; }

        #region Propiedades
        public bool B_ConsultarRol { get; set; }
        #endregion
    }
}
