using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratula
{
    public static class SvAlmacenCaratulaIniciarRepositorio
    {
        public static RpAlmacenCaratula IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratula rpAlmacenCaratula = null)
        {
            if (rpAlmacenCaratula == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratula = new RpAlmacenCaratula(dbContext);
            return rpAlmacenCaratula;
        }
    }
}
