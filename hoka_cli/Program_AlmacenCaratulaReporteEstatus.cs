using hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_AlmacenCaratulaReporteEstatus
    {
        static void AMain(string[] args)
        {
            #region AlmacenCaratulaReporteEstatus
            // AlmacenCaratulaReporteEstatus
            RpAlmacenCaratulaEstatus rpAlmacenCaratulaReporteEstatus = SvAlmacenCaratulaEstatusIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaEstatus esAlmacenCaratulaReporteEstatus = new EsAlmacenCaratulaEstatus()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                CaratulaEstatus = new EnAlmacenCaratulaEstatus()
                {
                    //AlmacenCaratulaReporteEstatusId = 1
                }
            };
            SvAlmacenCaratulaEstatusConsultar servicio = new SvAlmacenCaratulaEstatusConsultar(rpAlmacenCaratulaReporteEstatus, esAlmacenCaratulaReporteEstatus);
            ICollection<EnAlmacenCaratulaEstatus> AlmacenCaratulaReporteEstatuses = servicio.Consultar();
            string jsonAlmacenCaratulaReporteEstatuses = JsonConvert.SerializeObject(AlmacenCaratulaReporteEstatuses);
            Console.WriteLine(jsonAlmacenCaratulaReporteEstatuses);
            Console.ReadKey();
            #endregion
        }
    }
}
