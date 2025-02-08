using hoka_cli.Context.Conexiones;

namespace hoka_cli.Context.EntityFramework.Servicios
{
    public static class SvEstablecerConexionDBCHokaCompuadmo
    {
        public static DBCHokaCompuadmo EstablecerConexion()
        {
            bool ambientePrueba = SvAmbientePruebaConsultar.Consultar();
            string cadenaConexion = SvConexionHokaCompuadmoConsultar.Consultar(ambientePrueba);
            DBCHokaCompuadmo Context = new DBCHokaCompuadmo(cadenaConexion);
            return Context;
        }
    }
}
