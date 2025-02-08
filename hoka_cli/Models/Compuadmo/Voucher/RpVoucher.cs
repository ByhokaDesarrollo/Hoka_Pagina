using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    public class RpVoucher : EnWorkRepository<EnVoucher>
    {
        public RpVoucher(DbContext context) : base(context) { }
    }
}
