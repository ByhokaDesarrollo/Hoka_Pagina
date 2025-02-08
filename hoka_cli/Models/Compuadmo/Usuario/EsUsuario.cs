using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public class EsUsuario : EsEstructura
    {
        public EsUsuario()
        {
            Usuario = new EnUsuario();
            Usuarios = new HashSet<EnUsuario>();
        }

        public EnUsuario Usuario { get; set; }
        public ICollection<EnUsuario> Usuarios { get; set; }

        #region Propiedades
        public bool B_ConsultarRol { get; set; }
        public bool B_ConsultarPerfil { get; set; }
        public bool B_ConsultarPermisoAlmacenCaratula { get; set; }
        #endregion
    }
}
