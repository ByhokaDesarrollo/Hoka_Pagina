using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher
{
    public class RpAlmacenCaratulaVoucherVoucher : EnWorkRepository<EnAlmacenCaratulaVoucherVoucher>
    {
        public RpAlmacenCaratulaVoucherVoucher(DbContext context) : base(context) { }
    }
}
