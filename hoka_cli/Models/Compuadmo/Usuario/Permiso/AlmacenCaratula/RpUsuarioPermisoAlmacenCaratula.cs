using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula
{
    public class RpUsuarioPermisoAlmacenCaratula : EnWorkRepository<EnUsuarioPermisoAlmacenCaratula>
    {
        public RpUsuarioPermisoAlmacenCaratula(DbContext context) : base(context) { }
    }
}
