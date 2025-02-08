using hoka_cli.Models.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System.Collections.Generic;
using hoka_cli.Models.Ingresos.Banco.Caratula;

namespace hoka_cli.HokaApp.Banco.Caratula
{
    public static class SvBancoCaratulaConsultarPorUsuario
    {
        public static string Consultar(EsAlmacenCaratula pAlmacenCaratula)
        {
            SvBancoCaratula svBancoCaratula = new SvBancoCaratula();
            EsAlmacenCaratula esAlmacenCaratula = pAlmacenCaratula;
            svBancoCaratula.ServicioMaestro("Consultar", esAlmacenCaratula);
            ICollection<EnAlmacenCaratula> Caratulas = svBancoCaratula.Estructura.Caratulas;
            string jsonCaratulas = JsonConvert.SerializeObject(Caratulas);
            return jsonCaratulas;
        }
    }
}
