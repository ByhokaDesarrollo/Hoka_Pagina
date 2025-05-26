using hoka_cli.Models.Ingresos.AlmacenCaratula;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratula
{
    public static class SvAlmacenCaratulaGrabar
    {
        public static bool Grabar(EnAlmacenCaratula entidad)
        {
            EsAlmacenCaratula Estructura = new EsAlmacenCaratula()
            {
                Caratula = entidad
            };
            SvAlmacenCaratula servicioMaestro = new SvAlmacenCaratula();
            servicioMaestro.ServicioMaestro("Grabar", Estructura);
            bool resultado = servicioMaestro.Estructura.Caratula != null;
            return resultado;
        }
    }
}
