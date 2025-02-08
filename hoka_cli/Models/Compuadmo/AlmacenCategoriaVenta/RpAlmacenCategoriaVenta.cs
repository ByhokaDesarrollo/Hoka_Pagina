using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    public class RpAlmacenCategoriaVenta : EnWorkRepository<EnAlmacenCategoriaVenta>
    {
        public RpAlmacenCategoriaVenta(DbContext context) : base(context) { }
    }
}
