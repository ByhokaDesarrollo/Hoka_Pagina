using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda
{
    public static class SvAlmacenCaratulaEfectivoMonedaIniciarRepositorio
    {
        public static RpAlmacenCaratulaEfectivoMoneda IniciarRepositorio(
            DBCHokaIngresos dbContext = null,
            RpAlmacenCaratulaEfectivoMoneda rpAlmacenCaratulaEfectivoMoneda = null)
        {
            if (rpAlmacenCaratulaEfectivoMoneda == null)
                if (dbContext == null)
                    dbContext = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
                rpAlmacenCaratulaEfectivoMoneda = new RpAlmacenCaratulaEfectivoMoneda(dbContext);
            return rpAlmacenCaratulaEfectivoMoneda;
        }
    }
}
