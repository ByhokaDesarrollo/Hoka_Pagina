using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Almacen
{
    public static class SvAlmacenIniciarRepositorio
    {
        public static RpAlmacen IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpAlmacen rpAlmacen = null)
        {
            if (rpAlmacen == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpAlmacen = new RpAlmacen(dbContext);
            return rpAlmacen;
        }
    }
}
