using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo
{
    public class RpAlmacenCaratulaVoucherRecibo : EnWorkRepository<EnAlmacenCaratulaVoucherRecibo>
    {
        public RpAlmacenCaratulaVoucherRecibo(DbContext context) : base(context) { }
    }
}
