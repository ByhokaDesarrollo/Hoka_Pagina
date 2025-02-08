using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    public static class SvMonedaIniciarRepositorio
    {
        public static RpMoneda IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpMoneda rpMoneda = null)
        {
            if (rpMoneda == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpMoneda = new RpMoneda(dbContext);
            return rpMoneda;
        }
    }
}
