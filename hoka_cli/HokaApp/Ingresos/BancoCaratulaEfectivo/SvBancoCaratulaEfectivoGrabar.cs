using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.Models.Ingresos.BancoCaratulaEfectivo;

namespace hoka_cli.HokaApp.Ingresos.BancoCaratulaEfectivo
{
    public static class SvBancoCaratulaEfectivoGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaEfectivo entidad)
        {
            EsAlmacenCaratulaEfectivo Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = entidad
            };
            SvBancoCaratulaEfectivo servicioMaestro = new SvBancoCaratulaEfectivo();
            servicioMaestro.ServicioMaestro("Grabar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaEfectivo != null;
            return resultado;
        }
    }
}
