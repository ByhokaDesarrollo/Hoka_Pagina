using hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_AlmacenCategoriaVenta
    {
        static void AMain(string[] args)
        {
            #region AlmacenCategoriaVenta
            // AlmacenCategoriaVenta
            RpAlmacenCategoriaVenta rpAlmacenCategoriaVenta = SvAlmacenCategoriaVentaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCategoriaVenta esAlmacenCategoriaVenta = new EsAlmacenCategoriaVenta()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                AlmacenCategoriaVenta = new EnAlmacenCategoriaVenta()
                {
                    Fecha = new DateTime(2024, 8, 20)
                },
                B_ConsultarAlmacen = true,
                B_ConsultarCategoria = true
            };
            SvAlmacenCategoriaVentaConsultar servicio =
                new SvAlmacenCategoriaVentaConsultar(rpAlmacenCategoriaVenta, esAlmacenCategoriaVenta);
            ICollection<EnAlmacenCategoriaVenta> AlmacenCategoriaVentaes = servicio.Consultar();
            string jsonAlmacenCategoriaVentaes = JsonConvert.SerializeObject(AlmacenCategoriaVentaes);
            Console.WriteLine(jsonAlmacenCategoriaVentaes);
            Console.ReadKey();
            #endregion
        }
    }
}
