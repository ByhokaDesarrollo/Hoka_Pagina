using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;
using Newtonsoft.Json;
using System.Linq;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVoucher
{
    public static class SvAlmacenCaratulaVoucherConsultar
    {
        public static string Consultar(int almacenCaratulaId)
        {
            EsAlmacenCaratulaVoucher Estructura = new EsAlmacenCaratulaVoucher()
            {
                CaratulaVoucher = new EnAlmacenCaratulaVoucher()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarVoucher = true
            };
            SvAlmacenCaratulaVoucher servicioMaestro = new SvAlmacenCaratulaVoucher();
            servicioMaestro.ServicioMaestro("Consultar", Estructura);
            EnAlmacenCaratulaVoucher AlmacenCaratulaVoucher =
                servicioMaestro.Estructura.CaratulasVoucher.Single();
            string jsonAlmacenCaratulaVoucher = JsonConvert.SerializeObject(AlmacenCaratulaVoucher);
            return jsonAlmacenCaratulaVoucher;
        }
    }
}
