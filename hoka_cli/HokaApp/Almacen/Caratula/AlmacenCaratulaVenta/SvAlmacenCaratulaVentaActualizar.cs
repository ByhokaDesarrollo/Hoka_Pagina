using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaVenta entidad)
        {
            EsAlmacenCaratulaVenta Estructura = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = entidad
            };
            SvAlmacenCaratulaVenta servicioMaestro = new SvAlmacenCaratulaVenta();
            servicioMaestro.ServicioMaestro("Actualizar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVenta != null;
            return resultado;
        }
    }
}
