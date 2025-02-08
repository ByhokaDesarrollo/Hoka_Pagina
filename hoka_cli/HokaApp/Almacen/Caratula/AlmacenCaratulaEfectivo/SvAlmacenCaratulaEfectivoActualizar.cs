using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo
{
    public static class SvAlmacenCaratulaEfectivoActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaEfectivo entidad)
        {
            EsAlmacenCaratulaEfectivo Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = entidad
            };
            SvAlmacenCaratulaEfectivo servicioMaestro = new SvAlmacenCaratulaEfectivo();
            servicioMaestro.ServicioMaestro("Actualizar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaEfectivo != null;
            return resultado;
        }
    }
}
