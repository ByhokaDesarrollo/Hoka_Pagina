using hoka_cli.Models.Compuadmo.Almacen;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace hoka_cli.HokaApp.Compuadmo.Almacen
{
    public static class SvAlmacenConsultar
    {
        public static string Consultar(EnAlmacen parametro)
        {
            EsAlmacen esAlmacen = new EsAlmacen()
            {
                Almacen = parametro
            };
            SvAlmacen svAlmacen = new SvAlmacen();
            svAlmacen.ServicioMaestro("Consultar", esAlmacen);
            ICollection<EnAlmacen> Almacenes = svAlmacen.Estructura.Almacenes;
            string jsonCaratulas = JsonConvert.SerializeObject(Almacenes);
            return jsonCaratulas;
        }
    }
}
