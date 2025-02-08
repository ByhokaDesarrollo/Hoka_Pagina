using hoka_cli.Models.Compuadmo.Almacen;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_Almacen
    {
        static void AMain(string[] args)
        {
            #region Almacen
            // Almacen
            RpAlmacen rpAlmacen = SvAlmacenIniciarRepositorio.IniciarRepositorio();
            EsAlmacen esAlmacen = new EsAlmacen()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                Almacen = new EnAlmacen()
                {
                    AlmacenId = 1
                }
            };
            SvAlmacenConsultar servicio = new SvAlmacenConsultar(rpAlmacen, esAlmacen);
            ICollection<EnAlmacen> Almacenes = servicio.Consultar();
            string jsonAlmacenes = JsonConvert.SerializeObject(Almacenes);
            Console.WriteLine(jsonAlmacenes);
            Console.ReadKey();
            #endregion
        }
    }
}
