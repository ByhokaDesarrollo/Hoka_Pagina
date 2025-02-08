using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios
{
    public static class SvAppAlmacenCaratulaConsultarCaratulasDelDia
    {
        public static ICollection<EnAlmacenCaratula> Consultar(DateTime fechaActual)
        {
            string jsonCaratulas = SvAlmacenCaratulaConsultarCaratulasDelDia.Consultar(fechaActual);
            ICollection<EnAlmacenCaratula> Caratulas = JsonConvert
                .DeserializeObject<ICollection<EnAlmacenCaratula>>(jsonCaratulas);
            return Caratulas;
        }
    }
}