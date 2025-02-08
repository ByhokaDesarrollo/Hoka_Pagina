using System.Configuration;

namespace hoka_cli.Context.Conexiones
{
    public static class SvConexionHokaIngresosConsultar
    {
        public static string Consultar(bool ambientePrueba)
        {
            string cadenaConexion = ambientePrueba
                ? ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaIngresos"].ToString()
                : ConfigurationManager.ConnectionStrings["CadenaConexionHokaIngresos"].ToString();
            return cadenaConexion;
        }
    }
}