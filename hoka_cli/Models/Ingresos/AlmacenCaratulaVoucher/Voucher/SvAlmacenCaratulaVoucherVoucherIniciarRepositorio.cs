using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher
{
    public static class SvAlmacenCaratulaVoucherVoucherIniciarRepositorio
    {
        public static RpAlmacenCaratulaVoucherVoucher IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaVoucherVoucher rpAlmacenCaratulaVoucherVoucher = null)
        {
            if (rpAlmacenCaratulaVoucherVoucher == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaVoucherVoucher = new RpAlmacenCaratulaVoucherVoucher(dbContext);
            return rpAlmacenCaratulaVoucherVoucher;
        }
    }
}
