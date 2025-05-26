using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAlmacenCaratulaEfectivoGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaEfectivo entidad)
        {
            EsAlmacenCaratulaEfectivo Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = entidad
            };
            SvAlmacenCaratulaEfectivo servicioMaestro = new SvAlmacenCaratulaEfectivo();
            servicioMaestro.ServicioMaestro("Grabar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaEfectivo != null;
            return resultado;
        }
    }
}
