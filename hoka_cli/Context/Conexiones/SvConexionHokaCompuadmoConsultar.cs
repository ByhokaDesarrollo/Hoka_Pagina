using System.Configuration;

namespace hoka_cli.Context.Conexiones
{
    public static class SvConexionHokaCompuadmoConsultar
    {
        public static string Consultar(bool ambientePrueba)
        {
            string cadenaConexion = ambientePrueba
                ? ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString()
                : ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
            return cadenaConexion;
        }
    }
}