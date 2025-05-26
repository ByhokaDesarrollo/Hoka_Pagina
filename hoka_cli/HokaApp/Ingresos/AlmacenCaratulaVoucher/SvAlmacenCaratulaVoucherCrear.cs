using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVoucher
{
    public static class SvAlmacenCaratulaVoucherCrear
    {
        public static bool Crear(EnAlmacenCaratulaVoucher entidad)
        {
            EsAlmacenCaratulaVoucher Estructura = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = entidad
            };
            SvAlmacenCaratulaVoucher servicioMaestro = new SvAlmacenCaratulaVoucher();
            servicioMaestro.ServicioMaestro("Crear", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVoucher != null;
            return resultado;
        }
    }
}
