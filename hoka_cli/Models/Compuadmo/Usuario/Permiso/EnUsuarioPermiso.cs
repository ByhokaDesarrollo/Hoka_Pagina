using hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Usuario.Permiso
{
    public class EnUsuarioPermiso
    {
        public ICollection<EnUsuarioPermisoAlmacenCaratula> PermisosAlmacenCaratula { get; set; }
    }
}
