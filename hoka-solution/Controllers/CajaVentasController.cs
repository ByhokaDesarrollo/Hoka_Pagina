using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using hoka.Models;
using hoka.Permisos;
using System.Configuration;
using System.Web.WebPages;

namespace hoka.Controllers
{
    
    public class CajaVentasController : Controller
    {

        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();

        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;
        public CajaVentasController()
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

       
    }
}