using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria
{
    public static class SvAlmacenCaratulaVentaCategoriaIniciarRepositorio
    {
        public static RpAlmacenCaratulaVentaCategoria IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaVentaCategoria rpAlmacenCaratulaVentaCategoria = null)
        {
            if (rpAlmacenCaratulaVentaCategoria == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaVentaCategoria = new RpAlmacenCaratulaVentaCategoria(dbContext);
            return rpAlmacenCaratulaVentaCategoria;
        }
    }
}
