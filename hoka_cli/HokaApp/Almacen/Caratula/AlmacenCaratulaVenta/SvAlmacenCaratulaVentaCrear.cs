using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaCrear
    {
        public static bool Crear(EnAlmacenCaratulaVenta entidad)
        {
            EsAlmacenCaratulaVenta Estructura = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = entidad
            };
            SvAlmacenCaratulaVenta servicioMaestro = new SvAlmacenCaratulaVenta();
            servicioMaestro.ServicioMaestro("Crear", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVenta != null;
            return resultado;
        }
    }
}
