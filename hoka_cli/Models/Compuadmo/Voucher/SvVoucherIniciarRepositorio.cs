using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Voucher
{
    public static class SvVoucherIniciarRepositorio
    {
        public static RpVoucher IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpVoucher rpVoucher = null)
        {
            if (rpVoucher == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpVoucher = new RpVoucher(dbContext);
            return rpVoucher;
        }
    }
}
