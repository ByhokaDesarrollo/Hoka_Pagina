using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Compuadmo.Moneda.Denominacion
{
    public static class SvMonedaDenominacionIniciarRepositorio
    {
        public static RpMonedaDenominacion IniciarRepositorio(
            DBCHokaCompuadmo dbContext = null,
            RpMonedaDenominacion rpMonedaDenominacion = null)
        {
            if (rpMonedaDenominacion == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
                rpMonedaDenominacion = new RpMonedaDenominacion(dbContext);
            return rpMonedaDenominacion;
        }
    }
}
