using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Catalogo.Almacen
{
    public class RpCatalogoAlmacen : EnWorkRepository<EnCatalogoAlmacen>
    {
        public RpCatalogoAlmacen(DbContext context) : base(context) { }
    }
}
