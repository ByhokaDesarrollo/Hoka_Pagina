using hoka_cli.Context.Conexiones;

namespace hoka_cli.Context.EntityFramework.Servicios
{
    public static class SvEstablecerConexionDBCHokaJoyeria
    {
        public static DBCHokaJoyeria EstablecerConexion()
        {
            bool ambientePrueba = SvAmbientePruebaConsultar.Consultar();
            string cadenaConexion = SvConexionHokaJoyeriaConsultar.Consultar(ambientePrueba);
            DBCHokaJoyeria Context = new DBCHokaJoyeria(cadenaConexion);
            return Context;
        }
    }
}
