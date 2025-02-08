using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Catalogo.Almacen
{
    public static class SvCatalogoAlmacenIniciarRepositorio
    {
        public static RpCatalogoAlmacen IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpCatalogoAlmacen rpCatalogoAlmacen = null)
        {
            if (rpCatalogoAlmacen == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpCatalogoAlmacen = new RpCatalogoAlmacen(dbContext);
            return rpCatalogoAlmacen;
        }
    }
}
