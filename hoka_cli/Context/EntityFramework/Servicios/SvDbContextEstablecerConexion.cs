using hoka_cli.Context.Conexiones;
using System.Data.Entity;

namespace hoka_cli.Context.EntityFramework.Servicios
{
    public static class SvDbContextEstablecerConexion
    {
        public static DbContext EstablecerConexion(
            string dbckNombre,
            string cadenaConexion = null,
            DbContext context = null)
        {
            if (context == null)
            {
                bool ambientePrueba = SvAmbientePruebaConsultar.Consultar();
                if (string.IsNullOrEmpty(cadenaConexion))
                {
                    switch (dbckNombre)
                    {
                        case "DBCHokaCompuadmo":
                            cadenaConexion = SvConexionHokaCompuadmoConsultar
                                .Consultar(ambientePrueba);
                            break;
                        case "DBCHokaIngresos":
                            cadenaConexion = SvConexionHokaIngresosConsultar
                                .Consultar(ambientePrueba);
                            break;
                        case "DBCHokaJoyeria":
                            cadenaConexion = SvConexionHokaJoyeriaConsultar
                                .Consultar(ambientePrueba);
                            break;
                        default:
                            break;
                    }
                }
                DbContext Context = new DbContext(cadenaConexion);
                return Context;
            }
            return context;
        }
    }
}
