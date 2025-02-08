using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class RpAlmacenCaratulaVoucher : EnWorkRepository<EnAlmacenCaratulaVoucher>
    {
        public RpAlmacenCaratulaVoucher(DbContext context) : base(context) { }
    }
}
