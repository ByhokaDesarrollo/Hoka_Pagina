using hoka_cli.Models.Ingresos.AlmacenCaratula;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratula
{
    public static class SvAlmacenCaratulaDesbloquear
    {
        public static bool Desbloquear(EnAlmacenCaratula entidad)
        {
            EsAlmacenCaratula Estructura = new EsAlmacenCaratula()
            {
                Caratula = entidad
            };
            SvAlmacenCaratula servicioMaestro = new SvAlmacenCaratula();
            servicioMaestro.ServicioMaestro("Desbloquear", Estructura);
            bool resultado = servicioMaestro.Estructura.Caratula != null;
            return resultado;
        }
    }
}
