using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.UsuarioPerfil
{
    public class RpUsuarioPerfil : EnWorkRepository<EnUsuarioPerfil>
    {
        public RpUsuarioPerfil(DbContext context) : base(context) { }
    }
}
