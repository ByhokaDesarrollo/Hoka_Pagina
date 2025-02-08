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
using System.Collections;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.Web.Script.Serialization;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace hoka.Controllers
{

    public class SuperPagesController : Controller
    {
        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();

        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;

        public SuperPagesController()
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
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult ObtenerCuadreEfectivo(string fecha, string idAlmacen)
        {
            DateTime FechaParsed;
            //DateTime FechaFinParsed;

            //DateTime Fecha = DateTime.Now;
            //Fecha.ToString("yyyy-MM-dd");

            if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            var cuadreEfectivo = new List<ViewValoresEntregados>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ObtenerCuadreMoneda", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaParsed);
                //cmd.Parameters.AddWithValue("@FechaFin", FechaFinParsed);
                cmd.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ImporteMonedaPesosL = 0;
                        int ImporteMonedaOrigenL = 0;

                        if (Convert.ToInt32(reader["Id"]) == 1)
                        {
                            ImporteMonedaPesosL = Convert.ToInt32(reader["ImporteMonedaPesos"]);
                            ImporteMonedaOrigenL = Convert.ToInt32(reader["ImporteMonedaPesos"]);
                        }
                        else
                        {
                            ImporteMonedaPesosL = Convert.ToInt32(reader["ImporteMonedaPesos"]);
                            ImporteMonedaOrigenL = Convert.ToInt32(reader["ImporteMonedaOrigen"]);
                        }


                        cuadreEfectivo.Add(new ViewValoresEntregados
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            ImporteMonedaPesos = ImporteMonedaPesosL,
                            ImporteMonedaOrigen = ImporteMonedaOrigenL,
                            IdRegistroValoresEntregados = Convert.ToInt32(reader["IdRegistroValoresEntregados"]),
                            MonedaNombre = reader["MonedaNombre"].ToString()
                        });
                    }
                }
                //
            }

            //ViewBag.CuadreEfectivo = cuadreEfectivo;

            var cuadreMonedaSummary = new List<CuadreMonedaSummary>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"SELECT * FROM ingresos.dbo.CuadreMonedaSummary WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen";

                //string baseQuery = @"SELECT * FROM joyeria.dbo.VObtenerTotalProductosJoyeria WHERE fecha = @fecha AND almacen= @almacen";


                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@Fecha", FechaParsed.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                cmd.CommandType = CommandType.Text;

                //Response.Write(FechaParsed.ToString("yyyy-MM-dd"));
                //Response.Write(idAlmacen);

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //List<SalesByAgenciesAndGuides> rawData = new List<SalesByAgenciesAndGuides>();

                    //ArrayList list = new ArrayList();

                    while (reader.Read())
                    {
                        cuadreMonedaSummary.Add(new CuadreMonedaSummary
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Fecha = reader["Fecha"].ToString(),
                            //IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            IdMoneda = Convert.ToInt32(reader["IdMoneda"]),
                            Recepcion = Convert.ToSingle(reader["Recepcion"])
                        });

                    }
                }
            }

            var listVMCuadreMoneda = new List<VMCuadreMoneda>();

            foreach (var value in cuadreEfectivo)
            {
                float Recepcion = 0;
                int IdSummary = 0;
                int IdMoneda = 0;
                string Fecha = "";

                foreach (var value2 in cuadreMonedaSummary)
                {
                    if (value.Id == value2.IdMoneda)
                    {
                        Recepcion = value2.Recepcion;
                        IdSummary = value2.Id;
                        IdMoneda = value2.IdMoneda;
                        Fecha = value2.Fecha;
                        break;
                    }
                }


                listVMCuadreMoneda.Add(new VMCuadreMoneda
                {
                    Id = value.Id,
                    //IdVoucher = IdVoucher,
                    ImporteMonedaPesos = value.ImporteMonedaPesos,
                    ImporteMonedaOrigen = value.ImporteMonedaOrigen,
                    //Cantidad = value.Cantidad,
                    MonedaNombre = value.MonedaNombre,
                    Recepcion = Recepcion,
                    IdSummary = IdSummary,
                    Fecha = Fecha,
                });
            }


            var jsonResult = Json(listVMCuadreMoneda, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
            return jsonResult;

        }

        [HttpPost]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult ObtenerCuadreVouchers(string fecha, string idAlmacen)
        {
            DateTime FechaParsed;
            //DateTime FechaFinParsed;

            //DateTime Fecha = DateTime.Now;
            //Fecha.ToString("yyyy-MM-dd");

            if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            var cuadreVoucher = new List<VCuadreVoucher>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ObtenerCuadreVoucher", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaParsed);
                //cmd.Parameters.AddWithValue("@FechaFin", FechaFinParsed);
                cmd.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        cuadreVoucher.Add(new VCuadreVoucher
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Importe = Convert.ToDouble(reader["Importe"]),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            VoucherNombre = reader["VoucherNombre"].ToString()
                        });
                    }
                }
            }

            var cuadreVoucherSummary = new List<CuadreVoucherSummary>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"SELECT * FROM ingresos.dbo.CuadreVoucherSummary WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen";

                //string baseQuery = @"SELECT * FROM joyeria.dbo.VObtenerTotalProductosJoyeria WHERE fecha = @fecha AND almacen= @almacen";


                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@Fecha", FechaParsed.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                cmd.CommandType = CommandType.Text;

                //Response.Write(FechaParsed.ToString("yyyy-MM-dd"));
                //Response.Write(idAlmacen);

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    //List<SalesByAgenciesAndGuides> rawData = new List<SalesByAgenciesAndGuides>();

                    //ArrayList list = new ArrayList();

                    while (reader.Read())
                    {
                        cuadreVoucherSummary.Add(new CuadreVoucherSummary
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Fecha = reader["Fecha"].ToString(),
                            //IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            IdVoucher = Convert.ToInt32(reader["IdVoucher"]),
                            Recepcion = Convert.ToSingle(reader["Recepcion"])
                        });

                    }
                }
            }

            var listVMCuadreVoucher = new List<VMCuadreVoucher>();

            foreach (var value in cuadreVoucher)
            {
                float Recepcion = 0;
                int IdSummary = 0;
                int IdVoucher = 0;
                string Fecha = "";

                foreach (var value2 in cuadreVoucherSummary)
                {
                    if (value.Id == value2.IdVoucher)
                    {
                        Recepcion = value2.Recepcion;
                        IdSummary = value2.Id;
                        IdVoucher = value2.IdVoucher;
                        Fecha = value2.Fecha;
                        break;
                    }
                }


                listVMCuadreVoucher.Add(new VMCuadreVoucher
                {
                    Id = value.Id,
                    //IdVoucher = IdVoucher,
                    Importe = value.Importe,
                    Cantidad = value.Cantidad,
                    VoucherNombre = value.VoucherNombre,
                    Recepcion = Recepcion,
                    IdSummary = IdSummary,
                    Fecha = Fecha,
                });
            }


            //ViewBag.CuadreEfectivo = cuadreEfectivo;

            var jsonResult = Json(listVMCuadreVoucher, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
            return jsonResult;

        }

        //ActualizarCuadreVouchers
        [HttpPost]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult ActualizarCuadreVouchers()
        {
            string Fecha = "";
            int IdAlmacen = 0;
            float Recepcion = 0;
            int IdVoucher = 0;
            int IdSummary = 0;

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("fecha"))
                {
                    Fecha = Request.Form[key];
                }
                if (key.StartsWith("idAlmacen"))
                {
                    IdAlmacen = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("recepcion"))
                {
                    Recepcion = Convert.ToSingle(Request.Form[key]);
                }
                if (key.StartsWith("idVoucher"))
                {
                    IdVoucher = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("idSummary"))
                {
                    IdSummary = Convert.ToInt32(Request.Form[key]);
                }
            }

            DateTime FechaParsed;

            if (!DateTime.TryParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ActualizarRecepcionVoucher", cn);

                cmd.Parameters.AddWithValue("Fecha", FechaParsed.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("Recepcion", Recepcion);
                cmd.Parameters.AddWithValue("IdVoucher", IdVoucher);
                cmd.Parameters.AddWithValue("IdSummary", IdSummary);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();
            }


            return RedirectToRoute("super-caja-vouchers");

        }


        [HttpPost]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult ActualizarCuadreMonedas()
        {
            string Fecha = "";
            int IdAlmacen = 0;
            float Recepcion = 0;
            int IdMoneda = 0;
            int IdSummary = 0;

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("fecha"))
                {
                    Fecha = Request.Form[key];
                }
                if (key.StartsWith("idAlmacen"))
                {
                    IdAlmacen = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("recepcion"))
                {
                    Recepcion = Convert.ToSingle(Request.Form[key]);
                }
                if (key.StartsWith("idMoneda"))
                {
                    IdMoneda = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("idSummary"))
                {
                    IdSummary = Convert.ToInt32(Request.Form[key]);
                }
            }

            DateTime FechaParsed;

            if (!DateTime.TryParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ActualizarRecepcionMoneda", cn);

                cmd.Parameters.AddWithValue("Fecha", FechaParsed.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("Recepcion", Recepcion);
                cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                cmd.Parameters.AddWithValue("IdSummary", IdSummary);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();
            }


            return RedirectToRoute("super-caja-cuadre-efectivo");

        }


        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCuadreEfectivo()
        {

            ViewBag.ActivePage = "VCuadreEfectivo";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var almacenId = Convert.ToInt32(reader["AlmacenId"]);
                    var almacenNombre = reader["NombreAlmacen"].ToString();
                    almacenes.Add(new Tuple<int, string>(almacenId, almacenNombre));
                }
            }

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            return View("/Views/SuperPages/VCuadreEfectivo.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCuadreVoucher()
        {

            ViewBag.ActivePage = "VCuadreVoucher";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var almacenId = Convert.ToInt32(reader["AlmacenId"]);
                    var almacenNombre = reader["NombreAlmacen"].ToString();
                    almacenes.Add(new Tuple<int, string>(almacenId, almacenNombre));
                }
            }

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            //


            return View("/Views/SuperPages/VCuadreVoucher.cshtml");

        }

        //
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCajaAlmacenes()
        {

            ViewBag.ActivePage = "VCajaAlmacenes";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            //DateTime fechaParsed;
            DateTime Fecha = DateTime.Now;
            Fecha.ToString("yyyy-MM-dd");

            //if (!DateTime.TryParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
            //{
            //    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            //}

            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var IDAlmacen = Convert.ToInt32(reader["IDAlmacen"]);
                    var almacenId = Convert.ToInt32(reader["AlmacenId"]);
                    var almacenNombre = reader["NombreAlmacen"].ToString();
                    almacenes.Add(new Tuple<int, string>(almacenId, almacenNombre));
                }
            }

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            return View("/Views/SuperPages/VCajaAlmacenes.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCajaAlmacenesFecha(string IdAlmacen)
        {

            ViewBag.ActivePage = "VCajaAlmacenes";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            List<Tuple<string>> fechas = new List<Tuple<string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {

                string baseQuery = @"SELECT distinct Fecha FROM ingresos.dbo.RegistroValoresEntregados WHERE IdAlmacen = @IdAlmacen group by Fecha";
                SqlCommand cmd = new SqlCommand(baseQuery, cn);
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);
                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd");
                    fechas.Add(new Tuple<string>(Fecha));
                }
            }

            ViewBag.Fechas = fechas;
            ViewBag.IdAlmacen = IdAlmacen;


            //var monedas = new List<Monedas>();

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    cn.Open();
            //    SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM ingresos.dbo.Monedas", cn);
            //    using (SqlDataReader reader = cmdRemisioPago.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {


            //            monedas.Add(new Monedas
            //            {
            //                Id = Convert.ToInt32(reader["Id"]),
            //                Nombre = reader["Nombre"].ToString()
            //            });

            //        }
            //    }
            //}

            //ViewBag.Monedas = monedas;


            return View("/Views/SuperPages/VCajaAlmacenesFecha.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMoneda(string IdAlmacen, string Fecha)
        {

            ViewBag.ActivePage = "VCajaAlmacenes";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            //List<Tuple<string>> fechas = new List<Tuple<string>>();

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{

            //    string baseQuery = @"SELECT distinct Fecha FROM ingresos.dbo.RegistroValoresEntregados WHERE IdAlmacen = @IdAlmacen group by Fecha";
            //    SqlCommand cmd = new SqlCommand(baseQuery, cn);
            //    cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);
            //    cn.Open();

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd");
            //        fechas.Add(new Tuple<string>(Fecha));
            //    }
            //}

            //ViewBag.Fechas = fechas;


            var monedas = new List<Monedas>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM ingresos.dbo.Monedas", cn);
                using (SqlDataReader reader = cmdRemisioPago.ExecuteReader())
                {
                    while (reader.Read())
                    {


                        monedas.Add(new Monedas
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString()
                        });

                    }
                }
            }

            ViewBag.Monedas = monedas;
            ViewBag.Fecha = Fecha;
            ViewBag.IdAlmacen = IdAlmacen;


            return View();

        }


        //public ActionResult VCajaAlmacenesFechaMonedaEfectivo(string IdAlmacen, string Fecha, string IdMoneda)
        //PAGINA DE REPORTE DE VENTAS  
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMonedaEfectivo(string IdAlmacen, string Fecha, string IdMoneda)
        {

            ViewBag.ActivePage = "VCajaAlmacenes";

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var IDAlmacen = Convert.ToInt32(reader["IDAlmacen"]);
                    var almacenId = Convert.ToInt32(reader["AlmacenId"]);
                    var almacenNombre = reader["NombreAlmacen"].ToString();
                    almacenes.Add(new Tuple<int, string>(almacenId, almacenNombre));
                }
            }

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            ///

            var monedas = new List<Monedas>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM ingresos.dbo.Monedas", cn);
                using (SqlDataReader reader = cmdRemisioPago.ExecuteReader())
                {
                    while (reader.Read())
                    {


                        monedas.Add(new Monedas
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString()
                        });

                    }
                }
            }

            ViewBag.Monedas = monedas;

            //DateTime Fecha = DateTime.Now;



            var valoresEntregados = new List<ValoresEntregados>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.ValoresEntregados WHERE FechaInsercion=@Fecha", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {

                        valoresEntregados.Add(new ValoresEntregados
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            Importe = Convert.ToInt32(reader["Importe"]),
                            IdMonedaDenominacion = Convert.ToInt32(reader["IdMonedaDenominacion"]),
                            TipoCambio = Convert.ToInt32(reader["TipoCambio"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            IdMoneda = Convert.ToInt32(reader["IdMoneda"]),
                            FechaInsercion = Convert.ToDateTime(reader["FechaInsercion"]).ToString("yy-MM-dd"),
                            IdRegistroValoresEntregados = Convert.ToInt32(reader["IdRegistroValoresEntregados"])
                        });

                    }
                }
            }

            var registroValoresEntregados = new List<RegistroValoresEntregados>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.RegistroValoresEntregados WHERE Fecha=@Fecha", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {

                        registroValoresEntregados.Add(new RegistroValoresEntregados
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            IdMoneda = Convert.ToInt32(reader["IdMoneda"]),
                            TipoCambio = Convert.ToSingle(reader["TipoCambio"]),
                        });

                    }
                }
            }

            //Response.Write(valoresEntregados);

            //var json1 = new JavaScriptSerializer().Serialize(valoresEntregados);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //return null;

            //List<MonedaDenominacion> monedaDenominacion = new List<MonedaDenominacion>();
            //MonedaDenominacion monedaDenominacion = new MonedaDenominacion();
            var monedaDenominacion = new List<MonedaDenominacion>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.MonedaDenominacion", cn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        monedaDenominacion.Add(new MonedaDenominacion
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Denominacion = Convert.ToInt32(reader["Denominacion"]),
                            IdMoneda = Convert.ToInt32(reader["IdMoneda"]),
                        });
                    }
                }
            }

            ViewBag.MonedaDenominacion = monedaDenominacion;

            ViewBag.ValoresEntregados = valoresEntregados;
            ViewBag.RegistroValoresEntregados = registroValoresEntregados;
            ViewBag.Almacen = IdAlmacen;
            ViewBag.IdMoneda = Convert.ToInt32(IdMoneda);

            DateTime FechaHoy = DateTime.Now;
            //DateTime FechaHoy = Convert.ToDateTime("2024-10-10").ToString("yyyy-MM-dd");

            ViewBag.FechaHoy = FechaHoy.ToString("yyyy-MM-dd");


            //Fecha = "2024-06-15";
            ViewBag.Fecha = Fecha;

            //if (ViewBag.FechaHoy == ViewBag.Fecha)
            //{
            //    //do something
            //    return null;
            //}


            //
            //DateTime Fecha = DateTime.Now;
            //Fecha.ToString("yyyy-MM-dd");

            //if (!DateTime.TryParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
            //{
            //    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            //}

            //var cuadreEfectivo = new List<ViewValoresEntregados>();

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ObtenerCuadreEfectivo", cn);
            //    cmd.Parameters.AddWithValue("@Fecha", Fecha);

            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cn.Open();

            //    using (SqlDataReader reader = cmd.ExecuteReader())
            //    {


            //        while (reader.Read())
            //        {
            //            cuadreEfectivo.Add(new ViewValoresEntregados
            //            {
            //                Id = Convert.ToInt32(reader["Id"]),
            //                ImporteMonedaPesos = Convert.ToSingle(reader["ImporteMonedaPesos"]),
            //                ImporteMonedaOrigen = Convert.ToSingle(reader["ImporteMonedaOrigen"]),
            //                IdRegistroValoresEntregados = Convert.ToInt32(reader["IdRegistroValoresEntregados"]),
            //                MonedaNombre = reader["MonedaNombre"].ToString()
            //            });
            //        }
            //    }
            //}

            //ViewBag.CuadreEfectivo = cuadreEfectivo;

            //


            return View();

        }

        [HttpPost]
        public ActionResult procesarValoresEntregados2()
        {
            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            //int IdAlmacen = 0;
            //List<Tuple<int>> almacenes = new List<Tuple<int>>();

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
            //    cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cn.Open();

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
            //        almacenes.Add(new Tuple<int>(Convert.ToInt32(reader["AlmacenId"])));
            //    }
            //}

            //Response.Write(IdAlmacen);

            //var json3 = new JavaScriptSerializer().Serialize(almacenes);
            //Response.Write("yourObject:" + json3 + "<br/>");

            //return null;

            // se puede verificar que el id del almacen aparezca en la lista de permitidos.

            //List<Tuple<string, string,string>> data = new List<Tuple<string, string, string>>();
            List<string> denominacionList = new List<string>();
            List<string> cantidadList = new List<string>();
            List<string> importeList = new List<string>();
            List<int> valoresEntregadosList = new List<int>();

            int index = 0;

            double TipoCambio = 0;
            int IdMoneda = 0;


            string FechaParam = "";
            string IdAlmacenParam = "";
            string IdMonedaParam = "";

            foreach (string key in Request.Form.AllKeys)
            {
                string value = "";
                if (key.StartsWith("denominacion"))
                {
                    value = Request.Form[key];
                    denominacionList.Add(value);
                    index++;
                }
                if (key.StartsWith("cantidad"))
                {
                    value = Request.Form[key];
                    cantidadList.Add(value);
                }
                if (key.StartsWith("importe"))
                {
                    value = Request.Form[key];
                    importeList.Add(value);
                }
                if (key.StartsWith("TipoCambio"))
                {
                    TipoCambio = Convert.ToDouble(Request.Form[key]);
                }
                if (key.StartsWith("IdMoneda"))
                {
                    IdMoneda = Convert.ToInt32((Request.Form[key]));
                }
                if (key.StartsWith("IdValoresEntregados"))
                {
                    //IdValoresEntregados = Convert.ToInt32((Request.Form[key])); 
                    valoresEntregadosList.Add(Convert.ToInt32(Request.Form[key]));

                }
                if (key.StartsWith("FechaParam"))
                {
                    FechaParam = Request.Form[key];
                }
                if (key.StartsWith("IdAlmacenParam"))
                {
                    IdAlmacenParam = Request.Form[key];
                }
                if (key.StartsWith("IdMonedaParam"))
                {
                    IdMonedaParam = Request.Form[key];
                }
            }

            //Response.Write("index" + index);
            Response.Write(TipoCambio);
            //int IdAlmacen = 211;

            //return null;

            DateTime Fecha = DateTime.Now;
            Fecha.ToString("yyyy-MM-dd");

            bool IsValid = true;

            //IdValoresEntregados = 23;

            //Response.Write(IdValoresEntregados);

            //return null;

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    cn.Open();

            //    SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioEfectivo", cn);

            //    cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
            //    cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
            //    cmd.Parameters.AddWithValue("Fecha", Fecha);
            //    cmd.Parameters.AddWithValue("TipoCambio", TipoCambio);

            //    cmd.Parameters.Add("IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;
            //    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
            //    cmd.Parameters.Add("IdInsertadoRegistroValoresEntregados", SqlDbType.Int).Direction = ParameterDirection.Output;
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cmd.ExecuteNonQuery();

            //    IsValid = Convert.ToBoolean(cmd.Parameters["IsValid"].Value);
            //    IdRegistroValoresEntregados = Convert.ToInt32(cmd.Parameters["IdInsertadoRegistroValoresEntregados"].Value);

            //}

            if (IsValid)
            {
                Response.Write("actualizar");

                for (int i = 0; i < index; i++)
                {

                    Response.Write("for");

                    /*
                        [Id]
                      ,[Cantidad]
                      ,[Importe]
                      ,[IdMonedaDenominacion]
                      ,[FechaInsercion]
                      ,[TipoCambio]
                      ,[Almacen]
                      ,[IdMoneda]
                     */

                    //Response.Write(denominacionList[9]);
                    //Response.Write(cantidadList[9]);
                    //Response.Write(importeList[9]);

                    //int Cantidad = 10;
                    //float Importe = 10;
                    //int IdMonedaDenominacion = 1;

                    //obtener el almacen de la sesion.

                    //int IdMoneda = 1;
                    //TipoCambio = Convert.ToInt32(TipoCambio);

                    Response.Write(Fecha);

                    if (TipoCambio > 0 && IdMoneda > 0 && valoresEntregadosList[0] > 0)
                    {

                        //verificar primero que no haya datos existentes sql
                        //sql

                        using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                        {
                            cn.Open();

                            //calcular el importe aqui en el backend
                            float importe = 0;

                            SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ModificarEfectivo", cn);
                            // editar,
                            cmd.Parameters.AddWithValue("Cantidad", cantidadList[i]);
                            cmd.Parameters.AddWithValue("Importe", importeList[i]);
                            cmd.Parameters.AddWithValue("IdMonedaDenominacion", denominacionList[i]);
                            cmd.Parameters.AddWithValue("TipoCambio", TipoCambio);
                            cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacenParam);
                            cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                            cmd.Parameters.AddWithValue("Fecha", Fecha);
                            cmd.Parameters.AddWithValue("IdValoresEntregados", valoresEntregadosList[i]);

                            cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.ExecuteNonQuery();

                            //bool Actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                            //string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                            //if (Actualizado)
                            //{
                            //return Json(new { success = false, message = mensaje });
                            //return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("PerfilesForUsersAdminTotal", "AdminPages") });
                            //}
                            //else
                            //{
                            //return Json(new { success = false, message = mensaje });
                            //}

                        }
                    }
                    else
                    {
                        // ir a funcion de insertado.
                        //InsertarValoresDeEfectivo();
                        InsertarValoresDeEfectivo(IdMoneda, TipoCambio, index, denominacionList, cantidadList
                        , importeList, valoresEntregadosList);
                    }


                }

            }

            //return null;

            return RedirectToRoute("super-caja-almacenes-fecha-moneda", new { IdAlmacen = IdAlmacenParam, Fecha = FechaParam });

            //return RedirectToAction("VCajaAlmacenesFechaMonedaEfectivo", new { IdAlmacen = IdAlmacenParam, Fecha = FechaParam, IdMoneda = IdMonedaParam });

            //Response.Write(denominacionList[9]);
            //Response.Write(cantidadList[9]);
            //Response.Write(importeList[9]);

            // string combinedString = string.Join(",", listValues.ToArray());
            //Console.WriteLine("asd");

            //System.Console.WriteLine("asd");

            //var json1 = new JavaScriptSerializer().Serialize(denominacionList);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //var json2 = new JavaScriptSerializer().Serialize(cantidadList);
            //Response.Write("yourObject:" + json2 + "<br/>");

            //var json3 = new JavaScriptSerializer().Serialize(importeList);
            //Response.Write("yourObject:" + json3 + "<br/>");

            //Response.Write(Request.Form);

            //return RedirectToAction("VValoresEntregados", "AdminPages");

            //return Json(new { success = false, message = "asd" }, JsonRequestBehavior.AllowGet);
        }

        public void InsertarValoresDeEfectivo(int IdMoneda, double TipoCambio, int index, List<string> denominacionList, List<string> cantidadList
            , List<string> importeList, List<int> valoresEntregadosList)
        {
            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            int IdAlmacen = 0;
            List<Tuple<int>> almacenes = new List<Tuple<int>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(Convert.ToInt32(reader["AlmacenId"])));
                }
            }


            //int IdAlmacen = 211;

            //return null;

            DateTime Fecha = DateTime.Now;
            Fecha.ToString("yyyy-MM-dd");

            bool IsValid = false;
            int IdRegistroValoresEntregados = 0;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioEfectivo", cn);

                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                cmd.Parameters.AddWithValue("Fecha", Fecha);
                cmd.Parameters.AddWithValue("TipoCambio", TipoCambio);

                cmd.Parameters.Add("IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("IdInsertadoRegistroValoresEntregados", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                IsValid = Convert.ToBoolean(cmd.Parameters["IsValid"].Value);
                IdRegistroValoresEntregados = Convert.ToInt32(cmd.Parameters["IdInsertadoRegistroValoresEntregados"].Value);

            }

            if (IsValid)
            {
                Response.Write("insertar");

                for (int i = 0; i < index; i++)
                {

                    Response.Write("for");

                    /*
                        [Id]
                      ,[Cantidad]
                      ,[Importe]
                      ,[IdMonedaDenominacion]
                      ,[FechaInsercion]
                      ,[TipoCambio]
                      ,[Almacen]
                      ,[IdMoneda]
                     */

                    //Response.Write(denominacionList[9]);
                    //Response.Write(cantidadList[9]);
                    //Response.Write(importeList[9]);

                    //int Cantidad = 10;
                    //float Importe = 10;
                    //int IdMonedaDenominacion = 1;

                    //obtener el almacen de la sesion.

                    //int IdMoneda = 1;
                    //TipoCambio = Convert.ToInt32(TipoCambio);

                    Response.Write(Fecha);

                    if (TipoCambio > 0 && IdMoneda > 0)
                    {

                        //verificar primero que no haya datos existentes sql
                        //sql

                        using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                        {
                            cn.Open();

                            float importe = 0;

                            SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_RegistrarEfectivo", cn);

                            cmd.Parameters.AddWithValue("Cantidad", cantidadList[i]);
                            cmd.Parameters.AddWithValue("Importe", importeList[i]);
                            cmd.Parameters.AddWithValue("IdMonedaDenominacion", denominacionList[i]);
                            cmd.Parameters.AddWithValue("TipoCambio", TipoCambio);
                            cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                            cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                            cmd.Parameters.AddWithValue("Fecha", Fecha);
                            cmd.Parameters.AddWithValue("IdRegistroValoresEntregados", IdRegistroValoresEntregados);

                            cmd.Parameters.Add("Insertado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.ExecuteNonQuery();

                            //bool Insertado = Convert.ToBoolean(cmd.Parameters["Insertado"].Value);
                            //string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                            //if (Insertado)
                            //{
                            //  return Json(new { success = false, message = mensaje });
                            //return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("PerfilesForUsersAdminTotal", "AdminPages") });
                            //}
                            //else
                            //{
                            //    return Json(new { success = false, message = mensaje });
                            //}

                        }
                    }


                }

            }
        }

        [HttpPost]
        public ActionResult AjaxGetPaxByStaff(string fechaInicio, string fechaFin, string almacen)
        {

            DateTime fechaInicioParsed;
            DateTime fechaFinParsed;

            if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                !DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {

                SqlCommand cmd = new SqlCommand("SELECT * FROM mkt.dbo.VGetPaxByStaff WHERE fecha >= @fechaInicio AND fecha <=@fechaFin AND idalmacen=@idAlmacen", cn);
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@idAlmacen", almacen);

                //Response.Write(fechaInicio);
                //Response.Write(fechaFin);
                //Response.Write(almacen);

                cn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    var dejadas = new List<Dejadas>();


                    while (reader.Read())
                    {

                        dejadas.Add(new Dejadas
                        {
                            NombreStaff = reader["nombrestaff"].ToString(),
                            Fecha = Convert.ToDateTime(reader["fecha"]).ToString("yyyy-MM-dd"),
                            Pax = Convert.ToInt32(reader["pax"])
                        });


                    }

                    var jsonResult = Json(dejadas, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }

        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VVouchers()
        {

            ViewBag.ActivePage = "VVouchers";

            //return null;
            return View("/Views/SuperPages/VVouchers.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VReporteRegistroVentas()
        {
            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var almacenId = Convert.ToInt32(reader["AlmacenId"]);
                    var almacenNombre = reader["NombreAlmacen"].ToString();
                    almacenes.Add(new Tuple<int, string>(almacenId, almacenNombre));
                }
            }

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            return View("/Views/SuperPages/VReporteRegistroVentas.cshtml");

        }

        [HttpPost]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult getSalesByCategories()
        {
            string fecha = "";
            string idAlmacen = "";

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("fecha"))
                {
                    fecha = Request.Form[key];
                }
                if (key.StartsWith("idAlmacen"))
                {
                    idAlmacen = Request.Form[key];
                }
            }

            DateTime fechaParsed;

            if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
            {
                return Json(new { success = false, message = "Error de formato de fecha" }, JsonRequestBehavior.AllowGet);
            }

            //Convert.ToDateTime(reader["FechaInsercion"]).ToString("yy-MM-dd"),

            var ventasCategoria = new List<VentasCategoria>();



            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.VentasCategorias WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", fechaParsed.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", idAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        ventasCategoria.Add(new VentasCategoria
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IdRegistroVentas = Convert.ToInt32(reader["IdRegistroVentas"]),
                            Categoria = reader["Categoria"].ToString(),

                            Venta = Convert.ToSingle(reader["Venta"]),
                            Comisiones = Convert.ToSingle(reader["Comisiones"]),
                            NetoVenta = Convert.ToSingle(reader["NetoVenta"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd")
                        });

                    }
                }
            }

            var jsonResult = Json(ventasCategoria, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
            return jsonResult;

        }

    }
}