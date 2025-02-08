using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion
{
    public static class SvAlmacenCaratulaEfectivoDenominacionIniciarRepositorio
    {
        public static RpAlmacenCaratulaEfectivoDenominacion IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaEfectivoDenominacion rpAlmacenCaratulaEfectivoDenominacion = null)
        {
            if (rpAlmacenCaratulaEfectivoDenominacion == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaEfectivoDenominacion = new RpAlmacenCaratulaEfectivoDenominacion(dbContext);
            return rpAlmacenCaratulaEfectivoDenominacion;
        }
    }
}
