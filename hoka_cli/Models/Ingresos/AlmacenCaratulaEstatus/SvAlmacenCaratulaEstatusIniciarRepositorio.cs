using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus
{
    public static class SvAlmacenCaratulaEstatusIniciarRepositorio
    {
        public static RpAlmacenCaratulaEstatus IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaEstatus rpAlmacenCaratulaEstatus = null)
        {
            if (rpAlmacenCaratulaEstatus == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaEstatus = new RpAlmacenCaratulaEstatus(dbContext);
            return rpAlmacenCaratulaEstatus;
        }
    }
}
