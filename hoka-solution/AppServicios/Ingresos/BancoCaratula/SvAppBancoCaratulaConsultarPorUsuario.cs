using Newtonsoft.Json;
using System.Collections.Generic;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using EnAlmacenCaratula = hoka.HokaCli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;
using hoka_cli.HokaApp.Ingresos.BancoCaratula;

namespace hoka.AppServicios.Ingresos.BancoCaratula
{
    public class SvAppBancoCaratulaConsultarPorUsuario
    {
        public static ICollection<EnAlmacenCaratula> Consultar(EsAlmacenCaratula pAlmacenCaratula)
        {
            string jsonCaratulas = SvBancoCaratulaConsultarPorUsuario.Consultar(pAlmacenCaratula);
            ICollection<EnAlmacenCaratula> Caratulas = JsonConvert
                .DeserializeObject<ICollection<EnAlmacenCaratula>>(jsonCaratulas);
            return Caratulas;
        }
    }
}