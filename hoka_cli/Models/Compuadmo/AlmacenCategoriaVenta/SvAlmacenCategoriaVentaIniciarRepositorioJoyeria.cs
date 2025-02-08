using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    public static class SvAlmacenCategoriaVentaIniciarRepositorioJoyeria
    {
        public static RpAlmacenCategoriaVenta IniciarRepositorio(
            DBCHokaJoyeria dbContext = null,
            RpAlmacenCategoriaVenta rpAlmacenCategoriaVenta = null)
        {
            if (rpAlmacenCategoriaVenta == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaJoyeria.EstablecerConexion();
                rpAlmacenCategoriaVenta = new RpAlmacenCategoriaVenta(dbContext);
            return rpAlmacenCategoriaVenta;
        }
    }
}
