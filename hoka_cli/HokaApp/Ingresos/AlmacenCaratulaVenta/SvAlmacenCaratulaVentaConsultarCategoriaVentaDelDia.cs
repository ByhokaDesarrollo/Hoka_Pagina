using hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Joyeria.AlmacenCategoriaVenta;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.HokaApp.Ingresos.AlmacenCaratulaVenta
{
    public static class SvAlmacenCaratulaVentaConsultarCategoriaVentaDelDia
    {
        public static string Consultar(EnAlmacenCaratula caratula)
        {
            RpAlmacenCategoriaVenta rpAlmacenCategoriaVenta = SvAlmacenCategoriaVentaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCategoriaVenta esAlmacenCategoriaVenta = new EsAlmacenCategoriaVenta()
            {
                Consulta = new EsConsulta()
                {
                    FechaInicio = caratula?.FechaRegistro ?? null,
                    FechaFin = caratula?.FechaRegistro ?? null,
                },
                AlmacenCategoriaVenta = new EnAlmacenCategoriaVenta()
                {
                    AlmacenId = caratula?.AlmacenId ?? 0,
                    Fecha = null
                },
                B_ConsultarAlmacen = true,
                B_ConsultarCategoria = true
            };
            SvAlmacenCategoriaVentaConsultar servicio =
                new SvAlmacenCategoriaVentaConsultar(rpAlmacenCategoriaVenta, esAlmacenCategoriaVenta);
            ICollection<EnAlmacenCategoriaVenta> AlmacenCategoriaVentas = servicio.Consultar();

            rpAlmacenCategoriaVenta = SvAlmacenCategoriaVentaIniciarRepositorioJoyeria.IniciarRepositorio();
            SvJoyeriaAlmacenCategoriaVentaConsultar servicioJoyeria =
                new SvJoyeriaAlmacenCategoriaVentaConsultar(rpAlmacenCategoriaVenta, esAlmacenCategoriaVenta);
            EnAlmacenCategoriaVenta AlmacenCategoriaJoyeriaVentas = servicioJoyeria.Consultar()?.Count > 0
                ? servicioJoyeria.Consultar().Single()
                : null;
            if (AlmacenCategoriaJoyeriaVentas != null)
                foreach (var categoria in AlmacenCategoriaVentas.Where(x => x.CategoriaId == AlmacenCategoriaJoyeriaVentas.CategoriaId))
                {
                    categoria.ImporteTotal = AlmacenCategoriaJoyeriaVentas.ImporteTotal;
                }

            string jsonAlmacenCategoriaVentas = JsonConvert.SerializeObject(AlmacenCategoriaVentas);
            return jsonAlmacenCategoriaVentas;
        }
    }
}
