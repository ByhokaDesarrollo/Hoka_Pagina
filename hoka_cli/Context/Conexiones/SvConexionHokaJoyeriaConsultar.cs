using System.Configuration;

namespace hoka_cli.Context.Conexiones
{
    public static class SvConexionHokaJoyeriaConsultar
    {
        public static string Consultar(bool ambientePrueba)
        {
            string cadenaConexion = ambientePrueba
                ? ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString()
                : ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();
            return cadenaConexion;
        }
    }
}