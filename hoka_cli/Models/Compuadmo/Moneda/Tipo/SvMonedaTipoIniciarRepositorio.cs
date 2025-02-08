using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Moneda.Tipo
{
    public static class SvMonedaTipoIniciarRepositorio
    {
        public static RpMonedaTipo IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpMonedaTipo rpMonedaTipo = null)
        {
            if (rpMonedaTipo == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpMonedaTipo = new RpMonedaTipo(dbContext);
            return rpMonedaTipo;
        }
    }
}
