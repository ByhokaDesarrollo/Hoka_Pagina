using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka.AppServicios.Ingresos.AlmacenCaratula
{
    public static class SvAppAlmacenCaratulaConsultarCaratulasDelDia
    {
        public static ICollection<EnAlmacenCaratula> Consultar(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int almacenId)
        {
            string jsonCaratulas = SvAlmacenCaratulaConsultarCaratulasDelDia.Consultar(
                fechaInicio:fechaInicio,
                fechaFin: fechaFin,
                almacenId: almacenId);
            ICollection<EnAlmacenCaratula> Caratulas = JsonConvert
                .DeserializeObject<ICollection<EnAlmacenCaratula>>(jsonCaratulas);
            return Caratulas;
        }
    }
}