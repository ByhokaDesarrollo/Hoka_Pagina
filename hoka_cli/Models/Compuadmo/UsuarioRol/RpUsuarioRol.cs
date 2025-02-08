using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.UsuarioRol
{
    public class RpUsuarioRol : EnWorkRepository<EnUsuarioRol>
    {
        public RpUsuarioRol(DbContext context) : base(context) { }
    }
}
