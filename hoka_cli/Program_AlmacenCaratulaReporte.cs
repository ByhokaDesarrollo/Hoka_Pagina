using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_AlmacenCaratulaReporte
    {
        static void AMain(string[] args)
        {
            #region AlmacenCaratulaReporte
            // AlmacenCaratulaReporte
            RpAlmacenCaratula rpAlmacenCaratulaReporte = SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratula esAlmacenCaratulaReporte = new EsAlmacenCaratula()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                Caratula = new EnAlmacenCaratula()
                {
                    FechaRegistro = new DateTime(2024, 8, 18)
                },
                B_ConsultarAlmacen = true,
                B_ConsultarUsuario = true,
                B_ConsultarEstatus = true
            };
            SvAlmacenCaratulaConsultar servicio =
                new SvAlmacenCaratulaConsultar(rpAlmacenCaratulaReporte, esAlmacenCaratulaReporte);
            ICollection<EnAlmacenCaratula> AlmacenCaratulaReportees = servicio.Consultar();
            string jsonAlmacenCaratulaReportees = JsonConvert.SerializeObject(AlmacenCaratulaReportees);
            Console.WriteLine(jsonAlmacenCaratulaReportees);
            Console.ReadKey();
            #endregion
        }
    }
}
