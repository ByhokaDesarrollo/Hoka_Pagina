using hoka_cli.Context.Conexiones;

namespace hoka_cli.Context.EntityFramework.Servicios
{
    public static class SvEstablecerConexionDBCHokaIngresos
    {
        public static DBCHokaIngresos EstablecerConexion()
        {
            bool esAmbientePrueba = SvAmbientePruebaConsultar.Consultar();
            string cadenaConexion = SvConexionHokaIngresosConsultar.Consultar(esAmbientePrueba);
            DBCHokaIngresos Context = new DBCHokaIngresos(cadenaConexion);
            return Context;
        }
    }
}
