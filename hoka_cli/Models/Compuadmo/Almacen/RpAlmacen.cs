using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Almacen
{
    public class RpAlmacen : EnWorkRepository<EnAlmacen>
    {
        public RpAlmacen(DbContext context) : base(context) { }
    }
}
