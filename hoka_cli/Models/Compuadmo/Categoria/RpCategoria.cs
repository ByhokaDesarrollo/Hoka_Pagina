using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Categoria
{
    public class RpCategoria : EnWorkRepository<EnCategoria>
    {
        public RpCategoria(DbContext context) : base(context) { }
    }
}
