using hoka_cli.Models.Ingresos.AlmacenCaratula;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratula
{
    public static class SvAlmacenCaratulaConsultarCaratulasDelDia
    {
        public static string Consultar(
            DateTime? fechaInicio,
            DateTime? fechaFin,
            int almacenId)
        {
            SvAlmacenCaratula svAlmacenCaratula = new SvAlmacenCaratula();
            EsAlmacenCaratula esAlmacenCaratula = new EsAlmacenCaratula()
            {
                Consulta = new Struct.EsConsulta() {
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                },
                Caratula = new EnAlmacenCaratula()
                {
                    AlmacenId = almacenId
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
