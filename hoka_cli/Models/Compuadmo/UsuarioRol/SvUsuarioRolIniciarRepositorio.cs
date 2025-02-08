using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.UsuarioRol
{
    public static class SvUsuarioRolIniciarRepositorio
    {
        public static RpUsuarioRol IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpUsuarioRol rpUsuarioRol = null)
        {
            if (rpUsuarioRol == null)
                if (dbContext == null) 
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpUsuarioRol = new RpUsuarioRol(dbContext);
            return rpUsuarioRol;
        }
    }
}
