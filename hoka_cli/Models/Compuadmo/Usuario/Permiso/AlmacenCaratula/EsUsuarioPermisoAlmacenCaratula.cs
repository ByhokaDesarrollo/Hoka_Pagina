using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula
{
    public class EsUsuarioPermisoAlmacenCaratula : EsEstructura
    {
        public EsUsuarioPermisoAlmacenCaratula()
        {
            Permiso = new EnUsuarioPermisoAlmacenCaratula();
            Permisos = new HashSet<EnUsuarioPermisoAlmacenCaratula>();
        }

        public EnUsuarioPermisoAlmacenCaratula Permiso { get; set; }
        public ICollection<EnUsuarioPermisoAlmacenCaratula> Permisos { get; set; }
    }
}
