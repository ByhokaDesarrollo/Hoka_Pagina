using hoka_cli.Models.Compuadmo.Catalogo.Almacen;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_CatalogoAlmacen
    {
        static void AMain(string[] args)
        {
            #region CatalogoAlmacen
            // CatalogoAlmacen
            RpCatalogoAlmacen rpCatalogoAlmacen = SvCatalogoAlmacenIniciarRepositorio.IniciarRepositorio();
            EsCatalogoAlmacen esCatalogoAlmacen = new EsCatalogoAlmacen()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                CatalogoAlmacen = new EnCatalogoAlmacen()
                {
                    //AlmacenId = 1
                }
            };
            SvCatalogoAlmacenConsultar servicio = new SvCatalogoAlmacenConsultar(rpCatalogoAlmacen, esCatalogoAlmacen);
            ICollection<EnCatalogoAlmacen> CatalogoAlmacenes = servicio.Consultar();
            string jsonCatalogoAlmacenes = JsonConvert.SerializeObject(CatalogoAlmacenes);
            Console.WriteLine(jsonCatalogoAlmacenes);
            Console.ReadKey();
            #endregion
        }
    }
}
