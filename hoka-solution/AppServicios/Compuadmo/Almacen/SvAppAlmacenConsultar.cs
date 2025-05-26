using hoka.HokaCli.Models.Compuadmo.Almacen;
using hoka_cli.HokaApp.Compuadmo.Almacen;
using Newtonsoft.Json;
using System.Collections.Generic;
using HokaCli_EnAlmacen = hoka_cli.Models.Compuadmo.Almacen.EnAlmacen;

namespace hoka.AppServicios.Compuadmo.Almacen
{
    public static class SvAppAlmacenConsultar
    {
        public static ICollection<EnAlmacen> Consultar(int almacenId = 0)
        {
            EnAlmacen AlmacenParametro = new EnAlmacen()
            {
                AlmacenId = almacenId
            };
            HokaCli_EnAlmacen ObjetoConvertido = ConvertirParametro(AlmacenParametro);
            string jsonAlmacen = SvAlmacenConsultar.Consultar(ObjetoConvertido);
            ICollection<EnAlmacen> Almacenes = JsonConvert
                .DeserializeObject<ICollection<EnAlmacen>>(jsonAlmacen);
            return Almacenes;
        }

        private static HokaCli_EnAlmacen ConvertirParametro(EnAlmacen entidad)
        {
            string jsonParametro = JsonConvert.SerializeObject(entidad);
            HokaCli_EnAlmacen ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacen>(jsonParametro);
            return ObjetoConvertido;
        }
    }
}