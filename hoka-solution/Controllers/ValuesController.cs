using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

/*using Microsoft.Extensions.Configuration;*/
/*using Microsoft.AspNetCore.Mvc;*/
using hoka.Models;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PagedList;
using System.Globalization;
using hoka.Permisos;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Web.WebPages;

namespace hoka.Controllers
{
    public class ValuesController : Controller
    {

        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();

        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;

        public ValuesController()
        {

            if (conexionDBDEV)
            {
                conexionDBHoka = DBHokaTEST;
                conexionDBJoy = DBJoyTEST;
            }
            else
            {
                conexionDBHoka = DBHoka;
                conexionDBJoy = DBJoy;
            }
        }

        [HttpPost]
        public ActionResult MostrarProductosSinRemision(string[] grupo)
        {


            //string conexion = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ConnectionString;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                try
                {
                    cn.Open();

                    string consulta = "SELECT * FROM Vista_ProductosSinRemision WHERE 1=1";


                    using (SqlCommand cmd = new SqlCommand(consulta, cn))
                    {
                        cmd.CommandType = CommandType.Text;

                        // Verificar si se seleccionaron grupos
                        if (grupo != null && grupo.Length > 0)
                        {
                            // Construir la cláusula OR para los grupos
                            StringBuilder categoriaClause = new StringBuilder();
                            for (int i = 0; i < grupo.Length; i++)
                            {
                                string paramName = "@grupo" + i;
                                categoriaClause.Append("grupo = ").Append(paramName);
                                cmd.Parameters.AddWithValue(paramName, grupo[i]);
                                if (i < grupo.Length - 1)
                                {
                                    categoriaClause.Append(" OR ");
                                }
                            }

                            cmd.CommandText += " AND (" + categoriaClause.ToString() + ")";
                        }

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            List<ProductoSinRemisionModel> productosSinRemision = new List<ProductoSinRemisionModel>();

                            while (reader.Read())
                            {
                                var producto = new ProductoSinRemisionModel
                                {
                                    codigobarra = reader["codigobarra"].ToString(),
                                    Producto = reader["Producto"].ToString(),
                                    Nombre = reader["Nombre"].ToString(),
                                    Categoria = reader["nombre_categoria"].ToString(),
                                    preciopub = reader["preciopub"].ToString(),
                                    grupo = reader["grupo"].ToString(),
                                    Existencias = reader["if"].ToString(),

                                    /* = reader[""].ToString(),
                                       = reader[""].ToString(),
                                       = reader[""].ToString(), */
                                };

                                productosSinRemision.Add(producto);
                            }

                            // Puedes ajustar el nombre de las propiedades según tu modelo
                            var jsonResult = Json(new { data = productosSinRemision }, JsonRequestBehavior.AllowGet);
                            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                            return (ActionResult)jsonResult;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones: Puedes registrar el error o tomar alguna otra acción según tus necesidades.
                    string errorMessage = $"Error al obtener productos sin remisión. Detalles: {ex.ToString()}";

                    Console.WriteLine(errorMessage);

                    var jsonResult = Json(new { success = false, message = errorMessage }, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;
                }
            }
        }
    }
}