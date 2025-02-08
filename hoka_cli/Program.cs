using hoka_cli.Models.Compuadmo.Moneda;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program
    {
        static void Main(string[] args)
        {
            #region Moneda
            // Moneda
            RpMoneda rpMoneda = SvMonedaIniciarRepositorio.IniciarRepositorio();
            EsMoneda esMoneda = new EsMoneda()
            {
                Consulta = new EsConsulta()
                {
                    // Insertar Codigo
                },
                Moneda = new EnMoneda()
                {

                },
                B_ConsultarMonedaTipo = true,
                B_ConsultarDenominacion = true
            };
            SvMonedaConsultar servicio =
                new SvMonedaConsultar(rpMoneda, esMoneda);
            ICollection<EnMoneda> Monedaes = servicio.Consultar();
            string jsonMonedaes = JsonConvert.SerializeObject(Monedaes);
            Console.WriteLine(jsonMonedaes);
            Console.ReadKey();
            #endregion
        }
    }
}
