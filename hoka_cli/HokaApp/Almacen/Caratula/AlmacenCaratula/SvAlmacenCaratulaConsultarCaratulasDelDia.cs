using hoka_cli.Models.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratula
{
    public static class SvAlmacenCaratulaConsultarCaratulasDelDia
    {
        public static string Consultar(DateTime fechaActual)
        {
            SvAlmacenCaratula svAlmacenCaratula = new SvAlmacenCaratula();
            EsAlmacenCaratula esAlmacenCaratula = new EsAlmacenCaratula()
            {
                Caratula = new EnAlmacenCaratula()
                {
                    FechaRegistro = fechaActual
                },
                B_ConsultarAlmacen = true,
                B_ConsultarUsuario = true,
                B_ConsultarEstatus = true
            };
            svAlmacenCaratula.ServicioMaestro("Consultar", esAlmacenCaratula);
            ICollection<EnAlmacenCaratula> Caratulas = svAlmacenCaratula.Estructura.Caratulas;
            string jsonCaratulas = JsonConvert.SerializeObject(Caratulas);
            return jsonCaratulas;
        }
    }
}
