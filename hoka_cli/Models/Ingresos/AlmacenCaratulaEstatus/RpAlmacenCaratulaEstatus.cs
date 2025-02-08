using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus
{
    public class RpAlmacenCaratulaEstatus : EnWorkRepository<EnAlmacenCaratulaEstatus>
    {
        public RpAlmacenCaratulaEstatus(DbContext context) : base(context) { }
    }
}
