using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.UsuarioPerfil
{
    public static class SvUsuarioPerfilIniciarRepositorio
    {
        public static RpUsuarioPerfil IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpUsuarioPerfil rpUsuarioPerfil = null)
        {
            if (rpUsuarioPerfil == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpUsuarioPerfil = new RpUsuarioPerfil(dbContext);
            return rpUsuarioPerfil;
        }
    }
}
