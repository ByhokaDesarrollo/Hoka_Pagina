using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaEfectivo
{
    public static class SvAlmacenCaratulaEfectivoCrear
    {
        public static bool Crear(EnAlmacenCaratulaEfectivo entidad)
        {
            EsAlmacenCaratulaEfectivo Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = entidad
            };
            SvAlmacenCaratulaEfectivo servicioMaestro = new SvAlmacenCaratulaEfectivo();
            servicioMaestro.ServicioMaestro("Crear", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaEfectivo != null;
            return resultado;
        }
    }
}
