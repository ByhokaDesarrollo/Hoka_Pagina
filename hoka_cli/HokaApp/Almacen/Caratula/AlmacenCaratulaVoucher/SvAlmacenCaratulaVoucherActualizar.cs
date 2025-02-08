using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVoucher
{
    public static class SvAlmacenCaratulaVoucherActualizar
    {
        public static bool Actualizar(EnAlmacenCaratulaVoucher entidad)
        {
            EsAlmacenCaratulaVoucher Estructura = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = entidad
            };
            SvAlmacenCaratulaVoucher servicioMaestro = new SvAlmacenCaratulaVoucher();
            servicioMaestro.ServicioMaestro("Actualizar", Estructura);
            bool resultado = servicioMaestro.Estructura.CaratulaVoucher != null;
            return resultado;
        }
    }
}
