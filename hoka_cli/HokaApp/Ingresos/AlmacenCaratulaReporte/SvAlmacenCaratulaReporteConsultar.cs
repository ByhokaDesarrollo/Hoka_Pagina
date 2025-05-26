using hoka_cli.Models.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System.Linq;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaCaratula
{
    public static class SvAlmacenCaratulaReporteConsultar
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
                B_ConsultarCaratulaVenta = true,
                B_ConsultarCaratulaEfectivo = true,
                B_ConsultarCaratulaVoucher = true,
                B_ConsultarCaratula = true
            };
            svAlmacenCaratula.ServicioMaestro("Consultar", esAlmacenCaratula);
            EnAlmacenCaratula Caratula = svAlmacenCaratula.Estructura.Caratulas.Single();
            string jsonCaratulas = JsonConvert.SerializeObject(Caratula);
            return jsonCaratulas;
        }
    }
}
