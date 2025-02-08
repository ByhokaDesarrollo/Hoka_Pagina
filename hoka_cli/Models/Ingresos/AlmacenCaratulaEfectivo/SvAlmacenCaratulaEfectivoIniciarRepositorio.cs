using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAlmacenCaratulaEfectivoIniciarRepositorio
    {
        public static RpAlmacenCaratulaEfectivo IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaEfectivo rpAlmacenCaratulaEfectivo = null)
        {
            if (rpAlmacenCaratulaEfectivo == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaEfectivo = new RpAlmacenCaratulaEfectivo(dbContext);
            return rpAlmacenCaratulaEfectivo;
        }
    }
}
