using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Usuario
{
    public static class SvUsuarioIniciarRepositorio
    {
        public static RpUsuario IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpUsuario rpUsuario = null)
        {
            if (rpUsuario == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpUsuario = new RpUsuario(dbContext);
            return rpUsuario;
        }
    }
}
