using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class RpAlmacenCaratulaEfectivo : EnWorkRepository<EnAlmacenCaratulaEfectivo>
    {
        public RpAlmacenCaratulaEfectivo(DbContext context) : base(context) { }
    }
}
