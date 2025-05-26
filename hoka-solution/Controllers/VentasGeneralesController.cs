using hoka.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System;
using Dapper;

namespace hoka.Controllers
{
    public class VentasGeneralesController : Controller
    {
        private static readonly string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static readonly string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();
        private static readonly string DBIngresos = ConfigurationManager.ConnectionStrings["CadenaConexionHokaIngresos"].ToString();
        private static readonly string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static readonly string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();
        private static readonly string DBIngresosTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaIngresos"].ToString();

        private readonly string conexionDBHoka;

        public VentasGeneralesController()
        {
            conexionDBHoka = DBHoka;
        }

        [HttpPost]
        public ActionResult filtrarTablaAB(string fechaInicio, string fechaFin, string almacen)
        {
            if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaInicioParsed) ||
                !DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime fechaFinParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            var rawData = GetRemisioData(fechaInicioParsed, fechaFinParsed, almacen);
            var processedData = ProcessRemisioData(rawData, almacen);

            return Json(processedData, JsonRequestBehavior.AllowGet);
        }

        private List<RemisioDModel> GetRemisioData(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            using (var connection = new SqlConnection(conexionDBHoka))
            {
                var query = almacen == "todos"
                    ? @"SELECT * FROM VRemisioDM WHERE fecha >= @fechaInicio AND fecha <= @fechaFin"
                    : @"SELECT * FROM VRemisioDM WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                var parameters = new DynamicParameters();
                parameters.Add("@fechaInicio", fechaInicio);
                parameters.Add("@fechaFin", fechaFin);
                if (almacen != "todos")
                {
                    parameters.Add("@almacen", almacen);
                }

                var result = connection.Query<RemisioDModel>(query, parameters).ToList();

                foreach (var item in result)
                {
                    item.VentaReal = CalculateVentaReal(
                        Convert.ToDouble(item.total),
                        Convert.ToDouble(item.descuento),
                        Convert.ToDouble(item.stotal));
                }

                return result;
            }
        }

        private double CalculateVentaReal(double total, double descuento, double stotal)
        {
            if (total > descuento)
            {
                var r1 = Math.Abs(total - descuento);
                var r2 = (r1 / descuento) + 1;
                return stotal * r2;
            }
            else if (total < descuento)
            {
                var r1 = Math.Abs(total - descuento);
                var r2 = r1 / descuento;
                var r3 = stotal - (stotal * r2);
                return r3;
            }
            else
            {
                return stotal;
            }
        }

        private object ProcessRemisioData(List<RemisioDModel> rawData, string almacen)
        {
            var knownCategories = new List<string>
            {
                "ABARROTES", "FARMACIA", "ARTESANIAS", "ARTESANIAS PREMIUM",
                "SOUVENIR", "TEXTIL", "TABAQUERIA", "CALENDARIOS"
            };

            double GetCategoryValue(Dictionary<string, double> dict, string category)
            {
                return dict.ContainsKey(category) ? dict[category] : 0;
            }

            if (almacen == "todos")
            {
                return rawData
                    .GroupBy(x => x.almacen)
                    .Select(g =>
                    {
                        var categorySums = new Dictionary<string, double>();
                        double totalGeneral = 0;
                        double sinCategoria = 0;

                        foreach (var registro in g)
                        {
                            var cat = registro.categoria;
                            var v = registro.VentaReal;

                            if (knownCategories.Contains(cat))
                            {
                                if (!categorySums.ContainsKey(cat))
                                {
                                    categorySums[cat] = 0;
                                }
                                categorySums[cat] += v;
                            }
                            else
                            {
                                sinCategoria += v;
                            }

                            totalGeneral += v;
                        }

                        return new
                        {
                            Sucursal = g.Key,
                            Group_Abarrotes = GetCategoryValue(categorySums, "ABARROTES"),
                            Group_Farmacia = GetCategoryValue(categorySums, "FARMACIA"),
                            Group_Artesanias = GetCategoryValue(categorySums, "ARTESANIAS"),
                            Group_Artesanias_Premium = GetCategoryValue(categorySums, "ARTESANIAS PREMIUM"),
                            Group_Souvenir = GetCategoryValue(categorySums, "SOUVENIR"),
                            Group_Textil = GetCategoryValue(categorySums, "TEXTIL"),
                            Group_Tabaqueria = GetCategoryValue(categorySums, "TABAQUERIA"),
                            Group_Calendarios = GetCategoryValue(categorySums, "CALENDARIOS"),

                            Group_SinCategoria = sinCategoria,
                            Group_TotalImporte = totalGeneral
                        };
                    })
                    .ToList();
            }
            else
            {
                var categorySums = new Dictionary<string, double>();
                double totalGeneral = 0;
                double sinCategoria = 0;

                foreach (var registro in rawData)
                {
                    var cat = registro.categoria;
                    var v = registro.VentaReal;

                    if (knownCategories.Contains(cat))
                    {
                        if (!categorySums.ContainsKey(cat))
                        {
                            categorySums[cat] = 0;
                        }
                        categorySums[cat] += v;
                    }
                    else
                    {
                        sinCategoria += v;
                    }

                    totalGeneral += v;
                }

                var resultadoConsolidado = new
                {
                    Group_Abarrotes = GetCategoryValue(categorySums, "ABARROTES"),
                    Group_Farmacia = GetCategoryValue(categorySums, "FARMACIA"),
                    Group_Artesanias = GetCategoryValue(categorySums, "ARTESANIAS"),
                    Group_Artesanias_Premium = GetCategoryValue(categorySums, "ARTESANIAS PREMIUM"),
                    Group_Souvenir = GetCategoryValue(categorySums, "SOUVENIR"),
                    Group_Textil = GetCategoryValue(categorySums, "TEXTIL"),
                    Group_Tabaqueria = GetCategoryValue(categorySums, "TABAQUERIA"),
                    Group_Calendarios = GetCategoryValue(categorySums, "CALENDARIOS"),

                    Group_SinCategoria = sinCategoria,
                    Group_TotalImporte = totalGeneral
                };

                return new[] { resultadoConsolidado };
            }
        }

        public ActionResult ReporteVentasGeneralTiendas()
        {
            var almacenes = GetAlmacenes();
            ViewBag.Almacenes = almacenes;
            return View();
        }

        private List<Tuple<string, string>> GetAlmacenes()
        {
            using (var connection = new SqlConnection(conexionDBHoka))
            {
                const string query = "SELECT almacen, nombre FROM almacenes ORDER BY almacen";
                var result = connection.Query<(string almacen, string nombre)>(query)
                    .Select(x => new Tuple<string, string>(x.almacen, x.nombre))
                    .ToList();

                return result;
            }
        }
    }
}