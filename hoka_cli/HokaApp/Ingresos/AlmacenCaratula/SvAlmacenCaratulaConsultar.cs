using hoka_cli.Models.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System.Linq;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratula
{
    public static class SvAlmacenCaratulaConsultar
    {
        public static string Consultar(EnAlmacenCaratula parametro)
        {
            SvAlmacenCaratula svAlmacenCaratula = new SvAlmacenCaratula();
            EsAlmacenCaratula esAlmacenCaratula = new EsAlmacenCaratula()
            {
                Caratula = new EnAlmacenCaratula()
                {
                    AlmacenCaratulaId = parametro.AlmacenCaratulaId,
                    FechaRegistro = parametro.FechaRegistro
                },
                B_ConsultarAlmacen = true,
                B_ConsultarUsuario = true,
                B_ConsultarEstatus = true,
                B_ConsultarCaratulaVenta = false,
                B_ConsultarCaratulaEfectivo = false,
                B_ConsultarCaratulaVoucher = false,
                B_ConsultarCaratula = false
            };
            svAlmacenCaratula.ServicioMaestro("Consultar", esAlmacenCaratula);
            EnAlmacenCaratula Caratula = svAlmacenCaratula.Estructura.Caratulas.Single();
            string jsonCaratulas = JsonConvert.SerializeObject(Caratula);
            return jsonCaratulas;
        }
    }
}
