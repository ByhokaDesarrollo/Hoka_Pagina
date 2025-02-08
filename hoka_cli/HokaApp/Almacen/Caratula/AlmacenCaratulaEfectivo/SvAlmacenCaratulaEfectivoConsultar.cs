using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using Newtonsoft.Json;
using System.Linq;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaEfectivo
{
    public static class SvAlmacenCaratulaEfectivoConsultar
    {
        public static string Consultar(int almacenCaratulaId)
        {
            EsAlmacenCaratulaEfectivo Estructura = new EsAlmacenCaratulaEfectivo()
            {
                CaratulaEfectivo = new EnAlmacenCaratulaEfectivo()
                {
                    AlmacenCaratulaId = almacenCaratulaId,
                    FechaCaptura = null
                },
                B_ConsultarMoneda = true
            };
            SvAlmacenCaratulaEfectivo servicioMaestro = new SvAlmacenCaratulaEfectivo();
            servicioMaestro.ServicioMaestro("Consultar", Estructura);
            EnAlmacenCaratulaEfectivo AlmacenCaratulaEfectivo =
                servicioMaestro.Estructura.CaratulasEfectivo.Single();
            string jsonAlmacenCaratulaEfectivo = JsonConvert.SerializeObject(AlmacenCaratulaEfectivo);
            return jsonAlmacenCaratulaEfectivo;
        }
    }
}
