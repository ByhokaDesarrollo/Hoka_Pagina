using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public class RpUsuario : EnWorkRepository<EnUsuario>
    {
        public RpUsuario(DbContext context) : base(context) { }
    }
}
