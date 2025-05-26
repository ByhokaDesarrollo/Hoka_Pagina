using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;
using Newtonsoft.Json;
using System.Linq;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaConsultar
    {
        public static string Consultar(int almacenCaratulaId)
        {
            EsAlmacenCaratulaVenta Estructura = new EsAlmacenCaratulaVenta()
            {
                CaratulaVenta = new EnAlmacenCaratulaVenta()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarCategoria = true
            };
            SvAlmacenCaratulaVenta servicioMaestro = new SvAlmacenCaratulaVenta();
            servicioMaestro.ServicioMaestro("Consultar", Estructura);
            EnAlmacenCaratulaVenta AlmacenCaratulaVenta =
                servicioMaestro.Estructura.CaratulasVenta.Single();
            string jsonAlmacenCaratulaVenta = JsonConvert.SerializeObject(AlmacenCaratulaVenta);
            return jsonAlmacenCaratulaVenta;
        }
    }
}
