using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula
{
    public static class SvUsuarioPermisoAlmacenCaratulaIniciarRepositorio
    {
        public static RpUsuarioPermisoAlmacenCaratula IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpUsuarioPermisoAlmacenCaratula rpUsuarioPermisoAlmacenCaratula = null)
        {
            if (rpUsuarioPermisoAlmacenCaratula == null)
                if (dbContext == null) 
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpUsuarioPermisoAlmacenCaratula = new RpUsuarioPermisoAlmacenCaratula(dbContext);
            return rpUsuarioPermisoAlmacenCaratula;
        }
    }
}
