using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaIniciarRepositorio
    {
        public static RpAlmacenCaratulaVenta IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaVenta rpAlmacenCaratulaVenta = null)
        {
            if (rpAlmacenCaratulaVenta == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaVenta = new RpAlmacenCaratulaVenta(dbContext);
            return rpAlmacenCaratulaVenta;
        }
    }
}
