using hoka.HokaCli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.HokaApp.Almacen.Caratula.AlmacenCaratulaVenta;
using Newtonsoft.Json;
using System.Collections.Generic;
using HokaCli_EnAlmacenCategoriaVenta = hoka_cli.Models.Ingresos.AlmacenCaratula.EnAlmacenCaratula;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratulaVenta.Servicio
{
    public static class SvAppAlmacenCaratulaVentaConsultarCategoriaVentaDelDia
    {
        public static ICollection<EnAlmacenCategoriaVenta> Consultar(EnAlmacenCaratula caratula)
        {
            HokaCli_EnAlmacenCategoriaVenta AlmacenCaratula = ConvertirCaratulaParametro(caratula);
            string jsonCaratulas = SvAlmacenCaratulaVentaConsultarCategoriaVentaDelDia.Consultar(AlmacenCaratula);
            ICollection<EnAlmacenCategoriaVenta> CategoriasVenta = JsonConvert
                .DeserializeObject<ICollection<EnAlmacenCategoriaVenta>>(jsonCaratulas);
            return CategoriasVenta;
        }

        private static HokaCli_EnAlmacenCategoriaVenta ConvertirCaratulaParametro(EnAlmacenCaratula caratula)
        {
            string jsonCaratulaParametro = JsonConvert.SerializeObject(caratula);
            HokaCli_EnAlmacenCategoriaVenta AlmacenCaratula =
                JsonConvert.DeserializeObject<HokaCli_EnAlmacenCategoriaVenta>(jsonCaratulaParametro);
            return AlmacenCaratula;
        }
    }
}