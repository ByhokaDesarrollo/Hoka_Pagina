using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaVenta entidad)
        {
            EsAlmacenCaratulaVenta Estructura = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = entidad
            };
            SvAlmacenCaratulaVenta servicioMaestro = new SvAlmacenCaratulaVenta();
            servicioMaestro.ServicioMaestro("Grabar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVenta != null;
            return resultado;
        }
    }
}
