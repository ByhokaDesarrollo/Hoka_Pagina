using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public static class SvAlmacenCaratulaVoucherIniciarRepositorio
    {
        public static RpAlmacenCaratulaVoucher IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaVoucher rpAlmacenCaratulaReporteVoucher = null)
        {
            if (rpAlmacenCaratulaReporteVoucher == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaReporteVoucher = new RpAlmacenCaratulaVoucher(dbContext);
            return rpAlmacenCaratulaReporteVoucher;
        }
    }
}
