using System.Configuration;

namespace hoka_cli.Context.Conexiones
{
    public static class SvAmbientePruebaConsultar
    {
        public static bool Consultar()
        {
            bool esAmbientePrueba = bool.Parse(ConfigurationManager.AppSettings["ENV_DB_DEV"]);
            return esAmbientePrueba;
        }
    }
}