using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    public static class SvAlmacenCategoriaVentaIniciarRepositorio
    {
        public static RpAlmacenCategoriaVenta IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpAlmacenCategoriaVenta rpAlmacenCategoriaVenta = null)
        {
            if (rpAlmacenCategoriaVenta == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpAlmacenCategoriaVenta = new RpAlmacenCategoriaVenta(dbContext);
            return rpAlmacenCategoriaVenta;
        }
    }
}
