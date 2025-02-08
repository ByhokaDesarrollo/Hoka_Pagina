using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher
{
    public static class SvAlmacenCaratulaVoucherGrabar
    {
        public static bool Grabar(EnAlmacenCaratulaVoucher entidad)
        {
            EsAlmacenCaratulaVoucher Estructura = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = entidad
            };
            SvAlmacenCaratulaVoucher servicioMaestro = new SvAlmacenCaratulaVoucher();
            servicioMaestro.ServicioMaestro("Grabar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVoucher != null;
            return resultado;
        }
    }
}
