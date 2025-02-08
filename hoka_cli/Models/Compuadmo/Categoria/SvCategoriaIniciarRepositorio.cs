using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Categoria
{
    public static class SvCategoriaIniciarRepositorio
    {
        public static RpCategoria IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpCategoria rpCategoria = null)
        {
            if (rpCategoria == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpCategoria = new RpCategoria(dbContext);
            return rpCategoria;
        }
    }
}
