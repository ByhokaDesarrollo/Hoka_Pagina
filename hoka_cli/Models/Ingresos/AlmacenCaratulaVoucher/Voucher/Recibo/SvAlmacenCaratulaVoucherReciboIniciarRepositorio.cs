using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo
{
    public static class SvAlmacenCaratulaVoucherReciboIniciarRepositorio
    {
        public static RpAlmacenCaratulaVoucherRecibo IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaVoucherRecibo rpAlmacenCaratulaVoucherRecibo = null)
        {
            if (rpAlmacenCaratulaVoucherRecibo == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaVoucherRecibo = new RpAlmacenCaratulaVoucherRecibo(dbContext);
            return rpAlmacenCaratulaVoucherRecibo;
        }
    }
}
