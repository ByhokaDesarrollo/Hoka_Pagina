using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class RpAlmacenCaratulaVenta : EnWorkRepository<EnAlmacenCaratulaVenta>
    {
        public RpAlmacenCaratulaVenta(DbContext context) : base(context) { }
    }
}
