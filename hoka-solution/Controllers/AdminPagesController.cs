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
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Web.Services.Description;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Web.UI.WebControls.WebParts;

namespace hoka.Controllers
{
    public class AdminPagesController : Controller
    {
        // GET: AdminPages

        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();
        private static string DBIngresos = ConfigurationManager.ConnectionStrings["CadenaConexionHokaIngresos"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();
        private static string DBIngresosTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaIngresos"].ToString();


        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;
        private static string conexionDBIngresos;

        public AdminPagesController()
        {

            if (conexionDBDEV)
            {
                conexionDBHoka = DBHokaTEST;
                conexionDBJoy = DBJoyTEST;
                conexionDBIngresos = DBIngresosTEST;
            }
            else
            {
                conexionDBHoka = DBHoka;
                conexionDBJoy = DBJoy;
                conexionDBIngresos = DBIngresos;
            }
        }

        private static List<UsuarioModel> ListaUsers = new List<UsuarioModel>();
        private static List<RemisioDModel> ListaRemisioD = new List<RemisioDModel>();
        private static List<RolModel> ListaRoles = new List<RolModel>();
        private static List<EstadoModel> ListaEstados = new List<EstadoModel>();


        public ActionResult Index()
        {
            return View();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

        //CONTROLADOR Y PROCEDIMIENTOS DE ADMINISTRADOR

        //PAGINA PRINCIPAL

        [ValidarSesion(idRol: 1)]
        public ActionResult InicioAdmin()
        {

            ViewBag.ActivePage = "InicioAdmin";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return View();
        }


        //BOTON DESPEGABLE DE CONFIGURACION

        //CONFIGURACION DEL PERFIL DEL USUARIO        
        [HttpGet]
        [ValidarSesion(idRol: 1)]
        public ActionResult MiPerfilAdmin()
        {
            ViewBag.ActivePage = "MiPerfilAdmin";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
            int? idUsuario = usuarioSesion?.Id_Usuario;

            if (!idUsuario.HasValue)
            {
                // Manejo de error, tal vez redirigir a una página de error o a la lista de usuarios.
                return RedirectToAction("MiPerfilAdminTotal", "AdminPages");
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var allModels = new AllModels();

                // Inicializar las listas en AllModels
                allModels.RolesM = new List<RolModel>();
                allModels.PerfilM = new List<PerfilModel>();
                allModels.AlmacenesM = new List<almacenesModel>();
                allModels.EstadoUM = new List<EstadoModel>();
                allModels.UsuariosAlmacenesM = new List<UsuariosAlmacenesModel>();

                SqlCommand cmdUsuario = new SqlCommand(@"
        SELECT u.*, ua.Id_Almacen, a.Nombre, a.Almacen
        FROM usuariosweb u
        LEFT JOIN UsuariosWebAlmacenes ua ON u.Id_Usuario = ua.Id_Usuario
        LEFT JOIN almacenes a ON ua.Id_Almacen = a.Id_Almacen
        WHERE u.Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {
                    while (drUsuario.Read())
                    {
                        if (allModels.UsuarioMNoList == null)
                        {
                            UsuarioModel usuario = new UsuarioModel();
                            usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                            usuario.NombreU = drUsuario["NombreU"].ToString();
                            usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                            usuario.Telefono = drUsuario["Telefono"].ToString();
                            usuario.Correo = drUsuario["Correo"].ToString();
                            /*usuario.Clave = drUsuario["Clave"].ToString();*/

                            usuario.Id_Rol = Convert.ToInt32(drUsuario["Id_Rol"]);
                            usuario.Id_Perfil = drUsuario["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drUsuario["Id_Perfil"]) : 0;
                            usuario.Id_Estado = Convert.ToInt32(drUsuario["Id_Estado"]);

                            // Conversión de las fechas
                            string fechaNacimientoString = drUsuario["FechaNacimientoU"].ToString();
                            if (DateTime.TryParse(fechaNacimientoString, out DateTime FechaNacimientoU))
                            {
                                usuario.FechaNacimientoU = FechaNacimientoU;
                            }
                            else
                            {
                                usuario.FechaNacimientoU = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            string fechaRegistroString = drUsuario["FechaRegistroU"].ToString();
                            if (DateTime.TryParse(fechaRegistroString, out DateTime FechaRegistroU))
                            {
                                usuario.FechaRegistroU = FechaRegistroU;
                            }
                            else
                            {
                                usuario.FechaRegistroU = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allModels.UsuarioMNoList = usuario;
                        }

                        // Si hay un almacén asociado al usuario, agregarlo a la lista de almacenes
                        if (drUsuario["Id_Almacen"] != DBNull.Value)
                        {
                            almacenesModel almacen = new almacenesModel();
                            almacen.Id_Almacen = Convert.ToInt32(drUsuario["Id_Almacen"]);
                            almacen.Almacen = drUsuario["Almacen"].ToString();
                            almacen.Nombre = drUsuario["Nombre"].ToString();
                            allModels.AlmacenesM.Add(almacen);
                        }
                    }
                }

                // Consultas adicionales para obtener los nombres correspondientes a los IDs

                // Obtener los nombres de los Roles
                SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                {
                    while (drRoles.Read())
                    {
                        RolModel rol = new RolModel();
                        rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                        rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                        allModels.RolesM.Add(rol);
                    }
                    drRoles.Close();
                }

                // Obtener los nombres de los Perfiles
                SqlCommand cmdPerfiles = new SqlCommand("SELECT Id_Perfil, Nombre_Perfil FROM perfilesUserWeb", cn);
                using (SqlDataReader drPerfiles = cmdPerfiles.ExecuteReader())
                {
                    while (drPerfiles.Read())
                    {
                        PerfilModel perfil = new PerfilModel();
                        perfil.Id_Perfil = drPerfiles["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drPerfiles["Id_Perfil"]) : 0;
                        perfil.Id_Perfil = Convert.ToInt32(drPerfiles["Id_Perfil"]);
                        perfil.Nombre_Perfil = drPerfiles["Nombre_Perfil"].ToString();
                        allModels.PerfilM.Add(perfil);
                    }
                    drPerfiles.Close();
                }

                // Obtener los nombres de los Estados
                SqlCommand cmdEstados = new SqlCommand("SELECT Id_Estado, Nombre_Estado FROM estadoweb", cn);
                using (SqlDataReader drEstados = cmdEstados.ExecuteReader())
                {
                    while (drEstados.Read())
                    {
                        EstadoModel estado = new EstadoModel();
                        estado.Id_Estado = Convert.ToInt32(drEstados["Id_Estado"]);
                        estado.Nombre_Estado = drEstados["Nombre_Estado"].ToString();
                        allModels.EstadoUM.Add(estado);
                    }
                    drEstados.Close();
                }

                if (allModels.UsuarioMNoList == null)
                {
                    return RedirectToAction("MiPerfilAdminTotal", "AdminPages");
                }

                return View(allModels);
            }
        }





        //PAGINA DE VENTAS POR PRODUCTO
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VProductosAdmin()
        {

            ViewBag.ActivePage = "VProductosAdmin";

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

            return View();
        }

        //PAGINA DE REPORTE DE VENTA POR ALMACEN
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 2)]
        public ActionResult VAlmacenAdmin()
        {

            ViewBag.ActivePage = "VAlmacenAdmin";
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

            return View();
        }

        //PAGINA DE REPORTE DE VENTA POR TIPO DE PAGO
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 7)]
        public ActionResult VTipoPagoAdmin()
        {
            ViewBag.ActivePage = "VTipoPagoAdmin";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnDeptoPago", cn);
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

            return View();
        }

        //PAGINA DE REPORTE DE VENTA POR VENDEDOR
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 3)]
        public ActionResult VVendedorAdmin()
        {

            ViewBag.ActivePage = "VVendedorAdmin";

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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioYVendedor", cn);
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

            return View();
        }


        //BOTON DESPEGABLE DE REPORTES JOYERIA

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 4)]
        public ActionResult VProductosJoyAdmin()
        {

            ViewBag.ActivePage = "VProductosJoyAdmin";

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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnJoyeria", cn);
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

            return View();
        }

        //PAGINA DE GRAFICAS
        [ValidarSesion(idRol: 1, permisos: 5)]
        public ActionResult GProductosAdmin()
        {

            ViewBag.ActivePage = "GProductosAdmin";
            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return View();
        }



        //BOTON DESPEGABLE DE CONSULTAS
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 8)]
        public ActionResult CTicketsAdmin()
        {
            ViewBag.ActivePage = "CTicketsAdmin";


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

            return View();
        }


        //VISTA SABER LOS DETALLES DEL TICKET DE LA VISTA CTicketsAdmin
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 8)]
        public ActionResult DetalleTicketAdmin(string idFolio)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsAdmin";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idFolio == null)
                return RedirectToAction("DetalleTicketAdminTotal", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                var allHokaModels = new AllHokaModels();
                allHokaModels.RemisioMM = new List<RemisioMModel>();
                allHokaModels.RemisioMPagoM = new List<RemisioMPagoModel>();
                allHokaModels.RemisioDyMM = new List<RemisioDyMModel>();
                allHokaModels.RemisioMVendedorM = new List<RemisioMVendedorModel>();

                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM VRemisioMPagoMWeb WHERE folio_factura = @folio", cn);
                cmdRemisioPago.Parameters.AddWithValue("@folio", idFolio);
                using (SqlDataReader drRemisioPago = cmdRemisioPago.ExecuteReader())
                {
                    while (drRemisioPago.Read())
                    {
                        RemisioMPagoModel remisiopago = new RemisioMPagoModel();

                        remisiopago.folio_factura = drRemisioPago["folio_factura"].ToString();
                        remisiopago.total = Convert.ToDouble(drRemisioPago["total"]);
                        remisiopago.NombreMoneda = drRemisioPago["NombreMoneda"].ToString();
                        remisiopago.NombreAlmacen = drRemisioPago["NombreAlmacen"].ToString();

                        // Conversión de las fechas
                        string fechaPagostring = drRemisioPago["fecha_pago"].ToString();
                        if (DateTime.TryParse(fechaPagostring, out DateTime fecha_pago))
                        {
                            remisiopago.fecha_pago = fecha_pago;
                        }
                        else
                        {
                            remisiopago.fecha_pago = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        allHokaModels.RemisioMPagoM.Add(remisiopago);
                    }

                    drRemisioPago.Close();


                    // Consultas adicionales para obtener las tablas con el idFolio

                    // Obtener
                    SqlCommand cmdRemisioProductos = new SqlCommand("SELECT * FROM VRemisioDyMWeb WHERE folio_remision = @folio", cn);
                    cmdRemisioProductos.Parameters.AddWithValue("@folio", idFolio);
                    using (SqlDataReader drRemisioProductoss = cmdRemisioProductos.ExecuteReader())
                    {
                        while (drRemisioProductoss.Read())
                        {
                            RemisioDyMModel remisioproducto = new RemisioDyMModel();
                            remisioproducto.folio_remision = drRemisioProductoss["folio_remision"].ToString();
                            remisioproducto.descripcion_larga = drRemisioProductoss["descripcion_larga"].ToString();
                            remisioproducto.codigobarras = drRemisioProductoss["codigobarras"].ToString();
                            remisioproducto.NombreAlmacen = drRemisioProductoss["NombreAlmacen"].ToString();
                            remisioproducto.cantidads = Convert.ToDouble(drRemisioProductoss["cantidads"]);
                            remisioproducto.deportiva = drRemisioProductoss["deportiva"].ToString();

                            // Conversión de las fechas
                            string fechastring = drRemisioProductoss["fecha"].ToString();
                            if (DateTime.TryParse(fechastring, out DateTime fecha))
                            {
                                remisioproducto.fecha = fecha;
                            }
                            else
                            {
                                remisioproducto.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allHokaModels.RemisioDyMM.Add(remisioproducto);
                        }
                        drRemisioProductoss.Close(); //tabla de los productos


                        // Obtener tabla de los vendedores
                        SqlCommand cmdRemisioVendedores = new SqlCommand("SELECT * FROM VRemisioMVendedor WHERE folio_factura = @folio", cn);
                        cmdRemisioVendedores.Parameters.AddWithValue("@folio", idFolio);
                        using (SqlDataReader drRemisioVendedores = cmdRemisioVendedores.ExecuteReader())
                        {
                            while (drRemisioVendedores.Read())
                            {
                                RemisioMVendedorModel remisiovendedor = new RemisioMVendedorModel();
                                remisiovendedor.folio_factura = drRemisioVendedores["folio_factura"].ToString();
                                remisiovendedor.NombreVendedor = drRemisioVendedores["NombreVendedor"].ToString();
                                remisiovendedor.NombreAlmacen = drRemisioVendedores["NombreAlmacen"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioVendedores["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisiovendedor.fecha = fecha2;
                                }
                                else
                                {
                                    remisiovendedor.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMVendedorM.Add(remisiovendedor);
                            }
                            drRemisioVendedores.Close();
                        }

                        // Obtener dato observacion
                        SqlCommand cmdRemisioMObser = new SqlCommand("SELECT * FROM VRemisioMWeb WHERE folio_remision = @folio", cn);
                        cmdRemisioMObser.Parameters.AddWithValue("@folio", idFolio);
                        using (SqlDataReader drRemisioMObser = cmdRemisioMObser.ExecuteReader())
                        {
                            while (drRemisioMObser.Read())
                            {
                                RemisioMModel remisioMObser = new RemisioMModel();
                                remisioMObser.folio_remision = drRemisioMObser["folio_remision"].ToString();
                                remisioMObser.observaciones = drRemisioMObser["observaciones"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioMObser["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisioMObser.fecha = fecha2;
                                }
                                else
                                {
                                    remisioMObser.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMM.Add(remisioMObser);
                            }
                            drRemisioMObser.Close();
                        }
                    }
                }

                List<AllHokaModels> listaAllModels = new List<AllHokaModels> { allHokaModels };

                return View(listaAllModels);
            }
        }


        //BOTON DESPEGABLE DE CONSULTAS PARA JOYERIA

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 9)]
        public ActionResult CTicketsJoyAdmin()
        {
            ViewBag.ActivePage = "CTicketsJoyAdmin";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnRemisioMJoy", cn);
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

            return View();
        }


        //VISTA SABER LOS DETALLES DEL TICKET DE LA VISTA CTicketsAdminTotal
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 9)]
        public ActionResult DetalleTicketJoyAdmin(string idFolio, string almacen)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsJoyAdmin";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idFolio == null)
                return RedirectToAction("DetalleTicketJoyAdmin", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                cn.Open();
                var allHokaModels = new AllHokaModels();
                allHokaModels.RemisioMM = new List<RemisioMModel>();
                allHokaModels.RemisioMPagoM = new List<RemisioMPagoModel>();
                allHokaModels.RemisioDyMM = new List<RemisioDyMModel>();
                allHokaModels.RemisioMVendedorM = new List<RemisioMVendedorModel>();

                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM VRemisioMPagoMWebJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                cmdRemisioPago.Parameters.AddWithValue("@folio", idFolio);
                cmdRemisioPago.Parameters.AddWithValue("@almacen", almacen);
                using (SqlDataReader drRemisioPago = cmdRemisioPago.ExecuteReader())
                {
                    while (drRemisioPago.Read())
                    {
                        RemisioMPagoModel remisiopago = new RemisioMPagoModel();

                        remisiopago.folio_factura = drRemisioPago["folio_factura"].ToString();
                        remisiopago.total = Convert.ToDouble(drRemisioPago["total"]);
                        remisiopago.NombreMoneda = drRemisioPago["NombreMoneda"].ToString();
                        remisiopago.NombreAlmacen = drRemisioPago["NombreAlmacen"].ToString();

                        // Conversión de las fechas
                        string fechaPagostring = drRemisioPago["fecha_pago"].ToString();
                        if (DateTime.TryParse(fechaPagostring, out DateTime fecha_pago))
                        {
                            remisiopago.fecha_pago = fecha_pago;
                        }
                        else
                        {
                            remisiopago.fecha_pago = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        allHokaModels.RemisioMPagoM.Add(remisiopago);
                    }

                    drRemisioPago.Close();


                    // Consultas adicionales para obtener las tablas con el idFolio

                    // Obtener tabla de los productos
                    SqlCommand cmdRemisioProductos = new SqlCommand("SELECT * FROM VRemisioDyMWebJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                    cmdRemisioProductos.Parameters.AddWithValue("@folio", idFolio);
                    cmdRemisioProductos.Parameters.AddWithValue("@almacen", almacen);
                    using (SqlDataReader drRemisioProductoss = cmdRemisioProductos.ExecuteReader())
                    {
                        while (drRemisioProductoss.Read())
                        {
                            RemisioDyMModel remisioproducto = new RemisioDyMModel();
                            remisioproducto.folio_factura = drRemisioProductoss["folio_factura"].ToString();
                            remisioproducto.descripcion_larga = drRemisioProductoss["descripcion_larga"].ToString();
                            remisioproducto.codigo_barras = drRemisioProductoss["codigo_barras"].ToString();
                            remisioproducto.NombreAlmacen = drRemisioProductoss["NombreAlmacen"].ToString();
                            remisioproducto.cantidads = Convert.ToDouble(drRemisioProductoss["cantidads"]);
                            remisioproducto.deportiva = drRemisioProductoss["deportiva"].ToString();

                            // Conversión de las fechas
                            string fechastring = drRemisioProductoss["fecha"].ToString();
                            if (DateTime.TryParse(fechastring, out DateTime fecha))
                            {
                                remisioproducto.fecha = fecha;
                            }
                            else
                            {
                                remisioproducto.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allHokaModels.RemisioDyMM.Add(remisioproducto);
                        }
                        drRemisioProductoss.Close();

                        // Obtener tabla de los vendedores
                        SqlCommand cmdRemisioVendedores = new SqlCommand("SELECT * FROM VRemisioMVendedorJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                        cmdRemisioVendedores.Parameters.AddWithValue("@folio", idFolio);
                        cmdRemisioVendedores.Parameters.AddWithValue("@almacen", almacen);
                        using (SqlDataReader drRemisioVendedores = cmdRemisioVendedores.ExecuteReader())
                        {
                            while (drRemisioVendedores.Read())
                            {
                                RemisioMVendedorModel remisiovendedor = new RemisioMVendedorModel();
                                remisiovendedor.folio_factura = drRemisioVendedores["folio_factura"].ToString();
                                remisiovendedor.NombreVendedor = drRemisioVendedores["NombreVendedor"].ToString();
                                remisiovendedor.NombreAlmacen = drRemisioVendedores["NombreAlmacen"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioVendedores["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisiovendedor.fecha = fecha2;
                                }
                                else
                                {
                                    remisiovendedor.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMVendedorM.Add(remisiovendedor);
                            }
                            drRemisioVendedores.Close();
                        }

                        // Obtener dato observacion
                        SqlCommand cmdRemisioMObser = new SqlCommand("SELECT * FROM joyeria.dbo.VRemisioMWebJoy WHERE folio_factura = @folio and almacen = @almacen", cn);
                        cmdRemisioMObser.Parameters.AddWithValue("@folio", idFolio);
                        cmdRemisioMObser.Parameters.AddWithValue("@almacen", almacen);
                        using (SqlDataReader drRemisioMObser = cmdRemisioMObser.ExecuteReader())
                        {
                            while (drRemisioMObser.Read())
                            {
                                RemisioMModel remisioMObser = new RemisioMModel();
                                remisioMObser.folio_factura = drRemisioMObser["folio_factura"].ToString();
                                remisioMObser.observaciones = drRemisioMObser["observaciones"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioMObser["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisioMObser.fecha = fecha2;
                                }
                                else
                                {
                                    remisioMObser.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMM.Add(remisioMObser);
                            }
                            drRemisioMObser.Close();
                        }
                    }
                }

                List<AllHokaModels> listaAllModels = new List<AllHokaModels> { allHokaModels };

                return View(listaAllModels);
            }
        }


        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 12)]
        public ActionResult CRotacionPAdmin()
        {
            ViewBag.ActivePage = "CRotacionPAdmin";


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

            return View();
        }


        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 13)]
        public ActionResult CRotacionPJoyAdmin()
        {
            ViewBag.ActivePage = "CRotacionPJoyAdmin";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnJoyeria", cn);
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

            return View();
        }


        //son post
        [HttpPost]
        [ValidarSesion(idRol: 1)]
        public ActionResult EditarMiPerfilPersonalAdmin(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            if (string.IsNullOrEmpty(ObjUsuario.NombreU))
            {
                return Json(new { success = false, message = "El Nombre es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ApellidoU))
            {
                return Json(new { success = false, message = "El Apellido es obligatorio" });
            }

            DateTime fechaNacimientoParsed;

            // Intenta parsear la fecha de nacimiento
            if (!DateTime.TryParseExact(FechaNacimientoU, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimientoParsed))
            {
                return Json(new { success = false, message = "Formato de fecha de nacimiento incorrecto." });
            }


            // Valida si la fecha de nacimiento está en el rango permitido
            DateTime minDate = new DateTime(1920, 1, 1);
            DateTime maxDate = DateTime.Today;
            if (fechaNacimientoParsed < minDate || fechaNacimientoParsed > maxDate)
            {
                return Json(new { success = false, message = "La fecha de nacimiento no es válida" });
            }



            // Validar el campo de teléfono
            if (string.IsNullOrEmpty(ObjUsuario.Telefono) || !EsNumero(ObjUsuario.Telefono) || ObjUsuario.Telefono.Length != 10)
            {
                return Json(new { success = false, message = "El teléfono debe tener 10 dígitos numéricos" });
            }


            // Validar la fecha de nacimiento (mayores de 18 años)
            if (ObjUsuario.FechaNacimientoU > DateTime.Today.AddYears(-18))
            {
                return Json(new { success = false, message = "Debe ser mayor de 18 años para registrarse" });
            }


            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarMiUsuarioDatosPersonales", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("NombreU", ObjUsuario.NombreU);
                cmd.Parameters.AddWithValue("ApellidoU", ObjUsuario.ApellidoU);
                ObjUsuario.FechaNacimientoU = fechaNacimientoParsed;// Asegúrate de que la propiedad FechaNacimientoU sea de tipo DateTime
                cmd.Parameters.AddWithValue("FechaNacimientoU", ObjUsuario.FechaNacimientoU);
                cmd.Parameters.AddWithValue("Telefono", ObjUsuario.Telefono);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "Tu usuario se ha actualizado";

                    // Verifica si el usuario en sesión es el mismo que el que se está editando
                    UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
                    if (usuarioSesion != null && usuarioSesion.Id_Usuario == ObjUsuario.Id_Usuario)
                    {
                        // Actualizar la sesión con la nueva información
                        Session["NombreUser"] = ObjUsuario.NombreU;
                    }

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("InicioAdmin", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }
        [HttpPost]
        [ValidarSesion(idRol: 2)]
        public ActionResult EditarMiPerfilPersonalUser(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            if (string.IsNullOrEmpty(ObjUsuario.NombreU))
            {
                return Json(new { success = false, message = "El Nombre es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ApellidoU))
            {
                return Json(new { success = false, message = "El Apellido es obligatorio" });
            }

            DateTime fechaNacimientoParsed;

            // Intenta parsear la fecha de nacimiento
            if (!DateTime.TryParseExact(FechaNacimientoU, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimientoParsed))
            {
                return Json(new { success = false, message = "Formato de fecha de nacimiento incorrecto." });
            }


            // Valida si la fecha de nacimiento está en el rango permitido
            DateTime minDate = new DateTime(1920, 1, 1);
            DateTime maxDate = DateTime.Today;
            if (fechaNacimientoParsed < minDate || fechaNacimientoParsed > maxDate)
            {
                return Json(new { success = false, message = "La fecha de nacimiento no es válida" });
            }



            // Validar el campo de teléfono
            if (string.IsNullOrEmpty(ObjUsuario.Telefono) || !EsNumero(ObjUsuario.Telefono) || ObjUsuario.Telefono.Length != 10)
            {
                return Json(new { success = false, message = "El teléfono debe tener 10 dígitos numéricos" });
            }


            // Validar la fecha de nacimiento (mayores de 18 años)
            if (ObjUsuario.FechaNacimientoU > DateTime.Today.AddYears(-18))
            {
                return Json(new { success = false, message = "Debe ser mayor de 18 años para registrarse" });
            }


            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarMiUsuarioDatosPersonales", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("NombreU", ObjUsuario.NombreU);
                cmd.Parameters.AddWithValue("ApellidoU", ObjUsuario.ApellidoU);
                ObjUsuario.FechaNacimientoU = fechaNacimientoParsed;// Asegúrate de que la propiedad FechaNacimientoU sea de tipo DateTime
                cmd.Parameters.AddWithValue("FechaNacimientoU", ObjUsuario.FechaNacimientoU);
                cmd.Parameters.AddWithValue("Telefono", ObjUsuario.Telefono);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "Tu usuario se ha actualizado";

                    // Verifica si el usuario en sesión es el mismo que el que se está editando
                    UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
                    if (usuarioSesion != null && usuarioSesion.Id_Usuario == ObjUsuario.Id_Usuario)
                    {
                        // Actualizar la sesión con la nueva información
                        Session["NombreUser"] = ObjUsuario.NombreU;
                    }

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("InicioAdmin", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


        //CONTROLADOR Y PROCEDIMIENTOS DE ADMINISTRADOR TOTAL

        //PAGINA PRINCIPAL

        [ValidarSesion(idRol: 3)]
        public ActionResult InicioAdminTotal()
        {
            ViewBag.ActivePage = "InicioAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return View();
        }


        //BOTON DESPEGABLE DE CONFIGURACION

        //CONFIGURACION DEL PERFIL DEL USUARIO        
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult MiPerfilAdminTotal()
        {
            ViewBag.ActivePage = "MiPerfilAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
            int? idUsuario = usuarioSesion?.Id_Usuario;

            if (!idUsuario.HasValue)
            {
                // Manejo de error, tal vez redirigir a una página de error o a la lista de usuarios.
                return RedirectToAction("MiPerfilAdminTotal", "AdminPages");
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var allModels = new AllModels();

                // Inicializar las listas en AllModels
                allModels.RolesM = new List<RolModel>();
                allModels.PerfilM = new List<PerfilModel>();
                allModels.AlmacenesM = new List<almacenesModel>();
                allModels.EstadoUM = new List<EstadoModel>();
                allModels.UsuariosAlmacenesM = new List<UsuariosAlmacenesModel>();

                SqlCommand cmdUsuario = new SqlCommand(@"
        SELECT u.*, ua.Id_Almacen, a.Nombre, a.Almacen
        FROM usuariosweb u
        LEFT JOIN UsuariosWebAlmacenes ua ON u.Id_Usuario = ua.Id_Usuario
        LEFT JOIN almacenes a ON ua.Id_Almacen = a.Id_Almacen
        WHERE u.Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {
                    while (drUsuario.Read())
                    {
                        if (allModels.UsuarioMNoList == null)
                        {
                            UsuarioModel usuario = new UsuarioModel();
                            usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                            usuario.NombreU = drUsuario["NombreU"].ToString();
                            usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                            usuario.Telefono = drUsuario["Telefono"].ToString();
                            usuario.Correo = drUsuario["Correo"].ToString();
                            /*usuario.Clave = drUsuario["Clave"].ToString();*/

                            usuario.Id_Rol = Convert.ToInt32(drUsuario["Id_Rol"]);
                            usuario.Id_Perfil = drUsuario["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drUsuario["Id_Perfil"]) : 0;
                            usuario.Id_Estado = Convert.ToInt32(drUsuario["Id_Estado"]);

                            // Conversión de las fechas
                            string fechaNacimientoString = drUsuario["FechaNacimientoU"].ToString();
                            if (DateTime.TryParse(fechaNacimientoString, out DateTime FechaNacimientoU))
                            {
                                usuario.FechaNacimientoU = FechaNacimientoU;
                            }
                            else
                            {
                                usuario.FechaNacimientoU = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            string fechaRegistroString = drUsuario["FechaRegistroU"].ToString();
                            if (DateTime.TryParse(fechaRegistroString, out DateTime FechaRegistroU))
                            {
                                usuario.FechaRegistroU = FechaRegistroU;
                            }
                            else
                            {
                                usuario.FechaRegistroU = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allModels.UsuarioMNoList = usuario;
                        }

                        // Si hay un almacén asociado al usuario, agregarlo a la lista de almacenes
                        if (drUsuario["Id_Almacen"] != DBNull.Value)
                        {
                            almacenesModel almacen = new almacenesModel();
                            almacen.Id_Almacen = Convert.ToInt32(drUsuario["Id_Almacen"]);
                            almacen.Almacen = drUsuario["Almacen"].ToString();
                            almacen.Nombre = drUsuario["Nombre"].ToString();
                            allModels.AlmacenesM.Add(almacen);
                        }
                    }
                }

                // Consultas adicionales para obtener los nombres correspondientes a los IDs

                // Obtener los nombres de los Roles
                SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                {
                    while (drRoles.Read())
                    {
                        RolModel rol = new RolModel();
                        rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                        rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                        allModels.RolesM.Add(rol);
                    }
                    drRoles.Close();
                }

                // Obtener los nombres de los Perfiles
                SqlCommand cmdPerfiles = new SqlCommand("SELECT Id_Perfil, Nombre_Perfil FROM perfilesUserWeb", cn);
                using (SqlDataReader drPerfiles = cmdPerfiles.ExecuteReader())
                {
                    while (drPerfiles.Read())
                    {
                        PerfilModel perfil = new PerfilModel();
                        perfil.Id_Perfil = drPerfiles["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drPerfiles["Id_Perfil"]) : 0;
                        perfil.Id_Perfil = Convert.ToInt32(drPerfiles["Id_Perfil"]);
                        perfil.Nombre_Perfil = drPerfiles["Nombre_Perfil"].ToString();
                        allModels.PerfilM.Add(perfil);
                    }
                    drPerfiles.Close();
                }

                // Obtener los nombres de los Estados
                SqlCommand cmdEstados = new SqlCommand("SELECT Id_Estado, Nombre_Estado FROM estadoweb", cn);
                using (SqlDataReader drEstados = cmdEstados.ExecuteReader())
                {
                    while (drEstados.Read())
                    {
                        EstadoModel estado = new EstadoModel();
                        estado.Id_Estado = Convert.ToInt32(drEstados["Id_Estado"]);
                        estado.Nombre_Estado = drEstados["Nombre_Estado"].ToString();
                        allModels.EstadoUM.Add(estado);
                    }
                    drEstados.Close();
                }

                if (allModels.UsuarioMNoList == null)
                {
                    return RedirectToAction("MiPerfilAdminTotal", "AdminPages");
                }

                return View(allModels);
            }
        }

        //VISTA PARA EDITAR LA CONTRASEÑA DEL USUARIO
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult MiContraseñaAdminTotal()
        {
            ViewBag.ActivePage = "MiContraseñaAdminTotal";
            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
            int? idUsuario = usuarioSesion?.Id_Usuario;

            if (idUsuario == null)
                return RedirectToAction("InicioAdminTotal", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                var allModels = new AllModels();

                SqlCommand cmdUsuario = new SqlCommand("SELECT * FROM usuariosWeb WHERE Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {


                    while (drUsuario.Read())
                    {
                        UsuarioModel usuario = new UsuarioModel();

                        usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                        usuario.NombreU = drUsuario["NombreU"].ToString();
                        usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                        usuario.Telefono = drUsuario["Telefono"].ToString();
                        usuario.Correo = drUsuario["Correo"].ToString();

                        allModels.UsuarioMNoList = usuario;
                    }

                    drUsuario.Close();
                }

                if (allModels.UsuarioMNoList == null)
                {
                    return RedirectToAction("InicioAdminTotal", "AdminPages");
                }

                return View(allModels);
            }
        }


        //PAGINA DE LISTA USUARIOS O CRUD
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult LUsuariosAdminTotal(string buscar, int? page)
        {
            ViewBag.ActivePage = "LUsuariosAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            int pageSize = 10; // Cantidad de elementos por página
            int pageNumber = (page ?? 1); // Número de página actual

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                // Obtener el número total de registros
                string countQuery = "SELECT COUNT(*) FROM usuariosweb WHERE NombreU LIKE '%' + @buscar + '%' OR ApellidoU LIKE '%' + @buscar + '%' OR Correo LIKE '%' + @buscar + '%'";
                SqlCommand countCmd = new SqlCommand(countQuery, cn);
                countCmd.Parameters.AddWithValue("@buscar", buscar ?? "");
                cn.Open();
                int totalRecords = (int)countCmd.ExecuteScalar();

                // Calcular el número total de páginas
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Calcular el desplazamiento (offset) para la consulta SQL
                int offset = (pageNumber - 1) * pageSize;

                // Consulta SQL paginada
                string query = @"
            SELECT *
            FROM (
                SELECT ROW_NUMBER() OVER (ORDER BY Id_Usuario) AS RowNumber, *
                FROM usuariosweb
                WHERE NombreU LIKE '%' + @buscar + '%' OR ApellidoU LIKE '%' + @buscar + '%' OR Correo LIKE '%' + @buscar + '%'
            ) AS T
            WHERE T.RowNumber BETWEEN @startRow AND @endRow";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@buscar", buscar ?? "");
                cmd.Parameters.AddWithValue("@startRow", offset + 1);
                cmd.Parameters.AddWithValue("@endRow", offset + pageSize);
                cmd.CommandType = CommandType.Text;


                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var allModels = new AllModels();

                    // Inicializar las listas en AllModels
                    allModels.UsuariosM = new List<UsuarioModel>();
                    allModels.RolesM = new List<RolModel>();
                    allModels.PerfilM = new List<PerfilModel>();
                    allModels.AlmacenesM = new List<almacenesModel>();
                    allModels.PermisosM = new List<PermisosModel>();
                    allModels.EstadoUM = new List<EstadoModel>();

                    while (dr.Read())
                    {
                        UsuarioModel usuario = new UsuarioModel();
                        usuario.Id_Usuario = Convert.ToInt32(dr["Id_Usuario"]);
                        usuario.NombreU = dr["NombreU"].ToString();
                        usuario.ApellidoU = dr["ApellidoU"].ToString();
                        usuario.Telefono = dr["Telefono"].ToString();
                        usuario.Correo = dr["Correo"].ToString();
                        usuario.Clave = dr["Clave"].ToString();
                        usuario.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);
                        usuario.Id_Perfil = dr["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(dr["Id_Perfil"]) : 0;
                        //aqui deberia ir lo de almacenes
                        usuario.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);

                        // Conversión de las fechas
                        string fechaNacimientoString = dr["FechaNacimientoU"].ToString();
                        if (DateTime.TryParse(fechaNacimientoString, out DateTime FechaNacimientoU))
                        {
                            usuario.FechaNacimientoU = FechaNacimientoU;
                        }
                        else
                        {
                            usuario.FechaNacimientoU = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        string fechaRegistroString = dr["FechaRegistroU"].ToString();
                        if (DateTime.TryParse(fechaRegistroString, out DateTime FechaRegistroU))
                        {
                            usuario.FechaRegistroU = FechaRegistroU;
                        }
                        else
                        {
                            usuario.FechaRegistroU = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        allModels.UsuariosM.Add(usuario);
                    }

                    dr.Close();

                    // Consultas adicionales para obtener los nombres correspondientes a los IDs

                    // Obtener los nombres de los Roles
                    SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                    using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                    {
                        while (drRoles.Read())
                        {
                            RolModel rol = new RolModel();
                            rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                            rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                            allModels.RolesM.Add(rol);
                        }
                        drRoles.Close();
                    }

                    // Obtener los nombres de los Perfiles
                    SqlCommand cmdPerfiles = new SqlCommand("SELECT Id_Perfil, Nombre_Perfil FROM perfilesUserWeb", cn);
                    using (SqlDataReader drPerfiles = cmdPerfiles.ExecuteReader())
                    {
                        while (drPerfiles.Read())
                        {
                            PerfilModel perfil = new PerfilModel();
                            perfil.Id_Perfil = drPerfiles["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drPerfiles["Id_Perfil"]) : 0;
                            perfil.Nombre_Perfil = drPerfiles["Nombre_Perfil"].ToString();
                            allModels.PerfilM.Add(perfil);
                        }
                        drPerfiles.Close();
                    }

                    // Obtener los nombres de los almacenes
                    SqlCommand cmdAlmacenes = new SqlCommand("SELECT Id_Almacen, Almacen ,Nombre FROM almacenes", cn);
                    using (SqlDataReader drAlmacenes = cmdAlmacenes.ExecuteReader())
                    {
                        while (drAlmacenes.Read())
                        {
                            almacenesModel almacen = new almacenesModel();
                            almacen.Id_Almacen = Convert.ToInt32(drAlmacenes["Id_Almacen"]);
                            almacen.Almacen = drAlmacenes["Almacen"].ToString(); //Id por la sucursal
                            almacen.Nombre = drAlmacenes["Nombre"].ToString();
                            allModels.AlmacenesM.Add(almacen);
                        }
                        drAlmacenes.Close();
                    }

                    // Obtener los nombres de los permisos
                    SqlCommand cmdPermisos = new SqlCommand("SELECT Id_Permiso, Nombre_Permiso FROM permisosweb", cn);
                    using (SqlDataReader drPermisos = cmdPermisos.ExecuteReader())
                    {
                        while (drPermisos.Read())
                        {
                            PermisosModel permiso = new PermisosModel();
                            permiso.Id_Permiso = Convert.ToInt32(drPermisos["Id_Permiso"]);
                            permiso.Nombre_Permiso = drPermisos["Nombre_Permiso"].ToString();
                            allModels.PermisosM.Add(permiso);
                        }
                        drPermisos.Close();
                    }

                    // Obtener los nombres de los Estados
                    SqlCommand cmdEstados = new SqlCommand("SELECT Id_Estado, Nombre_Estado FROM estadoweb", cn);
                    using (SqlDataReader drEstados = cmdEstados.ExecuteReader())
                    {
                        while (drEstados.Read())
                        {
                            EstadoModel estado = new EstadoModel();
                            estado.Id_Estado = Convert.ToInt32(drEstados["Id_Estado"]);
                            estado.Nombre_Estado = drEstados["Nombre_Estado"].ToString();
                            allModels.EstadoUM.Add(estado);
                        }
                        drEstados.Close();
                    }

                    // Aquí puedes obtener y asignar los datos para las demás propiedades en AllModels, si es necesario

                    List<AllModels> listaAllModels = new List<AllModels> { allModels };

                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageNumber;
                    ViewBag.Buscar = buscar;

                    return View(listaAllModels);
                }
            }
        }

        //CONFIGURACION DE LOS PERFILES DE USUARIOS (ETIQUETAS)
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult PerfilesForUsersAdminTotal(string buscar, int? page)
        {
            ViewBag.ActivePage = "PerfilesForUsersAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            int pageSize = 10; // Cantidad de elementos por página
            int pageNumber = (page ?? 1); // Número de página actual

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                // Obtener el número total de registros
                string countQuery = "SELECT COUNT(*) FROM perfilesUserWeb WHERE Nombre_Perfil LIKE '%' + @buscar + '%' ";
                SqlCommand countCmd = new SqlCommand(countQuery, cn);
                countCmd.Parameters.AddWithValue("@buscar", buscar ?? "");
                cn.Open();
                int totalRecords = (int)countCmd.ExecuteScalar();

                // Calcular el número total de páginas
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                // Calcular el desplazamiento (offset) para la consulta SQL
                int offset = (pageNumber - 1) * pageSize;

                // Consulta SQL paginada
                string query = @"
            SELECT *
            FROM (
                SELECT ROW_NUMBER() OVER (ORDER BY Id_Perfil) AS RowNumber, *
                FROM perfilesUserWeb
                WHERE Nombre_Perfil LIKE '%' + @buscar + '%'
            ) AS T
            WHERE T.RowNumber BETWEEN @startRow AND @endRow";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@buscar", buscar ?? "");
                cmd.Parameters.AddWithValue("@startRow", offset + 1);
                cmd.Parameters.AddWithValue("@endRow", offset + pageSize);
                cmd.CommandType = CommandType.Text;


                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var allModels = new AllModels();

                    // Inicializar las listas en AllModels
                    allModels.RolesM = new List<RolModel>();
                    allModels.PerfilM = new List<PerfilModel>();

                    while (dr.Read())
                    {
                        PerfilModel perfil = new PerfilModel();
                        perfil.Id_Perfil = Convert.ToInt32(dr["Id_Perfil"]);
                        perfil.Nombre_Perfil = dr["Nombre_Perfil"].ToString();
                        perfil.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);


                        allModels.PerfilM.Add(perfil);
                    }

                    dr.Close();

                    // Consultas adicionales para obtener los nombres correspondientes a los IDs

                    // Obtener los nombres de los Roles
                    SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                    using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                    {
                        while (drRoles.Read())
                        {
                            RolModel rol = new RolModel();
                            rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                            rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                            allModels.RolesM.Add(rol);
                        }
                        drRoles.Close();
                    }
                    // Aquí puedes obtener y asignar los datos para las demás propiedades en AllModels, si es necesario

                    List<AllModels> listaAllModels = new List<AllModels> { allModels };

                    ViewBag.TotalPages = totalPages;
                    ViewBag.CurrentPage = pageNumber;
                    ViewBag.Buscar = buscar;

                    return View(listaAllModels);
                }
            }
        }

        //VISTA PARA EDITAR EL USUARIO
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarUsuario(int? idUsuario)
        {


            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var allModels = new AllModels();

                SqlCommand cmdUsuario = new SqlCommand(@"
            SELECT *
            FROM VUsuariosRelacion
            WHERE Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {
                    while (drUsuario.Read())
                    {
                        if (allModels.UsuarioMNoList == null)
                        {
                            UsuarioModel usuario = new UsuarioModel();
                            usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                            usuario.NombreU = drUsuario["NombreU"].ToString();
                            usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                            usuario.Telefono = drUsuario["Telefono"].ToString();
                            usuario.Correo = drUsuario["Correo"].ToString();

                            // Conversión de las fechas
                            string fechaNacimientoString = drUsuario["FechaNacimientoU"].ToString();
                            if (DateTime.TryParse(fechaNacimientoString, out DateTime FechaNacimientoU))
                            {
                                usuario.FechaNacimientoU = FechaNacimientoU;
                            }
                            else
                            {
                                usuario.FechaNacimientoU = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }
                            allModels.UsuarioMNoList = usuario;
                        }
                    }
                }

                return View(allModels);
            }
        }

        //VISTA PARA EDITAR EL USUARIO
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarPermisos(int? idUsuario)
        {

            ViewBag.ActivePage = "LUsuariosAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            /*if (idUsuario == null)
                return RedirectToAction("LUsuariosAdminTotal", "AdminPages");*/

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var allModels = new AllModels();

                // Inicializar las listas en AllModels
                allModels.RolesM = new List<RolModel>();
                allModels.PerfilM = new List<PerfilModel>();
                allModels.AlmacenesM = new List<almacenesModel>();
                allModels.PermisosM = new List<PermisosModel>();
                allModels.EstadoUM = new List<EstadoModel>();
                allModels.UsuariosAlmacenesM = new List<UsuariosAlmacenesModel>(); // Agregar esta inicialización
                allModels.UsuariosPermisosM = new List<UsuariosPermisosModel>(); // Agregar esta inicialización

                SqlCommand cmdUsuario = new SqlCommand("SELECT * FROM usuariosweb WHERE Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {
                    if (drUsuario.Read())
                    {
                        UsuarioModel usuario = new UsuarioModel();
                        usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                        usuario.NombreU = drUsuario["NombreU"].ToString();
                        usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                        usuario.Correo = drUsuario["Correo"].ToString();

                        usuario.Id_Rol = Convert.ToInt32(drUsuario["Id_Rol"]);
                        usuario.Id_Perfil = drUsuario["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drUsuario["Id_Perfil"]) : 0;
                        usuario.Id_Estado = Convert.ToInt32(drUsuario["Id_Estado"]);

                        allModels.UsuarioMNoList = usuario;
                    }
                    drUsuario.Close();
                }

                // Consultas adicionales para obtener los nombres correspondientes a los IDs
                // Obtener los nombres de los Almacenes

                SqlCommand cmdAlmacenes = new SqlCommand("SELECT a.* FROM almacenes a INNER JOIN UsuariosWebAlmacenes ua ON a.Id_Almacen = ua.Id_Almacen WHERE ua.Id_Usuario = @Id_Usuario", cn);
                cmdAlmacenes.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drAlmacenes = cmdAlmacenes.ExecuteReader())
                {
                    while (drAlmacenes.Read())
                    {
                        almacenesModel almacen = new almacenesModel();
                        almacen.Id_Almacen = Convert.ToInt32(drAlmacenes["Id_Almacen"]);
                        almacen.Almacen = drAlmacenes["Almacen"].ToString();
                        almacen.Nombre = drAlmacenes["Nombre"].ToString();

                        allModels.AlmacenesM.Add(almacen);
                    }
                    drAlmacenes.Close();
                }

                //Obtener los nombres de los permisos

                SqlCommand cmdPermisos = new SqlCommand("SELECT p.* FROM permisosweb p INNER JOIN UsuariosWebPermisos up ON p.Id_Permiso = up.Id_Permiso WHERE up.Id_Usuario = @Id_Usuario", cn);
                cmdPermisos.Parameters.AddWithValue("@Id_Usuario", idUsuario);

                using (SqlDataReader drPermisos = cmdPermisos.ExecuteReader())
                {
                    while (drPermisos.Read())
                    {
                        PermisosModel permiso = new PermisosModel();
                        permiso.Id_Permiso = Convert.ToInt32(drPermisos["Id_Permiso"]);
                        permiso.Nombre_Permiso = drPermisos["Nombre_Permiso"].ToString();

                        allModels.PermisosM.Add(permiso);
                    }
                    drPermisos.Close();
                }

                // Obtener los nombres de los Roles
                SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                {
                    while (drRoles.Read())
                    {
                        RolModel rol = new RolModel();
                        rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                        rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                        allModels.RolesM.Add(rol);
                    }
                    drRoles.Close();
                }

                // Obtener los nombres de los Perfiles
                SqlCommand cmdPerfiles = new SqlCommand("SELECT Id_Perfil, Nombre_Perfil FROM perfilesUserWeb", cn);
                using (SqlDataReader drPerfiles = cmdPerfiles.ExecuteReader())
                {
                    while (drPerfiles.Read())
                    {
                        PerfilModel perfil = new PerfilModel();
                        perfil.Id_Perfil = drPerfiles["Id_Perfil"] != DBNull.Value ? Convert.ToInt32(drPerfiles["Id_Perfil"]) : 0;
                        perfil.Nombre_Perfil = drPerfiles["Nombre_Perfil"].ToString();
                        allModels.PerfilM.Add(perfil);
                    }
                    drPerfiles.Close();
                }

                // Obtener los nombres de los Estados
                SqlCommand cmdEstados = new SqlCommand("SELECT Id_Estado, Nombre_Estado FROM estadoweb", cn);
                using (SqlDataReader drEstados = cmdEstados.ExecuteReader())
                {
                    while (drEstados.Read())
                    {
                        EstadoModel estado = new EstadoModel();
                        estado.Id_Estado = Convert.ToInt32(drEstados["Id_Estado"]);
                        estado.Nombre_Estado = drEstados["Nombre_Estado"].ToString();
                        allModels.EstadoUM.Add(estado);
                    }
                    drEstados.Close();
                }

                /*if (allModels.UsuarioMNoList == null)
                {
                    return RedirectToAction("LUsuariosAdminTotal", "AdminPages");
                }*/

                return View(allModels);
            }
        }

        //VISTA PARA EDITAR LA CONTRASEÑA DEL USUARIO
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarContraseña(int? idUsuario)
        {

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idUsuario == null)
                return RedirectToAction("LUsuariosAdminTotal", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                var allModels = new AllModels();

                SqlCommand cmdUsuario = new SqlCommand("SELECT * FROM usuariosWeb WHERE Id_Usuario = @Id_Usuario", cn);
                cmdUsuario.Parameters.AddWithValue("@Id_Usuario", idUsuario);
                using (SqlDataReader drUsuario = cmdUsuario.ExecuteReader())
                {
                    while (drUsuario.Read())
                    {
                        UsuarioModel usuario = new UsuarioModel();

                        usuario.Id_Usuario = Convert.ToInt32(drUsuario["Id_Usuario"]);
                        usuario.NombreU = drUsuario["NombreU"].ToString();
                        usuario.ApellidoU = drUsuario["ApellidoU"].ToString();
                        usuario.Telefono = drUsuario["Telefono"].ToString();
                        usuario.Correo = drUsuario["Correo"].ToString();

                        allModels.UsuarioMNoList = usuario;
                    }

                    drUsuario.Close();
                }

                if (allModels.UsuarioMNoList == null)
                {
                    return RedirectToAction("LUsuariosAdminTotal", "AdminPages");
                }

                return View(allModels);
            }
        }

        //VISTA PARA EDITAR EL PERFIL DEL USUARIO USUARIO
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarPerfil(int? idPerfil)
        {
            ViewBag.ActivePage = "PerfilesForUsersAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();


            if (idPerfil == null)
                return RedirectToAction("PerfilesForUsersAdminTotal", "AdminPages");


            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var allModels = new AllModels();

                // Inicializar las listas en AllModels
                allModels.RolesM = new List<RolModel>();
                allModels.PerfilM = new List<PerfilModel>();

                SqlCommand cmdPerfiles = new SqlCommand(@"SELECT * FROM perfilesUserWeb WHERE Id_Perfil = @Id_Perfil", cn);
                cmdPerfiles.Parameters.AddWithValue("@Id_Perfil", idPerfil);
                using (SqlDataReader drPerfiles = cmdPerfiles.ExecuteReader())
                {
                    while (drPerfiles.Read())
                    {

                        PerfilModel perfil = new PerfilModel();
                        perfil.Id_Perfil = Convert.ToInt32(drPerfiles["Id_Perfil"]);
                        perfil.Nombre_Perfil = drPerfiles["Nombre_Perfil"].ToString();
                        perfil.Id_Rol = Convert.ToInt32(drPerfiles["Id_Rol"]);

                        allModels.PerfilMNoList = perfil;

                    }
                }

                // Consultas adicionales para obtener los nombres correspondientes a los IDs

                // Obtener los nombres de los Roles
                SqlCommand cmdRoles = new SqlCommand("SELECT Id_Rol, Nombre_Rol FROM rolesweb", cn);
                using (SqlDataReader drRoles = cmdRoles.ExecuteReader())
                {
                    while (drRoles.Read())
                    {
                        RolModel rol = new RolModel();
                        rol.Id_Rol = Convert.ToInt32(drRoles["Id_Rol"]);
                        rol.Nombre_Rol = drRoles["Nombre_Rol"].ToString();
                        allModels.RolesM.Add(rol);
                    }
                    drRoles.Close();
                }

                if (allModels.PerfilMNoList == null)
                {
                    return RedirectToAction("PerfilesForUsersAdminTotal", "AdminPages");
                }

                return View(allModels);
            }
        }

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VReporteSalesByAgenciesAdmin()
        {

            ViewBag.ActivePage = "VReporteSalesByAgenciesAdmin";

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

            return View();
        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VReporteSalesByAgenciesAdminTotal()
        {

            ViewBag.ActivePage = "VReporteSalesByAgenciesAdminTotal";

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

            return View();
        }

        //FILTRO 
        // fn hugo
        [HttpPost]
        public ActionResult SalesByAgenciesAndGuides(string fechaInicio, string fechaFin, string almacen)
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

                //string baseQuery = @"SELECT * FROM SalesByAgenciesAndGuides
                //WHERE fecha >= @fechaInicio AND fecha <= @fechaFin and almacen=@almacen";

                //SqlCommand cmd = new SqlCommand(baseQuery, cn);

                //cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                //cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                //cmd.Parameters.AddWithValue("@almacen", almacen ?? (object)DBNull.Value);

                //cmd.CommandType = CommandType.Text;

                SqlCommand cmd = new SqlCommand("compuadmo.dbo.sp_SalesByAgenciesAndGuides", cn);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.Parameters.AddWithValue("@fecha1", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fecha2", fechaFinParsed);
                cmd.CommandType = CommandType.StoredProcedure;


                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<SalesByAgenciesAndGuides> rawData = new List<SalesByAgenciesAndGuides>();

                    ArrayList list = new ArrayList();

                    while (dr.Read())
                    {
                        SalesByAgenciesAndGuides response = new SalesByAgenciesAndGuides();


                        response.Group_estatus = dr["estatus"].ToString();
                        response.Group_total = dr["total"] != DBNull.Value ? Convert.ToDouble(dr["total"]) : 0;
                        response.Group_descuento = dr["descuento"] != DBNull.Value ? Convert.ToDouble(dr["descuento"]) : 0;
                        response.Group_matricula = dr["matricula"] != DBNull.Value ? Convert.ToInt32(dr["matricula"]) : 0;
                        response.Group_agencia = dr["agencia"].ToString();
                        response.Group_nom_guia = dr["nom_guia"].ToString();
                        response.Group_producto = dr["producto"] != DBNull.Value ? Convert.ToInt32(dr["producto"]) : 0;
                        response.Group_stotal = dr["stotal"] != DBNull.Value ? Convert.ToDouble(dr["stotal"]) : 0;
                        response.Group_descripcion_larga = dr["descripcion_larga"].ToString();
                        response.Group_folio_remision = dr["folio_remision"].ToString();

                        //response.Group_pax = dr["pax"] != DBNull.Value ? Convert.ToInt32(dr["pax"]) : 0;


                        if (response.Group_producto == 26966)
                        {
                            response.Group_tipo_venta = "Calendario";
                            response.Group_cantidad_vendida = dr["cantidad_vendida"] != DBNull.Value ? Convert.ToDouble(dr["cantidad_vendida"]) : 0;
                            response.Group_calendarios = dr["total"] != DBNull.Value ? Convert.ToDouble(dr["total"]) : 0;
                            response.Group_venta_tienda = 0;
                            //response.total_venta = response.calendarios;
                            response.Group_total_venta = dr["total"] != DBNull.Value ? Convert.ToDouble(dr["total"]) : 0;
                        }
                        else
                        {
                            response.Group_tipo_venta = "Producto";
                            response.Group_cantidad_vendida = dr["cantidad_vendida"] != DBNull.Value ? Convert.ToDouble(dr["cantidad_vendida"]) : 0;
                            response.Group_calendarios = 0;
                            response.Group_venta_tienda = dr["total"] != DBNull.Value ? Convert.ToDouble(dr["total"]) : 0;
                            response.Group_total_venta = dr["total"] != DBNull.Value ? Convert.ToDouble(dr["total"]) : 0;
                        }


                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime x_date))
                        {
                            response.Group_fecha = x_date.ToUniversalTime();
                        }
                        else
                        {
                            response.Group_fecha = DateTime.MinValue;
                        }

                        if (!list.Contains(response.Group_folio_remision))
                        {

                            list.Add(response.Group_folio_remision);
                            rawData.Add(response);
                        }

                    }



                    //var groupedData = rawData.GroupBy(d => new
                    //{

                    //    d.Group_folio_remision,
                    //    d.Group_total,
                    //    d.Group_descuento,
                    //    d.Group_matricula,
                    //    d.Group_agencia,
                    //    d.Group_nom_guia,
                    //    d.Group_producto,
                    //    d.Group_stotal,
                    //    d.Group_descripcion_larga,

                    //    d.Group_tipo_venta,
                    //    d.Group_cantidad_vendida,
                    //    d.Group_calendarios,
                    //    d.Group_venta_tienda,
                    //    d.Group_total_venta,
                    //    d.Group_fecha

                    //})
                    //.Select(g => new SalesByAgenciesAndGuides
                    //{
                    //    Group_matricula = g.Key.Group_matricula,
                    //    Group_agencia = g.Key.Group_agencia,
                    //    Group_nom_guia = g.Key.Group_nom_guia,

                    //    Group_producto = g.Key.Group_producto,
                    //    Group_stotal = g.Key.Group_stotal,
                    //    Group_descripcion_larga = g.Key.Group_descripcion_larga,
                    //    Group_folio_remision = g.Key.Group_folio_remision,

                    //    Group_tipo_venta = g.Key.Group_tipo_venta,
                    //    Group_cantidad_vendida = g.Key.Group_cantidad_vendida,
                    //    Group_calendarios = g.Key.Group_calendarios,
                    //    Group_venta_tienda = g.Key.Group_venta_tienda,
                    //    Group_total_venta = g.Key.Group_total_venta,
                    //    Group_fecha = g.Key.Group_fecha

                    //})
                    //.ToList();


                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }












        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VReporteTaquillaAdminTotal()
        {

            ViewBag.ActivePage = "VReporteTaquillaAdminTotal";

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

            return View();
        }

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VReporteTaquillaAdmin()
        {

            ViewBag.ActivePage = "VReporteTaquillaAdmin";

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

            return View();
        }



        //BOTON DESPEGABLE DE REPORTES VENTAS

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VProductosAdminTotal()
        {

            ViewBag.ActivePage = "VProductosAdminTotal";

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

            return View();
        }


        //PAGINA DE REPORTE DE PRODUCTO NO BENDIDOS
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 1)]
        public ActionResult VProductosNoVendidosAdminTotal()
        {

            ViewBag.ActivePage = "VProductosNoVendidosAdminTotal";

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
            {// Llamada a un procedimiento almacenado para obtener almacenes por usuari
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

            return View();
        }



        //PAGINA DE REPORTE DE VENTA POR ALMACEN
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 2)]
        public ActionResult VAlmacenAdminTotal()
        {
            ViewBag.ActivePage = "VAlmacenAdminTotal";


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

            return View();
        }




        //PAGINA DE REPORTE DE VENTA POR TIPO DE PAGO
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 7)]
        public ActionResult VTipoPagoAdminTotal()
        {
            ViewBag.ActivePage = "VTipoPagoAdminTotal";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnDeptoPago", cn);
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

            return View();
        }


        //PAGINA DE REPORTE DE VENTA POR VENDEDOR
        [ValidarSesion(idRol: 3, permisos: 3)]
        public ActionResult VVendedorAdminTotal()
        {

            ViewBag.ActivePage = "VVendedorAdminTotal";

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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioYVendedor", cn);
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

            return View();
        }


        //BOTON DESPEGABLE DE REPORTES JOYERIA

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 4)]
        public ActionResult VProductosJoyAdminTotal()
        {

            ViewBag.ActivePage = "VProductosJoyAdminTotal";

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

            // revisar hugo, esta funcion no conecta a base de datos, podrian ser los stores o alguna tabla faltante, vista joyeria, reporte venta por producto
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnJoyeria", cn);
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

            return View();
        }

        //BOTON DESPEGABLE DE CONSULTAS

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 8)]
        public ActionResult CTicketsAdminTotal()
        {
            ViewBag.ActivePage = "CTicketsAdminTotal";


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

            return View();
        }


        //VISTA SABER LOS DETALLES DEL TICKET DE LA VISTA CTicketsAdminTotal
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 8)]
        public ActionResult DetalleTicketAdminTotal(string idFolio)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idFolio == null)
                return RedirectToAction("DetalleTicketAdminTotal", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                var allHokaModels = new AllHokaModels();
                allHokaModels.RemisioMM = new List<RemisioMModel>();
                allHokaModels.RemisioMPagoM = new List<RemisioMPagoModel>();
                allHokaModels.RemisioDyMM = new List<RemisioDyMModel>();
                allHokaModels.RemisioMVendedorM = new List<RemisioMVendedorModel>();

                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM VRemisioMPagoMWeb WHERE folio_factura = @folio", cn);
                cmdRemisioPago.Parameters.AddWithValue("@folio", idFolio);
                using (SqlDataReader drRemisioPago = cmdRemisioPago.ExecuteReader())
                {
                    while (drRemisioPago.Read())
                    {
                        RemisioMPagoModel remisiopago = new RemisioMPagoModel();

                        remisiopago.folio_factura = drRemisioPago["folio_factura"].ToString();
                        remisiopago.total = Convert.ToDouble(drRemisioPago["total"]);
                        remisiopago.NombreMoneda = drRemisioPago["NombreMoneda"].ToString();
                        remisiopago.NombreAlmacen = drRemisioPago["NombreAlmacen"].ToString();

                        // Conversión de las fechas
                        string fechaPagostring = drRemisioPago["fecha_pago"].ToString();
                        if (DateTime.TryParse(fechaPagostring, out DateTime fecha_pago))
                        {
                            remisiopago.fecha_pago = fecha_pago;
                        }
                        else
                        {
                            remisiopago.fecha_pago = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        allHokaModels.RemisioMPagoM.Add(remisiopago);
                    }

                    drRemisioPago.Close();


                    // Consultas adicionales para obtener las tablas con el idFolio

                    // Obtener tabla de los productos
                    SqlCommand cmdRemisioProductos = new SqlCommand("SELECT * FROM VRemisioDyMWeb WHERE folio_remision = @folio", cn);
                    cmdRemisioProductos.Parameters.AddWithValue("@folio", idFolio);
                    using (SqlDataReader drRemisioProductoss = cmdRemisioProductos.ExecuteReader())
                    {
                        while (drRemisioProductoss.Read())
                        {
                            RemisioDyMModel remisioproducto = new RemisioDyMModel();
                            remisioproducto.folio_remision = drRemisioProductoss["folio_remision"].ToString();
                            remisioproducto.descripcion_larga = drRemisioProductoss["descripcion_larga"].ToString();
                            remisioproducto.codigobarras = drRemisioProductoss["codigobarras"].ToString();
                            remisioproducto.NombreAlmacen = drRemisioProductoss["NombreAlmacen"].ToString();
                            remisioproducto.cantidads = Convert.ToDouble(drRemisioProductoss["cantidads"]);
                            remisioproducto.deportiva = drRemisioProductoss["deportiva"].ToString();

                            // Conversión de las fechas
                            string fechastring = drRemisioProductoss["fecha"].ToString();
                            if (DateTime.TryParse(fechastring, out DateTime fecha))
                            {
                                remisioproducto.fecha = fecha;
                            }
                            else
                            {
                                remisioproducto.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allHokaModels.RemisioDyMM.Add(remisioproducto);
                        }
                        drRemisioProductoss.Close();

                        // Obtener tabla de los vendedores
                        SqlCommand cmdRemisioVendedores = new SqlCommand("SELECT * FROM VRemisioMVendedor WHERE folio_factura = @folio", cn);
                        cmdRemisioVendedores.Parameters.AddWithValue("@folio", idFolio);
                        using (SqlDataReader drRemisioVendedores = cmdRemisioVendedores.ExecuteReader())
                        {
                            while (drRemisioVendedores.Read())
                            {
                                RemisioMVendedorModel remisiovendedor = new RemisioMVendedorModel();
                                remisiovendedor.folio_factura = drRemisioVendedores["folio_factura"].ToString();
                                remisiovendedor.NombreVendedor = drRemisioVendedores["NombreVendedor"].ToString();
                                remisiovendedor.NombreAlmacen = drRemisioVendedores["NombreAlmacen"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioVendedores["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisiovendedor.fecha = fecha2;
                                }
                                else
                                {
                                    remisiovendedor.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMVendedorM.Add(remisiovendedor);
                            }
                            drRemisioVendedores.Close();
                        }

                        // Obtener dato observacion
                        SqlCommand cmdRemisioMObser = new SqlCommand("SELECT * FROM VRemisioMWeb WHERE folio_remision = @folio", cn);
                        cmdRemisioMObser.Parameters.AddWithValue("@folio", idFolio);
                        using (SqlDataReader drRemisioMObser = cmdRemisioMObser.ExecuteReader())
                        {
                            while (drRemisioMObser.Read())
                            {
                                RemisioMModel remisioMObser = new RemisioMModel();
                                remisioMObser.folio_remision = drRemisioMObser["folio_remision"].ToString();
                                remisioMObser.observaciones = drRemisioMObser["observaciones"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioMObser["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisioMObser.fecha = fecha2;
                                }
                                else
                                {
                                    remisioMObser.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMM.Add(remisioMObser);
                            }
                            drRemisioMObser.Close();
                        }
                    }
                }

                List<AllHokaModels> listaAllModels = new List<AllHokaModels> { allHokaModels };

                return View(listaAllModels);
            }
        }


        //BOTON DESPEGABLE DE PARA LA VISTA DE CONSULTAS PARA JOYERIA

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 9)]
        public ActionResult CTicketsJoyAdminTotal()
        {
            ViewBag.ActivePage = "CTicketsJoyAdminTotal";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnRemisioMJoy", cn);
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

            return View();
        }


        //VISTA SABER LOS DETALLES DEL TICKET DE LA VISTA CTicketsAdminTotal
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 9)]
        public ActionResult DetalleTicketJoyAdminTotal(string idFolio, string almacen)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsJoyAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idFolio == null)
                return RedirectToAction("DetalleTicketJoyAdminTotal", "AdminPages");

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                cn.Open();
                var allHokaModels = new AllHokaModels();
                allHokaModels.RemisioMM = new List<RemisioMModel>();
                allHokaModels.RemisioMPagoM = new List<RemisioMPagoModel>();
                allHokaModels.RemisioDyMM = new List<RemisioDyMModel>();
                allHokaModels.RemisioMVendedorM = new List<RemisioMVendedorModel>();

                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM joyeria.dbo.VRemisioMPagoMWebJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                cmdRemisioPago.Parameters.AddWithValue("@folio", idFolio);
                cmdRemisioPago.Parameters.AddWithValue("@almacen", almacen);
                using (SqlDataReader drRemisioPago = cmdRemisioPago.ExecuteReader())
                {
                    while (drRemisioPago.Read())
                    {
                        RemisioMPagoModel remisiopago = new RemisioMPagoModel();

                        remisiopago.folio_factura = drRemisioPago["folio_factura"].ToString();
                        remisiopago.total = Convert.ToDouble(drRemisioPago["total"]);
                        remisiopago.NombreMoneda = drRemisioPago["NombreMoneda"].ToString();
                        remisiopago.NombreAlmacen = drRemisioPago["NombreAlmacen"].ToString();

                        // Conversión de las fechas
                        string fechaPagostring = drRemisioPago["fecha_pago"].ToString();
                        if (DateTime.TryParse(fechaPagostring, out DateTime fecha_pago))
                        {
                            remisiopago.fecha_pago = fecha_pago;
                        }
                        else
                        {
                            remisiopago.fecha_pago = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        allHokaModels.RemisioMPagoM.Add(remisiopago);
                    }

                    drRemisioPago.Close();


                    // Consultas adicionales para obtener las tablas con el idFolio

                    // Obtener tabla de los productos
                    SqlCommand cmdRemisioProductos = new SqlCommand("SELECT * FROM joyeria.dbo.VRemisioDyMWebJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                    cmdRemisioProductos.Parameters.AddWithValue("@folio", idFolio);
                    cmdRemisioProductos.Parameters.AddWithValue("@almacen", almacen);
                    using (SqlDataReader drRemisioProductoss = cmdRemisioProductos.ExecuteReader())
                    {
                        while (drRemisioProductoss.Read())
                        {
                            RemisioDyMModel remisioproducto = new RemisioDyMModel();
                            remisioproducto.folio_factura = drRemisioProductoss["folio_factura"].ToString();
                            remisioproducto.descripcion_larga = drRemisioProductoss["descripcion_larga"].ToString();
                            remisioproducto.codigo_barras = drRemisioProductoss["codigo_barras"].ToString();
                            remisioproducto.NombreAlmacen = drRemisioProductoss["NombreAlmacen"].ToString();
                            remisioproducto.cantidads = Convert.ToDouble(drRemisioProductoss["cantidads"]);
                            remisioproducto.deportiva = drRemisioProductoss["deportiva"].ToString();

                            // Conversión de las fechas
                            string fechastring = drRemisioProductoss["fecha"].ToString();
                            if (DateTime.TryParse(fechastring, out DateTime fecha))
                            {
                                remisioproducto.fecha = fecha;
                            }
                            else
                            {
                                remisioproducto.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                            }

                            allHokaModels.RemisioDyMM.Add(remisioproducto);
                        }
                        drRemisioProductoss.Close();

                        // Obtener tabla de los vendedores
                        SqlCommand cmdRemisioVendedores = new SqlCommand("SELECT * FROM joyeria.dbo.VRemisioMVendedorJoy WHERE folio_factura = @folio and NombreAlmacen = @almacen", cn);
                        cmdRemisioVendedores.Parameters.AddWithValue("@folio", idFolio);
                        cmdRemisioVendedores.Parameters.AddWithValue("@almacen", almacen);
                        using (SqlDataReader drRemisioVendedores = cmdRemisioVendedores.ExecuteReader())
                        {
                            while (drRemisioVendedores.Read())
                            {
                                RemisioMVendedorModel remisiovendedor = new RemisioMVendedorModel();
                                remisiovendedor.folio_factura = drRemisioVendedores["folio_factura"].ToString();
                                remisiovendedor.NombreVendedor = drRemisioVendedores["NombreVendedor"].ToString();
                                remisiovendedor.NombreAlmacen = drRemisioVendedores["NombreAlmacen"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioVendedores["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisiovendedor.fecha = fecha2;
                                }
                                else
                                {
                                    remisiovendedor.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMVendedorM.Add(remisiovendedor);
                            }
                            drRemisioVendedores.Close();
                        }

                        // Obtener dato observacion
                        SqlCommand cmdRemisioMObser = new SqlCommand("SELECT * FROM joyeria.dbo.VRemisioMWebJoy WHERE folio_factura = @folio and almacen = @almacen", cn);
                        cmdRemisioMObser.Parameters.AddWithValue("@folio", idFolio);
                        cmdRemisioMObser.Parameters.AddWithValue("@almacen", almacen);
                        using (SqlDataReader drRemisioMObser = cmdRemisioMObser.ExecuteReader())
                        {
                            while (drRemisioMObser.Read())
                            {
                                RemisioMModel remisioMObser = new RemisioMModel();
                                remisioMObser.folio_factura = drRemisioMObser["folio_factura"].ToString();
                                remisioMObser.observaciones = drRemisioMObser["observaciones"].ToString();

                                // Conversión de las fechas
                                string fechastring2 = drRemisioMObser["fecha"].ToString();
                                if (DateTime.TryParse(fechastring2, out DateTime fecha2))
                                {
                                    remisioMObser.fecha = fecha2;
                                }
                                else
                                {
                                    remisioMObser.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                                }

                                allHokaModels.RemisioMM.Add(remisioMObser);
                            }
                            drRemisioMObser.Close();
                        }
                    }
                }

                List<AllHokaModels> listaAllModels = new List<AllHokaModels> { allHokaModels };

                return View(listaAllModels);
            }
        }



        //INVENTARIO Y ROTACION

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 10)]
        public ActionResult CInventariosAdminTotal()
        {
            ViewBag.ActivePage = "CInventariosAdminTotal";


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

            return View();
        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 12)]
        public ActionResult CRotacionPAdminTotal()
        {
            ViewBag.ActivePage = "CRotacionPAdminTotal";


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

            return View();
        }




        //JOYERIA ROTACION E INVENTARIOS
        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 11)]
        public ActionResult CInventariosJoyAdminTotal()
        {
            return View();
        }

        [HttpGet]
        [ValidarSesion(idRol: 3, permisos: 13)]
        public ActionResult CRotacionPJoyAdminTotal()
        {
            ViewBag.ActivePage = "CRotacionPJoyAdminTotal";


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
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuarioEnJoyeria", cn);
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

            return View();
        }




        //BOTON DESPEGABLE DE GRAFICAS

        //PAGINA DE GRAFICAS
        [ValidarSesion(idRol: 3, permisos: 5)]
        public ActionResult GProductosAdminTotal()
        {

            ViewBag.ActivePage = "GProductosAdminTotal";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return View();
        }





        //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------





        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VAlmacenAdminTotal
        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VAlmacenAdminTotal
        [HttpPost]
        public ActionResult filtrarTablaA(string fechaInicio, string fechaFin, string almacen)
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
                cn.Open();

                // 1. OBTENER EL CONTEO DE TICKETS POR DÍA
                string countQuery = @"SELECT CAST(fecha AS DATE) as FechaDia, COUNT(*) as ConteoDia 
                            FROM remisioM
                            WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen
                            GROUP BY CAST(fecha AS DATE)";

                SqlCommand countCmd = new SqlCommand(countQuery, cn);
                countCmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                countCmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                countCmd.Parameters.AddWithValue("@almacen", almacen);

                Dictionary<DateTime, int> conteoPorDia = new Dictionary<DateTime, int>();
                using (SqlDataReader countReader = countCmd.ExecuteReader())
                {
                    while (countReader.Read())
                    {
                        DateTime fechaDia = Convert.ToDateTime(countReader["FechaDia"]);
                        int conteo = Convert.ToInt32(countReader["ConteoDia"]);
                        conteoPorDia[fechaDia] = conteo;
                    }
                }

                // 2. OBTENER LOS DATOS DE VENTAS
                string baseQuery = @"SELECT * FROM VRemisioDM
                            WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);
                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<RemisioDModel> rawData = new List<RemisioDModel>();

                    while (dr.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();

                        LRemisioD.categoria = dr["categoria"].ToString();
                        LRemisioD.stotal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;
                        LRemisioD.total = dr["total"] != DBNull.Value ? Convert.ToInt64(dr["total"]) : 0;
                        LRemisioD.descuento = dr["descuento"] != DBNull.Value ? Convert.ToInt64(dr["descuento"]) : 0;

                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioD.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioD.fecha = DateTime.MinValue;
                        }

                        if (LRemisioD.total > LRemisioD.descuento)
                        {
                            var resultado_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resultado_2 = (resultado_1 / LRemisioD.descuento) + 1;
                            var resultado_3 = LRemisioD.stotal * resultado_2;
                            LRemisioD.VentaReal = resultado_3;
                        }
                        else if (LRemisioD.total < LRemisioD.descuento)
                        {
                            var resul_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resul_2 = resul_1 / LRemisioD.descuento;
                            var resul_ext = LRemisioD.stotal * resul_2;
                            var resul_3 = LRemisioD.stotal - resul_ext;
                            LRemisioD.VentaReal = resul_3;
                        }
                        else if (LRemisioD.total == LRemisioD.descuento)
                        {
                            LRemisioD.VentaReal = LRemisioD.stotal;
                        }
                        rawData.Add(LRemisioD);
                    }

                    Dictionary<DateTime, Dictionary<string, double>> dataProcessed = new Dictionary<DateTime, Dictionary<string, double>>();

                    foreach (var record in rawData)
                    {
                        DateTime fecha = (DateTime)record.fecha;
                        string categoria = record.categoria;
                        double ventaReal = record.VentaReal;

                        if (!dataProcessed.ContainsKey(fecha))
                        {
                            dataProcessed[fecha] = new Dictionary<string, double>();
                        }

                        if (!dataProcessed[fecha].ContainsKey(categoria))
                        {
                            dataProcessed[fecha][categoria] = 0;
                        }
                        dataProcessed[fecha][categoria] += ventaReal;

                        if (!dataProcessed[fecha].ContainsKey("Group_TotalImporte"))
                        {
                            dataProcessed[fecha]["Group_TotalImporte"] = 0;
                        }
                        dataProcessed[fecha]["Group_TotalImporte"] += ventaReal;
                    }

                    List<object> finalData = new List<object>();
                    var knownCategories = new List<string> {
                "ALCOHOL", "SOUVENIR", "ARTESANIAS","ARTESANIAS PREMIUM", "TEXTIL", "FARMACIA",
                "CERVEZA", "REFRESCOS", "AGUAS", "ABARROTES","TABAQUERIA","CALENDARIOS", "ENERGETICOS", "RTD", "SC","Group_TotalImporte"
            };

                    foreach (var date in dataProcessed.Keys)
                    {
                        double sinCategoria = 0;
                        foreach (var kvp in dataProcessed[date])
                        {
                            if (!knownCategories.Contains(kvp.Key))
                            {
                                sinCategoria += kvp.Value;
                            }
                        }

                        // Obtener el conteo para este día específico
                        int conteoDia = conteoPorDia.ContainsKey(date.Date) ? conteoPorDia[date.Date] : 0;

                        var record = new
                        {
                            Gruop_fecha = date,

                            Group_Souvenir = dataProcessed[date].ContainsKey("SOUVENIR") ? dataProcessed[date]["SOUVENIR"] : 0,
                            Group_Artesanias = dataProcessed[date].ContainsKey("ARTESANIAS") ? dataProcessed[date]["ARTESANIAS"] : 0,
                            Group_Artesanias_Premium = dataProcessed[date].ContainsKey("ARTESANIAS PREMIUM") ? dataProcessed[date]["ARTESANIAS PREMIUM"] : 0,
                            Group_Textil = dataProcessed[date].ContainsKey("TEXTIL") ? dataProcessed[date]["TEXTIL"] : 0,
                            Group_Farmacia = dataProcessed[date].ContainsKey("FARMACIA") ? dataProcessed[date]["FARMACIA"] : 0,
                            Group_Cerveza = dataProcessed[date].ContainsKey("CERVEZA") ? dataProcessed[date]["CERVEZA"] : 0,
                            Group_Refrescos = dataProcessed[date].ContainsKey("REFRESCOS") ? dataProcessed[date]["REFRESCOS"] : 0,
                            Group_Aguas = dataProcessed[date].ContainsKey("AGUAS") ? dataProcessed[date]["AGUAS"] : 0,
                            Group_Abarrotes = dataProcessed[date].ContainsKey("ABARROTES") ? dataProcessed[date]["ABARROTES"] : 0,
                            Group_Tabaqueria = dataProcessed[date].ContainsKey("TABAQUERIA") ? dataProcessed[date]["TABAQUERIA"] : 0,
                            Group_Calendarios = dataProcessed[date].ContainsKey("CALENDARIOS") ? dataProcessed[date]["CALENDARIOS"] : 0,
                            Group_Energeticos = dataProcessed[date].ContainsKey("ENERGETICOS") ? dataProcessed[date]["ENERGETICOS"] : 0,
                            Group_RTD = dataProcessed[date].ContainsKey("RTD") ? dataProcessed[date]["RTD"] : 0,
                            Group_SC = dataProcessed[date].ContainsKey("SC") ? dataProcessed[date]["SC"] : 0,
                            Group_TotalImporte = dataProcessed[date].ContainsKey("Group_TotalImporte") ? dataProcessed[date]["Group_TotalImporte"] : 0,
                            Group_SinCategoria = sinCategoria,
                            Group_TotalTickets = conteoDia
                        };

                        finalData.Add(record);
                    }

                    var jsonResult = Json(new
                    {
                        data = finalData,
                        totalRegistrosBD = conteoPorDia.Values.Sum()
                    }, JsonRequestBehavior.AllowGet);

                    jsonResult.MaxJsonLength = int.MaxValue;
                    return jsonResult;
                }
            }
        }










        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosNoVendidos
        [HttpPost]
        public ActionResult filtrarTablaNoVendidos(string fechaInicio, string fechaFin, string almacen, string[] categoria, string[] grupo)
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
                string baseQuery = @"SELECT * FROM Vista_ProductosSinRemision
                 WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;


                cn.Open();

                // Verificar si se seleccionaron categorías
                if (categoria != null && categoria.Length > 0)
                {
                    // Construir la cláusula OR para las categorías
                    StringBuilder categoriaClause = new StringBuilder();
                    for (int i = 0; i < categoria.Length; i++)
                    {
                        string paramName = "@categoria" + i;
                        categoriaClause.Append("categoria = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, categoria[i]);
                        if (i < categoria.Length - 1)
                        {
                            categoriaClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + categoriaClause.ToString() + ")";
                }

                // Verificar si se seleccionaron grupos
                if (grupo != null && grupo.Length > 0)
                {
                    // Construir la cláusula OR para los grupos
                    StringBuilder grupoClause = new StringBuilder();
                    for (int i = 0; i < grupo.Length; i++)
                    {
                        string paramName = "@grupo" + i;
                        grupoClause.Append("grupo = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, grupo[i]);
                        if (i < grupo.Length - 1)
                        {
                            grupoClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + grupoClause.ToString() + ")";
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<RemisioDModel> rawData = new List<RemisioDModel>();

                    while (dr.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();
                        LRemisioD.codigobarras = dr["codigobarras"].ToString();
                        LRemisioD.productoNombre = dr["productoNombre"].ToString();
                        LRemisioD.almacen = dr["almacen"].ToString();
                        LRemisioD.categoria = dr["categoria"].ToString();
                        LRemisioD.grupo = dr["grupo"].ToString();
                        LRemisioD.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRemisioD.stotal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;
                        LRemisioD.costo = dr["costo"] != DBNull.Value ? Convert.ToInt64(dr["costo"]) : 0;
                        LRemisioD.Existencia = dr["if"] != DBNull.Value ? Convert.ToInt64(dr["if"]) : 0;
                        LRemisioD.preciopub = dr["preciopub"] != DBNull.Value ? Convert.ToSingle(dr["preciopub"]) : 0;
                        LRemisioD.deportiva = dr["deportiva"].ToString();
                        LRemisioD.impuesto = dr["impuesto"] != DBNull.Value ? Convert.ToInt64(dr["impuesto"]) : 0;

                        //tabla remisioM
                        LRemisioD.total = dr["total"] != DBNull.Value ? Convert.ToInt64(dr["total"]) : 0;
                        LRemisioD.descuento = dr["descuento"] != DBNull.Value ? Convert.ToInt64(dr["descuento"]) : 0;

                        // Conversión de la fecha
                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioD.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioD.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        //validacion para la operacion del resultado Venta Real
                        if (LRemisioD.total > LRemisioD.descuento)
                        {
                            var resultado_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resultado_2 = (resultado_1 / LRemisioD.descuento) + 1;  // Asegúrate de que descuento no sea 0
                            var resultado_3 = LRemisioD.stotal * resultado_2;

                            LRemisioD.VentaReal = resultado_3;
                        }
                        else if (LRemisioD.total < LRemisioD.descuento)
                        {
                            var resul_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resul_2 = resul_1 / LRemisioD.descuento;  // Asegúrate de que descuento no sea 0
                            var resul_ext = LRemisioD.stotal * resul_2;
                            var resul_3 = LRemisioD.stotal - resul_ext;

                            LRemisioD.VentaReal = resul_3;
                        }
                        else if (LRemisioD.total == LRemisioD.descuento)
                        {
                            LRemisioD.VentaReal = LRemisioD.stotal;
                        }

                        // Identificar si deportiva es 'F'
                        if (LRemisioD.deportiva == "F")
                        {
                            LRemisioD.total_fijo = LRemisioD.VentaReal;
                        }
                        else
                        {
                            LRemisioD.total_depor = LRemisioD.VentaReal;
                        }

                        // Cálculo de vcosto y utilidad
                        LRemisioD.vcosto = LRemisioD.cantidads * LRemisioD.costo;
                        LRemisioD.utilidad = LRemisioD.VentaReal - LRemisioD.vcosto;

                        //Calculo de IVA    
                        if (LRemisioD.impuesto >= 1)
                        {
                            // Calculo de IVAa
                            LRemisioD.VentaSinIVA = LRemisioD.VentaReal / 1.16;
                            LRemisioD.VentaIVA = LRemisioD.VentaReal - LRemisioD.VentaSinIVA;
                            LRemisioD.VentaConIVA = LRemisioD.VentaSinIVA + LRemisioD.VentaIVA;
                        }
                        else if (LRemisioD.impuesto == 0)
                        {

                            // Y también es igual a la venta con IVA
                            LRemisioD.VentaConIVA = LRemisioD.VentaReal;
                        }

                        //Calculo para el valor de inventario tomando la existencia
                        LRemisioD.VInventario = LRemisioD.costo * LRemisioD.Existencia;



                        rawData.Add(LRemisioD);
                    }

                    var groupedData = rawData.GroupBy(d => new
                    {
                        d.codigobarras,
                        d.productoNombre,
                        d.almacen,
                        d.categoria,
                        d.grupo,
                        d.costo,
                        d.preciopub,
                        d.Existencia
                    })
                    .Select(g => new GruopRemisioDModel
                    {
                        Group_codigobarras = g.Key.codigobarras,
                        Group_productoNombre = g.Key.productoNombre,
                        Group_almacen = g.Key.almacen,
                        Group_categoria = g.Key.categoria,
                        Group_grupo = g.Key.grupo,
                        Group_costo = g.Key.costo,
                        Group_Existencia = g.Key.Existencia,
                        Group_preciopub = g.Key.preciopub,

                        //Todos estos valores se suman
                        Group_cantidads = g.Sum(x => x.cantidads),
                        Group_VentaReal = g.Sum(x => x.VentaReal),
                        Group_VentaSinIVA = g.Sum(x => x.VentaSinIVA),    // Sumatoria de VentaSinIVA
                        Group_VentaIVA = g.Sum(x => x.VentaIVA),          // Sumatoria de VentaIVA
                        Group_VentaConIVA = g.Sum(x => x.VentaConIVA),    // Sumatoria de VentaConIVA
                        Group_total_fijo = g.Where(x => x.deportiva == "F").Sum(x => x.total_fijo),
                        Group_total_depor = g.Where(x => x.deportiva != "F").Sum(x => x.total_depor),
                        Group_vcosto = g.Sum(x => x.vcosto),
                        Group_VInventario = g.Sum(x => x.VInventario),
                        Group_utilidad = g.Sum(x => x.utilidad),
                        Gruop_fecha = (DateTime)g.Max(x => x.fecha),

                    })
                    .ToList();

                    var jsonResult = Json(groupedData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }





        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        [HttpPost]
        public ActionResult filtrarTablaNoVendidosJoy(string fechaInicio, string fechaFin, string almacen, string[] categoria)
        {
            DateTime fechaInicioParsed;
            DateTime fechaFinParsed;

            if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                !DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                string baseQuery = @"SELECT * FROM Vista_ProductosSinRemisionJoy
                 WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;


                cn.Open();

                // Verificar si se seleccionaron categorías
                if (categoria != null && categoria.Length > 0)
                {
                    // Construir la cláusula OR para las categorías
                    StringBuilder categoriaClause = new StringBuilder();
                    for (int i = 0; i < categoria.Length; i++)
                    {
                        string paramName = "@categoria" + i;
                        categoriaClause.Append("categoria = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, categoria[i]);
                        if (i < categoria.Length - 1)
                        {
                            categoriaClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + categoriaClause.ToString() + ")";
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<RemisioDJoyModel> rawData = new List<RemisioDJoyModel>();

                    while (dr.Read())
                    {
                        RemisioDJoyModel LRemisioDJoy = new RemisioDJoyModel();
                        LRemisioDJoy.codigo_barras = dr["codigo_barras"].ToString();
                        LRemisioDJoy.Nombre_largo = dr["descripcion_larga"].ToString();
                        LRemisioDJoy.almacen = dr["almacen"].ToString();
                        LRemisioDJoy.categoria = dr["categoria"].ToString();
                        LRemisioDJoy.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRemisioDJoy.peso = dr["peso"] != DBNull.Value ? Math.Round(Convert.ToDouble(dr["peso"]), 2) : 0;
                        LRemisioDJoy.VentaReal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;
                        LRemisioDJoy.deportiva = dr["deportiva"].ToString();
                        // Conversión de la fecha
                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioDJoy.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioDJoy.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }
                        LRemisioDJoy.Existencia = dr["if"] != DBNull.Value ? Convert.ToInt64(dr["if"]) : 0;


                        //saber cual es deportiva
                        // revisar
                        LRemisioDJoy.total_depor = LRemisioDJoy.VentaReal;

                        // Calculo de IVAa
                        LRemisioDJoy.VentaSinIVA = LRemisioDJoy.VentaReal / 1.16;
                        LRemisioDJoy.VentaIVA = LRemisioDJoy.VentaReal - LRemisioDJoy.VentaSinIVA;
                        LRemisioDJoy.VentaConIVA = LRemisioDJoy.VentaSinIVA + LRemisioDJoy.VentaIVA;


                        rawData.Add(LRemisioDJoy);
                    }

                    var groupedData = rawData.GroupBy(d => new
                    {
                        d.codigo_barras,
                        d.Nombre_largo,
                        d.almacen,
                        d.categoria,
                        d.Existencia
                    })
                    .Select(g => new GruopRemisioDModel
                    {
                        Group_codigobarras = g.Key.codigo_barras,
                        Group_productoNombre = g.Key.Nombre_largo,
                        Group_almacen = g.Key.almacen,
                        Group_categoria = g.Key.categoria,
                        Group_Existencia = g.Key.Existencia,

                        //Todos estos valores se suman
                        Group_cantidads = g.Sum(x => x.cantidads),
                        Group_peso = Math.Round(g.Sum(x => x.peso), 2),
                        Group_VentaReal = g.Sum(x => x.VentaReal),
                        Group_VentaSinIVA = g.Sum(x => x.VentaSinIVA),    // Sumatoria de VentaSinIVA
                        Group_VentaIVA = g.Sum(x => x.VentaIVA),          // Sumatoria de VentaIVA
                        Group_VentaConIVA = g.Sum(x => x.VentaConIVA),    // Sumatoria de VentaConIVA
                        Group_total_depor = g.Where(x => x.deportiva != "F").Sum(x => x.total_depor),
                        Gruop_fecha = g.Max(x => x.fecha),

                    })
                    .ToList();

                    var jsonResult = Json(groupedData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        // fn hugo
        [HttpPost]
        public ActionResult GetSalesByProduct(string fechaInicio, string fechaFin, string agencia, string hotel, string guia, string almacen)
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

                string baseQuery = @"SELECT * FROM taquilla.dbo.vGetTicketsByStore
                WHERE fecha >= @fechaInicio AND fecha <= @fechaFin";

                if (agencia != "")
                {
                    baseQuery = baseQuery + @" AND agencia=@agencia";
                }
                if (almacen != "")
                {
                    baseQuery = baseQuery + @" AND almacen=@almacen";
                }
                if (hotel != "")
                {
                    baseQuery = baseQuery + @" AND hotel=@hotel";
                }
                if (guia != "")
                {
                    baseQuery = baseQuery + @" AND nom_guia=@guia";
                }

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);

                cmd.Parameters.AddWithValue("@agencia", agencia ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@almacen", almacen ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@hotel", hotel ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@guia", guia ?? (object)DBNull.Value);

                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<ViewStoresModel> rawData = new List<ViewStoresModel>();

                    while (dr.Read())
                    {
                        ViewStoresModel response = new ViewStoresModel();
                        //response.agency_name = dr["agency_name"].ToString();
                        //response.guide = dr["guide"].ToString();
                        //response.pax = dr["pax"].ToString();
                        //response.description = dr["description"].ToString();
                        //response.product_code = dr["product_code"].ToString();
                        //response.ticket = dr["ticket"].ToString();

                        response.agency_name = dr["agencia"].ToString();
                        response.hotel = dr["hotel"].ToString();
                        response.guide = dr["nom_guia"].ToString();
                        response.pax = dr["pax"].ToString();
                        response.description = dr["descripcion_larga"].ToString();
                        response.product_code = dr["producto"].ToString();
                        response.reference = dr["referencia"].ToString();
                        response.total = Convert.ToInt64(dr["stotal"]);
                        response.sucursal = dr["almacen"] != DBNull.Value ? Convert.ToInt32(dr["almacen"]) : 0;
                        response.folio_remision = dr["folio_remision"] != DBNull.Value ? Convert.ToSingle(dr["folio_remision"]) : 0;

                        response.totalx = dr["totalx"] != DBNull.Value ? Convert.ToSingle(dr["totalx"]) : 0;

                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime x_date))
                        {
                            response.x_date = x_date.ToUniversalTime();
                        }
                        else
                        {
                            response.x_date = DateTime.MinValue;
                        }

                        rawData.Add(response);
                    }

                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }



        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        // fn hugo
        [HttpPost]
        public ActionResult GetTicketsByStore(string fechaInicio, string fechaFin, string agencia, string hotel, string guia, string almacen)
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

                string baseQuery = @"SELECT * FROM taquilla.dbo.vGetTicketsByStore
                WHERE fecha >= @fechaInicio AND fecha <= @fechaFin";

                if (agencia != "")
                {
                    baseQuery = baseQuery + @" AND agencia=@agencia";
                }
                if (almacen != "")
                {
                    baseQuery = baseQuery + @" AND almacen=@almacen";
                }
                if (hotel != "")
                {
                    baseQuery = baseQuery + @" AND hotel=@hotel";
                }
                if (guia != "")
                {
                    baseQuery = baseQuery + @" AND nom_guia=@guia";
                }


                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);


                cmd.Parameters.AddWithValue("@agencia", agencia ?? (object)DBNull.Value);




                cmd.Parameters.AddWithValue("@almacen", almacen ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@hotel", hotel ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@guia", guia ?? (object)DBNull.Value);


                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<ViewStoresModel> rawData = new List<ViewStoresModel>();

                    while (dr.Read())
                    {
                        ViewStoresModel response = new ViewStoresModel();
                        //response.agency_name = dr["agency_name"].ToString();
                        //response.guide = dr["guide"].ToString();
                        //response.pax = dr["pax"].ToString();
                        //response.description = dr["description"].ToString();
                        //response.product_code = dr["product_code"].ToString();
                        //response.ticket = dr["ticket"].ToString();

                        response.agency_name = dr["agencia"].ToString();
                        response.hotel = dr["hotel"].ToString();
                        response.guide = dr["nom_guia"].ToString();
                        response.pax = dr["pax"].ToString();
                        response.description = dr["descripcion_larga"].ToString();
                        response.product_code = dr["producto"].ToString();
                        response.reference = dr["referencia"].ToString();
                        response.total = Convert.ToInt64(dr["stotal"]);
                        response.sucursal = dr["almacen"] != DBNull.Value ? Convert.ToInt32(dr["almacen"]) : 0;
                        response.folio_remision = dr["folio_remision"] != DBNull.Value ? Convert.ToSingle(dr["folio_remision"]) : 0;

                        response.totalx = dr["totalx"] != DBNull.Value ? Convert.ToSingle(dr["totalx"]) : 0;

                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime x_date))
                        {
                            response.x_date = x_date.ToUniversalTime();
                        }
                        else
                        {
                            response.x_date = DateTime.MinValue;
                        }

                        rawData.Add(response);
                    }

                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }


        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        // fn hugo
        [HttpPost]
        public ActionResult GetGuidesByStore(string almacen, string fecha1, string fecha2)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!DateTime.TryParseExact(fecha1, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                    !DateTime.TryParseExact(fecha2, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
                {
                    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
                }

                SqlCommand cmd = new SqlCommand("taquilla.dbo.sp_GetGuidesByStore", cn);
                cmd.Parameters.AddWithValue("@Almacen", almacen);
                cmd.Parameters.AddWithValue("@Fecha1", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@Fecha2", fechaFinParsed);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<ViewStoresModel> rawData = new List<ViewStoresModel>();

                    while (dr.Read())
                    {
                        ViewStoresModel response = new ViewStoresModel();
                        response.nom_guia = dr["nombre_guia"] != DBNull.Value ? dr["nombre_guia"].ToString() : "";
                        rawData.Add(response);
                    }

                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        // fn hugo
        [HttpPost]
        public ActionResult GetHotelsByStore(string almacen, string fecha1, string fecha2)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!DateTime.TryParseExact(fecha1, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                    !DateTime.TryParseExact(fecha2, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
                {
                    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
                }

                SqlCommand cmd = new SqlCommand("taquilla.dbo.sp_GetHotelsByStore", cn);
                cmd.Parameters.AddWithValue("@Almacen", almacen);
                cmd.Parameters.AddWithValue("@Fecha1", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@Fecha2", fechaFinParsed);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<string> rawData = new List<string>();

                    while (dr.Read())
                    {
                        //ViewStoresModel response = new ViewStoresModel();
                        //response.hotel = dr["hotel"] != DBNull.Value ? dr["hotel"].ToString() : "";
                        rawData.Add(dr["hotel"] != DBNull.Value ? dr["hotel"].ToString() : "");
                    }

                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        // fn hugo
        [HttpPost]
        public ActionResult GetAgenciesByStore(string almacen, string fecha1, string fecha2)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!DateTime.TryParseExact(fecha1, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                    !DateTime.TryParseExact(fecha2, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
                {
                    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
                }

                SqlCommand cmd = new SqlCommand("taquilla.dbo.sp_GetAgenciesByStore", cn);
                cmd.Parameters.AddWithValue("@Almacen", almacen);
                cmd.Parameters.AddWithValue("@Fecha1", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@Fecha2", fechaFinParsed);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<string> rawData = new List<string>();

                    while (dr.Read())
                    {
                        //ViewStoresModel response = new ViewStoresModel();
                        //response.hotel = dr["hotel"] != DBNull.Value ? dr["hotel"].ToString() : "";
                        rawData.Add(dr["agencia"] != DBNull.Value ? dr["agencia"].ToString() : "");
                    }

                    rawData.ToList();

                    var jsonResult = Json(rawData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }





        //FUNCION DE LA VISTA DE VProductosAdminTotal ( Reporte de vendidos y no vendidos de productos)
        // Método auxiliar para verificar si existe una columna en el SqlDataReader
        [HttpPost]
        public ActionResult getReportSalesByProduct(
     string fechaInicio,
     string fechaFin,
     string almacen,
     string[] categoria,
     string[] grupo,
     int page = 1,
     int pageSize = 5000)
        {
            try
            {
                // Validaciones (mantener igual)
                if (string.IsNullOrWhiteSpace(fechaInicio) ||
                    string.IsNullOrWhiteSpace(fechaFin) ||
                    string.IsNullOrWhiteSpace(almacen))
                {
                    return Json(new { success = false, message = "Fechas y almacén son requeridos" });
                }

                // Parseo de fechas (mantener igual)
                DateTime fechaInicioParsed, fechaFinParsed;
                string[] formats = { "dd/MM/yyyy", "MM/dd/yyyy", "yyyy-MM-dd" };

                if (!DateTime.TryParseExact(fechaInicio, formats, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out fechaInicioParsed) ||
                    !DateTime.TryParseExact(fechaFin, formats, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out fechaFinParsed))
                {
                    return Json(new { success = false, message = "Formato de fecha incorrecto. Use dd/MM/yyyy" });
                }

                fechaInicioParsed = fechaInicioParsed.Date;
                fechaFinParsed = fechaFinParsed.Date.AddDays(1).AddSeconds(-1);

                // Validar rango de fechas (máximo 3 meses)
                if ((fechaFinParsed - fechaInicioParsed).TotalDays > 90)
                {
                    return Json(new { success = false, message = "El rango de fechas no puede ser mayor a 3 meses" });
                }

                // Almacén (mantener igual)
                if (!long.TryParse(almacen, out long almacenId))
                {
                    return Json(new { success = false, message = "El almacén debe ser un número válido." });
                }

                // Normalizar arrays (mantener igual)
                if (categoria == null) categoria = Array.Empty<string>();
                if (grupo == null) grupo = Array.Empty<string>();

                using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                {
                    cn.Open();

                    var result = new List<object>();
                    int offset = (page - 1) * pageSize;

                    // 1. PRIMERO OBTENER EL TOTAL GENERAL (antes de la paginación)
                    double totalGeneral = 0;
                    var queryTotal = new StringBuilder(@"
SELECT COALESCE(SUM(rd.stotal), 0) AS totalGeneral
FROM VReportSalesByProduct rd WITH (NOLOCK)
WHERE rd.almacen = @almacen
  AND rd.fecha BETWEEN @fechaInicio AND @fechaFin");

                    // Agregar condiciones de categoría/grupo si hay valores específicos
                    if (categoria != null && categoria.Length > 0 && !categoria.Any(c => c?.Trim().ToLower() == "all"))
                    {
                        AddArrayParameters(queryTotal, "categoria", "rd.categoria", categoria);
                    }

                    if (grupo != null && grupo.Length > 0 && !grupo.Any(g => g?.Trim().ToLower() == "all"))
                    {
                        AddArrayParameters(queryTotal, "grupo", "rd.grupo", grupo);
                    }

                    using (SqlCommand cmdTotal = new SqlCommand(queryTotal.ToString(), cn))
                    {
                        cmdTotal.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                        cmdTotal.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                        cmdTotal.Parameters.AddWithValue("@almacen", almacenId);

                        AddArrayParametersToCommand(cmdTotal, "categoria", categoria);
                        AddArrayParametersToCommand(cmdTotal, "grupo", grupo);

                        var totalObj = cmdTotal.ExecuteScalar();
                        totalGeneral = totalObj != DBNull.Value ? Convert.ToDouble(totalObj) : 0;
                    }

                    // 2. Obtener datos paginados
                    var batchData = new List<RemisioDModel>();

                    // 2.1. Productos vendidos (con paginación)
                    var queryVendidos = new StringBuilder(@"
SELECT 
    rd.codigobarras,
    rd.descripcion_larga AS productoNombre,
    rd.almacen,
    rd.categoria,
    rd.grupo,
    rd.deportiva,
    rd.cantidads,
    rd.stotal,
    rd.costo,
    rd.impuesto,
    rd.fecha,
    rd.existencia,
    rd.pventareal,
    rd.pventareal AS preciopub,
    rd.costo AS ucosto
FROM VReportSalesByProduct rd WITH (NOLOCK)
WHERE rd.almacen = @almacen
  AND rd.fecha BETWEEN @fechaInicio AND @fechaFin");

                    // Solo agregar condiciones de categoría/grupo si hay valores específicos
                    if (categoria != null && categoria.Length > 0 && !categoria.Any(c => c?.Trim().ToLower() == "all"))
                    {
                        AddArrayParameters(queryVendidos, "categoria", "rd.categoria", categoria);
                    }

                    if (grupo != null && grupo.Length > 0 && !grupo.Any(g => g?.Trim().ToLower() == "all"))
                    {
                        AddArrayParameters(queryVendidos, "grupo", "rd.grupo", grupo);
                    }

                    queryVendidos.Append(@"
ORDER BY rd.codigobarras
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY");

                    using (SqlCommand cmdVendidos = new SqlCommand(queryVendidos.ToString(), cn))
                    {
                        cmdVendidos.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                        cmdVendidos.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                        cmdVendidos.Parameters.AddWithValue("@almacen", almacenId);
                        cmdVendidos.Parameters.AddWithValue("@offset", offset);
                        cmdVendidos.Parameters.AddWithValue("@pageSize", pageSize);

                        AddArrayParametersToCommand(cmdVendidos, "categoria", categoria);
                        AddArrayParametersToCommand(cmdVendidos, "grupo", grupo);

                        using (SqlDataReader dr = cmdVendidos.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                RemisioDModel item = MapRemisioDFromReader(dr);
                                batchData.Add(item);
                            }
                        }
                    }

                    // 2.2. Productos NO vendidos (solo si es el primer lote)
                    if (page == 1)
                    {
                        var queryNoVendidos = new StringBuilder(@"
SELECT 
    ap.codigobarra AS codigobarras,
    MAX(p.Nombre) AS productoNombre,
    ap.almacen,
    MAX(cat.Nombre) AS categoria,
    MAX(p.grupo) AS grupo,
    MAX(p.ultcost) AS costo,
    MAX(p.preciopub) AS preciopub,
    MAX(ap.[if]) AS existencia,
    @fechaInicio AS fecha,
    'F' AS deportiva,
    0 AS cantidads,
    0 AS stotal,
    0 AS impuesto,
    MAX(p.preciopub) AS pventareal,
    MAX(p.ultcost) AS ucosto
FROM alma_prod ap WITH (NOLOCK)
INNER JOIN productos p WITH (NOLOCK) ON ap.producto = p.producto
LEFT JOIN categorias cat WITH (NOLOCK) ON p.categoria = cat.categoria
WHERE ap.almacen = @almacen
  AND NOT EXISTS (
    SELECT 1
    FROM VReportSalesByProduct v WITH (NOLOCK)
    WHERE v.codigobarras = ap.codigobarra
      AND v.almacen = ap.almacen
      AND v.fecha BETWEEN @fechaInicio AND @fechaFin
)");

                        // Manejo de categorías
                        if (categoria != null && categoria.Length > 0 && !categoria.Any(c => c?.Trim().ToLower() == "all"))
                        {
                            queryNoVendidos.Append(" AND (");
                            bool isNumeric = categoria.All(c => int.TryParse(c, out _));
                            if (isNumeric)
                            {
                                if (categoria.Length == 1)
                                    queryNoVendidos.Append("p.categoria = CAST(@categoria0 AS int)");
                                else
                                    for (int i = 0; i < categoria.Length; i++)
                                    {
                                        if (i > 0) queryNoVendidos.Append(" OR ");
                                        queryNoVendidos.Append($"p.categoria = CAST(@categoria{i} AS int)");
                                    }
                            }
                            else
                            {
                                if (categoria.Length == 1)
                                    queryNoVendidos.Append("cat.Nombre = @categoria0");
                                else
                                    for (int i = 0; i < categoria.Length; i++)
                                    {
                                        if (i > 0) queryNoVendidos.Append(" OR ");
                                        queryNoVendidos.Append($"cat.Nombre = @categoria{i}");
                                    }
                            }
                            queryNoVendidos.Append(")");
                        }

                        // Manejo de grupos
                        if (grupo != null && grupo.Length > 0 && !grupo.Any(g => g?.Trim().ToLower() == "all"))
                        {
                            queryNoVendidos.Append(" AND (");
                            if (grupo.Length == 1)
                            {
                                queryNoVendidos.Append("p.grupo = @grupo0");
                            }
                            else
                            {
                                for (int i = 0; i < grupo.Length; i++)
                                {
                                    if (i > 0) queryNoVendidos.Append(" OR ");
                                    queryNoVendidos.Append($"p.grupo = @grupo{i}");
                                }
                            }
                            queryNoVendidos.Append(")");
                        }

                        queryNoVendidos.Append(@"
GROUP BY ap.codigobarra, ap.almacen");

                        using (SqlCommand cmdNoVendidos = new SqlCommand(queryNoVendidos.ToString(), cn))
                        {
                            cmdNoVendidos.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                            cmdNoVendidos.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                            cmdNoVendidos.Parameters.AddWithValue("@almacen", almacenId);

                            // Agregar parámetros de categoría
                            if (categoria != null && categoria.Length > 0 && !categoria.Any(c => c?.Trim().ToLower() == "all"))
                            {
                                bool isNumeric = categoria.All(c => int.TryParse(c, out _));
                                for (int i = 0; i < categoria.Length; i++)
                                {
                                    if (isNumeric)
                                        cmdNoVendidos.Parameters.AddWithValue($"@categoria{i}", int.Parse(categoria[i]));
                                    else
                                        cmdNoVendidos.Parameters.AddWithValue($"@categoria{i}", categoria[i]);
                                }
                            }
                            // Agregar parámetros de grupo
                            if (grupo != null && grupo.Length > 0 && !grupo.Any(g => g?.Trim().ToLower() == "all"))
                            {
                                for (int i = 0; i < grupo.Length; i++)
                                {
                                    cmdNoVendidos.Parameters.AddWithValue($"@grupo{i}", grupo[i]);
                                }
                            }

                            using (SqlDataReader dr = cmdNoVendidos.ExecuteReader())
                            {
                                while (dr.Read())
                                {
                                    RemisioDModel item = MapRemisioDFromReader(dr);
                                    batchData.Add(item);
                                }
                            }
                        }
                    }

                    // 3. Procesar lote actual
                    var groupedBatch = ProcessBatch(batchData);
                    result.AddRange(groupedBatch);

                    // 4. Retornar resultados paginados + el total
                    return new JsonResult()
                    {
                        Data = new
                        {
                            success = true,
                            data = result,
                            currentPage = page,
                            pageSize = pageSize,
                            hasMore = batchData.Count >= pageSize,
                            totalGeneral = totalGeneral // Ahora es un valor fijo calculado aparte
                        },
                        MaxJsonLength = int.MaxValue,
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error: {ex.Message}",
                    stackTrace = ex.StackTrace,
                    innerException = ex.InnerException?.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        // Método para mapear desde SqlDataReader
        private RemisioDModel MapRemisioDFromReader(SqlDataReader dr)
        {
            RemisioDModel item = new RemisioDModel();

            item.codigobarras = GetValueOrDefault<string>(dr, "codigobarras", string.Empty);
            item.productoNombre = GetValueOrDefault<string>(dr, "productoNombre", string.Empty);
            item.almacen = GetValueOrDefault<string>(dr, "almacen", string.Empty);
            item.categoria = GetValueOrDefault<string>(dr, "categoria", string.Empty);
            item.grupo = GetValueOrDefault<string>(dr, "grupo", string.Empty);
            item.deportiva = GetValueOrDefault<string>(dr, "deportiva", "F");
            item.cantidads = GetValueOrDefault<long>(dr, "cantidads", 0);
            item.stotal = GetValueOrDefault<long>(dr, "stotal", 0);
            item.costo = GetValueOrDefault<double>(dr, "costo", 0);
            item.ultcost = item.costo;
            item.impuesto = GetValueOrDefault<long>(dr, "impuesto", 0);
            item.fecha = GetValueOrDefault<DateTime>(dr, "fecha", DateTime.MinValue);
            item.preciopub = GetValueOrDefault<float>(dr, "preciopub", 0);

            item.Existencia = dr["existencia"] != DBNull.Value ? Convert.ToInt64(dr["existencia"]) : 0;

            if (item.cantidads == 0)
            {
                item.VentaReal = 0;
                item.VentaSinIVA = 0;
                item.VentaIVA = 0;
                item.VentaConIVA = 0;
                item.total_fijo = 0;
                item.total_depor = 0;
                item.vcosto = item.costo;
                item.utilidad = 0;
            }
            else
            {
                // 👇 CORRIGE ESTA LÍNEA 👇
                item.VentaReal = GetValueOrDefault<double>(dr, "pventareal", 0);

                item.vcosto = item.cantidads * item.costo;
                item.utilidad = item.VentaReal - item.vcosto;

                if (item.deportiva == "F")
                    item.total_fijo = item.VentaReal;
                else
                    item.total_depor = item.VentaReal;

                if (item.impuesto >= 1)
                {
                    item.VentaSinIVA = item.VentaReal / 1.16;
                    item.VentaIVA = item.VentaReal - item.VentaSinIVA;
                    item.VentaConIVA = item.VentaReal;
                }
                else
                {
                    item.VentaSinIVA = item.VentaReal;
                    item.VentaConIVA = item.VentaReal;
                }
            }

            item.VInventario = item.costo * item.Existencia;

            return item;
        }

        // Método para procesar por lotes
        private List<object> ProcessBatch(List<RemisioDModel> batchData)
        {
            return batchData
                .GroupBy(d => new
                {
                    d.codigobarras,
                    d.productoNombre,
                    d.almacen,
                    d.categoria,
                    d.grupo,
                })
                .Select(g => new
                {
                    Group_codigobarras = g.Key.codigobarras,
                    Group_productoNombre = g.Key.productoNombre,
                    Group_almacen = g.Key.almacen,
                    Group_categoria = g.Key.categoria,
                    Group_grupo = g.Key.grupo,
                    Group_costo = g.Average(x => x.costo),
                    Group_Existencia = g.First().Existencia,
                    // 👇 CAMBIA ESTO: el precio publicado puedes dejarlo promedio o mejor el primero también:
                    Group_preciopub = g.First().preciopub,
                    // 👇 AGREGA ESTO: el precio de venta real UNITARIO (el que NO debe variar)
                    Group_pventareal = g.First().pventareal,
                    Depor_string = g.First().deportiva,
                    ultcost = g.Average(x => x.costo),
                    Group_cantidads = g.Sum(x => x.cantidads),
                    Group_VentaReal = g.Sum(x => x.VentaReal),
                    Group_VentaSinIVA = g.Sum(x => x.VentaSinIVA),
                    Group_VentaIVA = g.Sum(x => x.VentaIVA),
                    Group_VentaConIVA = g.Sum(x => x.VentaConIVA),
                    Group_total_fijo = g.Where(x => x.deportiva == "F").Sum(x => x.total_fijo),
                    Group_total_depor = g.Where(x => x.deportiva != "F").Sum(x => x.total_depor),
                    Group_vcosto = g.Sum(x => x.vcosto),
                    Group_VInventario = g.Sum(x => x.VInventario),
                    Group_utilidad = g.Sum(x => x.utilidad),
                    Gruop_fecha = g.Max(x => x.fecha)
                })
                .ToList<object>();
        }



        // Métodos auxiliares (sin cambios)
        private bool ColumnExists(SqlDataReader reader, string columnName)
        {
            var schemaTable = reader.GetSchemaTable();
            if (schemaTable == null)
                return false;
            foreach (DataRow row in schemaTable.Rows)
            {
                if (row["ColumnName"].ToString().Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private T GetValueOrDefault<T>(SqlDataReader dr, string columnName, T defaultValue)
        {
            if (!ColumnExists(dr, columnName))
                return defaultValue;

            int ordinal = dr.GetOrdinal(columnName);
            if (dr.IsDBNull(ordinal))
                return defaultValue;

            object value = dr.GetValue(ordinal);
            if (value is T)
            {
                return (T)value;
            }
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        private void AddArrayParameters(StringBuilder query, string paramName, string fieldName, string[] values)
        {
            if (values != null && values.Length > 0)
            {
                // Caso cuando se selecciona "all" o el array está vacío (no agregar filtro)
                if (values.Any(v => v?.Trim().ToLower() == "all"))
                {
                    return;
                }

                query.Append(" AND (");

                // Caso especial para un solo valor
                if (values.Length == 1)
                {
                    query.Append($"{fieldName} = @{paramName}0");
                }
                else
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (i > 0) query.Append(" OR ");
                        query.Append($"{fieldName} = @{paramName}{i}");
                    }
                }

                query.Append(")");
            }
        }

        private void AddArrayParametersToCommand(SqlCommand cmd, string paramName, string[] values)
        {
            if (values != null && values.Length > 0 && !values.Any(v => v?.Trim().ToLower() == "all"))
            {
                for (int i = 0; i < values.Length; i++)
                {
                    cmd.Parameters.AddWithValue($"@{paramName}{i}", values[i]);
                }
            }
        }

        private double CalculateVentaReal(double stotal, double total, double descuento)
        {
            return stotal - descuento;
        }






        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        // fn hugo
        [HttpPost]
        public ActionResult filtrarTablaR(string fechaInicio, string fechaFin, string almacen, string[] categoria, string[] grupo)
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
                string baseQuery = @"SELECT * FROM remisioD
                 WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;


                cn.Open();

                // Verificar si se seleccionaron categorías
                if (categoria != null && categoria.Length > 0)
                {
                    // Construir la cláusula OR para las categorías
                    StringBuilder categoriaClause = new StringBuilder();
                    for (int i = 0; i < categoria.Length; i++)
                    {
                        string paramName = "@categoria" + i;
                        categoriaClause.Append("categoria = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, categoria[i]);
                        if (i < categoria.Length - 1)
                        {
                            categoriaClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + categoriaClause.ToString() + ")";
                }

                // Verificar si se seleccionaron grupos
                if (grupo != null && grupo.Length > 0)
                {
                    // Construir la cláusula OR para los grupos
                    StringBuilder grupoClause = new StringBuilder();
                    for (int i = 0; i < grupo.Length; i++)
                    {
                        string paramName = "@grupo" + i;
                        grupoClause.Append("grupo = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, grupo[i]);
                        if (i < grupo.Length - 1)
                        {
                            grupoClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + grupoClause.ToString() + ")";
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<RemisioDModel> rawData = new List<RemisioDModel>();

                    while (dr.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();
                        LRemisioD.codigobarras = dr["codigobarras"].ToString();
                        LRemisioD.productoNombre = dr["productoNombre"].ToString();
                        LRemisioD.almacen = dr["almacen"].ToString();
                        LRemisioD.categoria = dr["categoria"].ToString();
                        LRemisioD.grupo = dr["grupo"].ToString();
                        LRemisioD.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRemisioD.stotal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;
                        LRemisioD.costo = dr["costo"] != DBNull.Value ? Convert.ToInt64(dr["costo"]) : 0;
                        LRemisioD.Existencia = dr["existencia"] != DBNull.Value ? Convert.ToInt64(dr["existencia"]) : 0;
                        LRemisioD.preciopub = dr["preciopub"] != DBNull.Value ? Convert.ToSingle(dr["preciopub"]) : 0;
                        //LRemisioD.ultcost = String.Format("{0:00}", dr["ultcost"]);
                        LRemisioD.ultcost = dr["ultcost"] != DBNull.Value ? Convert.ToInt64(dr["ultcost"]) : 0;
                        //LRemisioD.ultcost = "0";

                        //revisar
                        LRemisioD.deportiva = dr["deportiva"].ToString();

                        LRemisioD.impuesto = dr["impuesto"] != DBNull.Value ? Convert.ToInt64(dr["impuesto"]) : 0;

                        //tabla remisioM
                        LRemisioD.total = dr["total"] != DBNull.Value ? Convert.ToInt64(dr["total"]) : 0;
                        LRemisioD.descuento = dr["descuento"] != DBNull.Value ? Convert.ToInt64(dr["descuento"]) : 0;

                        // Conversión de la fecha
                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioD.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioD.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        //validacion para la operacion del resultado Venta Real
                        if (LRemisioD.total > LRemisioD.descuento)
                        {
                            var resultado_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resultado_2 = (resultado_1 / LRemisioD.descuento) + 1;  // Asegúrate de que descuento no sea 0
                            var resultado_3 = LRemisioD.stotal * resultado_2;

                            LRemisioD.VentaReal = resultado_3;
                        }
                        else if (LRemisioD.total < LRemisioD.descuento)
                        {
                            var resul_1 = Math.Abs(LRemisioD.total - LRemisioD.descuento);
                            var resul_2 = resul_1 / LRemisioD.descuento;  // Asegúrate de que descuento no sea 0
                            var resul_ext = LRemisioD.stotal * resul_2;
                            var resul_3 = LRemisioD.stotal - resul_ext;

                            LRemisioD.VentaReal = resul_3;
                        }
                        else if (LRemisioD.total == LRemisioD.descuento)
                        {
                            LRemisioD.VentaReal = LRemisioD.stotal;
                        }

                        // Identificar si deportiva es 'F'
                        //revisar
                        if (LRemisioD.deportiva == "F")
                        {
                            LRemisioD.total_fijo = LRemisioD.VentaReal;
                        }
                        else
                        {
                            // revisar
                            LRemisioD.total_depor = LRemisioD.VentaReal;
                        }

                        // Cálculo de vcosto y utilidad
                        LRemisioD.vcosto = LRemisioD.cantidads * LRemisioD.costo;
                        LRemisioD.utilidad = LRemisioD.VentaReal - LRemisioD.vcosto;

                        //Calculo de IVA    
                        if (LRemisioD.impuesto >= 1)
                        {
                            // Calculo de IVAa
                            LRemisioD.VentaSinIVA = LRemisioD.VentaReal / 1.16;
                            LRemisioD.VentaIVA = LRemisioD.VentaReal - LRemisioD.VentaSinIVA;
                            LRemisioD.VentaConIVA = LRemisioD.VentaSinIVA + LRemisioD.VentaIVA;
                        }
                        else if (LRemisioD.impuesto == 0)
                        {

                            // Y también es igual a la venta con IVA
                            LRemisioD.VentaConIVA = LRemisioD.VentaReal;
                        }

                        //Calculo para el valor de inventario tomando la existencia
                        LRemisioD.VInventario = LRemisioD.costo * LRemisioD.Existencia;



                        rawData.Add(LRemisioD);
                    }

                    var groupedData = rawData.GroupBy(d => new
                    {
                        d.codigobarras,
                        d.productoNombre,
                        d.almacen,
                        d.categoria,
                        d.grupo,
                        d.costo,
                        d.preciopub,
                        d.Existencia,
                        d.deportiva,
                        d.ultcost
                    })
                    .Select(g => new GruopRemisioDModel
                    {
                        Group_codigobarras = g.Key.codigobarras,
                        Group_productoNombre = g.Key.productoNombre,
                        Group_almacen = g.Key.almacen,
                        Group_categoria = g.Key.categoria,
                        Group_grupo = g.Key.grupo,
                        Group_costo = g.Key.costo,
                        Group_Existencia = g.Key.Existencia,
                        Group_preciopub = g.Key.preciopub,


                        //Todos estos valores se suman
                        Group_cantidads = g.Sum(x => x.cantidads),
                        Group_VentaReal = g.Sum(x => x.VentaReal),
                        Group_VentaSinIVA = g.Sum(x => x.VentaSinIVA),    // Sumatoria de VentaSinIVA
                        Group_VentaIVA = g.Sum(x => x.VentaIVA),          // Sumatoria de VentaIVA
                        Group_VentaConIVA = g.Sum(x => x.VentaConIVA),    // Sumatoria de VentaConIVA
                        Group_total_fijo = g.Where(x => x.deportiva == "F").Sum(x => x.total_fijo),

                        Group_total_depor = g.Where(x => x.deportiva != "F").Sum(x => x.total_depor),


                        Depor_string = g.Key.deportiva,
                        ultcost = g.Key.ultcost,

                        //Group_total_depor = g.Key.preciopub,

                        Group_vcosto = g.Sum(x => x.vcosto),
                        Group_VInventario = g.Sum(x => x.VInventario),
                        Group_utilidad = g.Sum(x => x.utilidad),
                        Gruop_fecha = (DateTime)g.Max(x => x.fecha),

                    })
                    .ToList();

                    var jsonResult = Json(groupedData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VproductosAdminTotal
        [HttpPost]
        public ActionResult filtrarTablaRJoy(string fechaInicio, string fechaFin, string almacen, string[] categoria)
        {
            DateTime fechaInicioParsed;
            DateTime fechaFinParsed;

            if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                !DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                string baseQuery = @"SELECT * FROM joyeria.dbo.VRemisioDJoy
                 WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;


                cn.Open();

                // Verificar si se seleccionaron categorías
                if (categoria != null && categoria.Length > 0)
                {
                    // Construir la cláusula OR para las categorías
                    StringBuilder categoriaClause = new StringBuilder();
                    for (int i = 0; i < categoria.Length; i++)
                    {
                        string paramName = "@categoria" + i;
                        categoriaClause.Append("categoria = ").Append(paramName);
                        cmd.Parameters.AddWithValue(paramName, categoria[i]);
                        if (i < categoria.Length - 1)
                        {
                            categoriaClause.Append(" OR ");
                        }
                    }

                    cmd.CommandText += " AND (" + categoriaClause.ToString() + ")";
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<RemisioDJoyModel> rawData = new List<RemisioDJoyModel>();

                    while (dr.Read())
                    {
                        RemisioDJoyModel LRemisioDJoy = new RemisioDJoyModel();
                        LRemisioDJoy.codigo_barras = dr["codigo_barras"].ToString();
                        LRemisioDJoy.Nombre_largo = dr["descripcion_larga"].ToString();
                        LRemisioDJoy.almacen = dr["almacen"].ToString();
                        LRemisioDJoy.categoria = dr["categoria"].ToString();
                        LRemisioDJoy.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRemisioDJoy.peso = dr["peso"] != DBNull.Value ? Math.Round(Convert.ToDouble(dr["peso"]), 2) : 0;
                        LRemisioDJoy.VentaReal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;
                        LRemisioDJoy.deportiva = dr["deportiva"].ToString();
                        // Conversión de la fecha
                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioDJoy.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioDJoy.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }
                        LRemisioDJoy.Existencia = dr["if"] != DBNull.Value ? Convert.ToInt64(dr["if"]) : 0;


                        //saber cual es deportiva
                        LRemisioDJoy.total_depor = LRemisioDJoy.VentaReal;

                        // Calculo de IVAa
                        LRemisioDJoy.VentaSinIVA = LRemisioDJoy.VentaReal / 1.16;
                        LRemisioDJoy.VentaIVA = LRemisioDJoy.VentaReal - LRemisioDJoy.VentaSinIVA;
                        LRemisioDJoy.VentaConIVA = LRemisioDJoy.VentaSinIVA + LRemisioDJoy.VentaIVA;


                        rawData.Add(LRemisioDJoy);
                    }

                    var groupedData = rawData.GroupBy(d => new
                    {
                        d.codigo_barras,
                        d.Nombre_largo,
                        d.almacen,
                        d.categoria,
                        d.Existencia
                    })
                    .Select(g => new GruopRemisioDModel
                    {
                        Group_codigobarras = g.Key.codigo_barras,
                        Group_productoNombre = g.Key.Nombre_largo,
                        Group_almacen = g.Key.almacen,
                        Group_categoria = g.Key.categoria,
                        Group_Existencia = g.Key.Existencia,

                        //Todos estos valores se suman
                        Group_cantidads = g.Sum(x => x.cantidads),
                        Group_peso = Math.Round(g.Sum(x => x.peso), 2),
                        Group_VentaReal = g.Sum(x => x.VentaReal),
                        Group_VentaSinIVA = g.Sum(x => x.VentaSinIVA),    // Sumatoria de VentaSinIVA
                        Group_VentaIVA = g.Sum(x => x.VentaIVA),          // Sumatoria de VentaIVA
                        Group_VentaConIVA = g.Sum(x => x.VentaConIVA),    // Sumatoria de VentaConIVA
                        Group_total_depor = g.Where(x => x.deportiva != "F").Sum(x => x.total_depor),
                        Gruop_fecha = g.Max(x => x.fecha),

                    })
                    .ToList();

                    var jsonResult = Json(groupedData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VVendedoresAdminTotal
        [HttpPost]
        public ActionResult filtrarTablaV(string fechaInicio, string fechaFin, string almacen, string[] vendedor)
        {
            DateTime fechaInicioParsed;
            DateTime fechaFinParsed;

            if (!DateTime.TryParseExact(fechaInicio, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioParsed) ||
                !DateTime.TryParseExact(fechaFin, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFinParsed))
            {
                return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
            }

            List<VistaVendedorModel> listaVendedores = new List<VistaVendedorModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"SELECT * FROM VVendedor
             WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                // Verificar si se seleccionaron los vendedores
                if (vendedor != null && vendedor.Length > 0)
                {
                    List<string> vendedorConditions = new List<string>();
                    for (int i = 0; i < vendedor.Length; i++)
                    {
                        string paramName = "@nombre" + i;
                        vendedorConditions.Add("nombre = " + paramName);
                        cmd.Parameters.AddWithValue(paramName, vendedor[i]);
                    }

                    if (vendedorConditions.Any())
                    {
                        cmd.CommandText += " AND (" + string.Join(" OR ", vendedorConditions) + ")";
                    }
                }

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<VistaVendedorModel> rawData = new List<VistaVendedorModel>();

                    while (dr.Read())
                    {
                        VistaVendedorModel LVendedores = new VistaVendedorModel();

                        // Conversión de la fecha

                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LVendedores.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LVendedores.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        LVendedores.folio_remision = dr["folio_remision"].ToString();
                        LVendedores.nombre = dr["nombre"].ToString();
                        LVendedores.almacen = dr["almacen"].ToString();
                        LVendedores.cuantosint = dr["cuantosint"] != DBNull.Value ? Convert.ToInt32(dr["cuantosint"]) : 0;
                        LVendedores.totalventa = dr["totalventa"] != DBNull.Value ? Convert.ToInt64(dr["totalventa"]) : 0;
                        LVendedores.comisiondepor = dr["comisiondepor"] != DBNull.Value ? Convert.ToInt64(dr["comisiondepor"]) : 0;
                        LVendedores.comisionfijo = dr["comisionfijo"] != DBNull.Value ? Convert.ToInt64(dr["comisionfijo"]) : 0;


                        LVendedores.ventatotalporvendedor = LVendedores.totalventa / (LVendedores.cuantosint != 0 ? LVendedores.cuantosint : 1);
                        var resultado1 = LVendedores.comisiondepor + LVendedores.comisionfijo;
                        LVendedores.totalcomision = resultado1 / (LVendedores.cuantosint != 0 ? LVendedores.cuantosint : 1);


                        rawData.Add(LVendedores);
                    }

                    var groupedData = rawData.GroupBy(d => new
                    {
                        d.nombre,
                        d.almacen
                    })
                    .Select(g => new GruopRemisioDModel
                    {
                        Group_nombre = g.Key.nombre,
                        Group_almacen = g.Key.almacen,

                        //sumatoria
                        Group_VentatotalporvendedorSum = g.Sum(item => item.ventatotalporvendedor),
                        Group_TotalcomisionSum = g.Sum(item => item.totalcomision),
                        Gruop_fecha = g.Max(x => x.fecha),

                    })
                    .ToList();

                    var jsonResult = Json(groupedData, JsonRequestBehavior.AllowGet);
                    jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                    return jsonResult;

                }
            }
        }

        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE VTipoPagoAdminTotal
        //PRUEBA PIVOT-----------------------------------------------
        [HttpPost]
        public ActionResult filtrarTablaTP(string fechaInicio, string fechaFin, string almacen)
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
                string baseQuery = @"SELECT * FROM deptopago WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicioParsed);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFinParsed);
                cmd.Parameters.AddWithValue("@almacen", almacen);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    List<AlmacenTPModel> rawData = new List<AlmacenTPModel>();

                    while (dr.Read())
                    {
                        AlmacenTPModel LAlmacenTP = new AlmacenTPModel();

                        LAlmacenTP.categoria = dr["categoria"].ToString();
                        LAlmacenTP.total = dr["total"] != DBNull.Value ? Math.Round(Convert.ToDouble(dr["total"]), 2) : 0;
                        LAlmacenTP.moneda = dr["moneda"].ToString();
                        LAlmacenTP.almacen = dr["almacen"].ToString();
                        LAlmacenTP.folio_remision = dr["folio_remision"].ToString();

                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LAlmacenTP.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LAlmacenTP.fecha = DateTime.MinValue;
                        }

                        rawData.Add(LAlmacenTP);
                    }

                    // Continuación de filtrarTablaTP...

                    Dictionary<string, Dictionary<string, double>> pivotData = new Dictionary<string, Dictionary<string, double>>();

                    foreach (var row in rawData)
                    {
                        if (!pivotData.ContainsKey(row.moneda))
                        {
                            pivotData[row.moneda] = new Dictionary<string, double>();
                        }

                        if (!pivotData[row.moneda].ContainsKey(row.categoria))
                        {
                            pivotData[row.moneda][row.categoria] = 0;
                        }

                        pivotData[row.moneda][row.categoria] += row.total;
                    }

                    return Json(pivotData, JsonRequestBehavior.AllowGet);
                }
            }
        }





        [HttpPost]
        public ActionResult procesarValoresEntregados2()
        {

            //List<Tuple<string, string,string>> data = new List<Tuple<string, string, string>>();
            List<string> denominacionList = new List<string>();
            List<string> cantidadList = new List<string>();
            List<string> importeList = new List<string>();
            List<string> valoresEntregadosList = new List<string>();

            int index = 0;

            double TipoCambio = 0;
            int IdMoneda = 0;
            int IdValoresEntregados = 0;

            string FechaParam = "";
            int IdAlmacenParam = 0;
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
                    valoresEntregadosList.Add(Request.Form[key]);
                    //IdValoresEntregados = Request.Form[key] != DBNull.Value ? Convert.ToInt32(Request.Form[key]) : 0;

                }
                if (key.StartsWith("FechaParam"))
                {
                    FechaParam = Request.Form[key];
                }
                if (key.StartsWith("IdAlmacenParam"))
                {
                    IdAlmacenParam = Convert.ToInt32(Request.Form[key]);
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
            Fecha.ToString("yy-mm-dd");

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

                    if (TipoCambio > 0 && IdMoneda > 0 && Convert.ToInt32(valoresEntregadosList[0]) > 0)
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
                            cmd.Parameters.AddWithValue("Fecha", FechaParam);
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
                        , importeList, valoresEntregadosList, IdAlmacenParam, FechaParam);
                    }


                }

            }

            //return null;

            return RedirectToAction("VCajaAlmacenesFechaMonedaEfectivo", new { IdAlmacen = IdAlmacenParam, Fecha = FechaParam, IdMoneda = IdMonedaParam });

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

        //
        [HttpPost]
        public ActionResult procesarValoresEntregadosVoucher()
        {

            //List<Tuple<string, string,string>> data = new List<Tuple<string, string, string>>();
            List<string> denominacionList = new List<string>();
            List<string> cantidadList = new List<string>();
            List<string> importeList = new List<string>();
            List<string> referenciaList = new List<string>();
            List<string> idRegstroVoucherList = new List<string>();

            int index = 0;

            int IdVoucher = 0;
            int IdValoresEntregados = 0;

            string FechaParam = "";
            int IdAlmacenParam = 0;
            string IdVoucherParam = "";

            foreach (string key in Request.Form.AllKeys)
            {
                string value = "";


                if (key.StartsWith("importe"))
                {
                    value = Request.Form[key];
                    importeList.Add(value);
                    index++;
                }
                if (key.StartsWith("referencia"))
                {
                    value = Request.Form[key];
                    referenciaList.Add(value);
                }

                if (key.StartsWith("IdVoucher"))
                {
                    IdVoucher = Convert.ToInt32((Request.Form[key]));
                }
                if (key.StartsWith("IdValoresEntregados"))
                {
                    //IdValoresEntregados = Convert.ToInt32((Request.Form[key]));
                    idRegstroVoucherList.Add(Request.Form[key]);
                    //IdValoresEntregados = Request.Form[key] != DBNull.Value ? Convert.ToInt32(Request.Form[key]) : 0;

                }
                if (key.StartsWith("FechaParam"))
                {
                    FechaParam = Request.Form[key];
                }
                if (key.StartsWith("IdAlmacenParam"))
                {
                    IdAlmacenParam = Convert.ToInt32(Request.Form[key]);
                }
                if (key.StartsWith("IdVoucherParam"))
                {
                    IdVoucherParam = Request.Form[key];
                }
            }

            //Response.Write("index" + index);
            //Response.Write(TipoCambio);
            //int IdAlmacen = 211;

            //DateTime Fecha = DateTime.Now;


            bool IsValid = true;

            if (IsValid && IdAlmacenParam > 0)
            {
                Response.Write("actualizar");

                for (int i = 0; i < index; i++)
                {

                    Response.Write("for");


                    Response.Write(FechaParam);

                    if (IdVoucher > 0 && Convert.ToInt32(idRegstroVoucherList[0]) > 0)
                    {
                        using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                        {
                            cn.Open();
                            float importe = 0;
                            SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ModificarVoucher", cn);

                            cmd.Parameters.AddWithValue("Importe", importeList[i]);
                            cmd.Parameters.AddWithValue("Referencia", referenciaList[i]);
                            cmd.Parameters.AddWithValue("IdRegistroVoucher", idRegstroVoucherList[i]);
                            cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("Mensaje", SqlDbType.NVarChar, 100).Direction = ParameterDirection.Output;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        InsertarValoresDeEfectivoVoucher(IdVoucher, index, denominacionList, cantidadList, importeList, idRegstroVoucherList, referenciaList, IdAlmacenParam, FechaParam);
                    }
                }
            }
            //return null;
            return RedirectToAction("VCajaAlmacenesFechaMonedaEfectivoVoucher", new { IdAlmacen = IdAlmacenParam, Fecha = FechaParam, IdVoucher = IdVoucherParam });
        }
        //

        public void InsertarValoresDeEfectivo(int IdMoneda, double TipoCambio, int index, List<string> denominacionList, List<string> cantidadList
            , List<string> importeList, List<string> valoresEntregadosList, int IdAlmacen, string FechaParam)
        {

            //List<string> denominacionList = denominacionList2;
            //List<string> cantidadList = CantidadList2;
            //List<string> importeList = importeList2;
            //List<string> valoresEntregadosList = valoresEntregadosList2;


            //int IdAlmacen = 211;

            //return null;

            //DateTime Fecha = DateTime.Now;

            bool IsValid = false;
            int IdRegistroValoresEntregados = 0;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioEfectivo", cn);

                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
                cmd.Parameters.AddWithValue("Fecha", FechaParam);
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

                    Response.Write(FechaParam);

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
                            cmd.Parameters.AddWithValue("Fecha", FechaParam);
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

        public void InsertarValoresDeEfectivoVoucher(int IdVoucher, int index, List<string> denominacionList, List<string> cantidadList
           , List<string> importeList, List<string> valoresEntregadosList, List<string> referenciaList, int IdAlmacen, string FechaParam)
        {

            //List<string> denominacionList = denominacionList2;
            //List<string> cantidadList = CantidadList2;
            //List<string> importeList = importeList2;
            //List<string> valoresEntregadosList = valoresEntregadosList2;

            // obtener por parametro
            //int IdAlmacen = 211;

            //return null;

            //DateTime Fecha = DateTime.Now;
            //Fecha.ToString("yy-mm-dd");

            bool IsValid = false;
            int IdRegistroDiaVoucher = 0;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioVoucher", cn);

                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("IdVoucher", IdVoucher);
                cmd.Parameters.AddWithValue("Fecha", FechaParam);

                cmd.Parameters.Add("IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("IdRegistroDiaVoucher", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                IsValid = Convert.ToBoolean(cmd.Parameters["IsValid"].Value);
                IdRegistroDiaVoucher = Convert.ToInt32(cmd.Parameters["IdRegistroDiaVoucher"].Value);

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

                    //Response.Write(FechaParam);

                    if (IdVoucher > 0)
                    {

                        //verificar primero que no haya datos existentes sql
                        //sql

                        using (SqlConnection cn = new SqlConnection(conexionDBHoka))
                        {
                            cn.Open();

                            float importe = 0;

                            SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_RegistrarVouchers", cn);

                            cmd.Parameters.AddWithValue("Cantidad", 1);
                            cmd.Parameters.AddWithValue("Importe", importeList[i]);
                            cmd.Parameters.AddWithValue("Referencia", referenciaList[i]);
                            //cmd.Parameters.AddWithValue("IdMonedaDenominacion", denominacionList[i]);
                            //cmd.Parameters.AddWithValue("TipoCambio", 1);
                            cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                            cmd.Parameters.AddWithValue("IdVoucher", IdVoucher);
                            cmd.Parameters.AddWithValue("Fecha", FechaParam);
                            cmd.Parameters.AddWithValue("IdRegistroDiaVoucher", IdRegistroDiaVoucher);

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

        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE CTicketAdminTotal
        [HttpPost]
        public ActionResult filtrarTablaCT(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaCT(fechaInicio, out fechaInicioParsed) || !fechaTablaCT(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaCT("Formato de fecha incorrecto.");
                }

                var data = DBTablaCT(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaCT($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaCT(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioMModel> DBTablaCT(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioMModel> rawData = new List<RemisioMModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"SELECT * FROM VRemisioMWeb
             WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioMModel LRemisioMWeb = new RemisioMModel();
                        LRemisioMWeb.folio_remision = dr["folio_remision"].ToString();
                        LRemisioMWeb.almacen = dr["almacen"].ToString();
                        LRemisioMWeb.tipo_cambio = dr["tipo_cambio"] != DBNull.Value ? Convert.ToDouble(dr["tipo_cambio"]) : 0.0;
                        LRemisioMWeb.stotal = dr["stotal"] != DBNull.Value ? Convert.ToDouble(dr["stotal"]) : 0.0;


                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioMWeb.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioMWeb.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        rawData.Add(LRemisioMWeb);
                    }
                }

                cn.Close();
            }

            return rawData;
        }
        private JsonResult JsonErrorTablaCT(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }


        //FILTRO QUE NOS SERVIRA PARA LA VISTA DE CTicketAdminTotalJoy
        [HttpPost]
        public ActionResult filtrarTablaCTJoy(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaCTJoy(fechaInicio, out fechaInicioParsed) || !fechaTablaCTJoy(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaCTJoy("Formato de fecha incorrecto.");
                }

                var data = DBTablaCTJoy(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaCTJoy($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaCTJoy(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioMModel> DBTablaCTJoy(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioMModel> rawData = new List<RemisioMModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                string baseQuery = @"SELECT * FROM joyeria.dbo.VRemisioMWebJoy
             WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioMModel LRemisioMWeb = new RemisioMModel();
                        LRemisioMWeb.folio_factura = dr["folio_factura"].ToString();
                        LRemisioMWeb.almacen = dr["almacen"].ToString();
                        LRemisioMWeb.tipo_cambio = dr["tipo_cambio"] != DBNull.Value ? Convert.ToDouble(dr["tipo_cambio"]) : 0.0;
                        LRemisioMWeb.stotal = dr["stotal"] != DBNull.Value ? Convert.ToDouble(dr["stotal"]) : 0.0;


                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRemisioMWeb.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRemisioMWeb.fecha = DateTime.MinValue; // o cualquier otro valor predeterminado
                        }

                        rawData.Add(LRemisioMWeb);
                    }
                }

                cn.Close();
            }

            return rawData;
        }
        private JsonResult JsonErrorTablaCTJoy(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Filtrar productos rotados
        [HttpPost]
        public ActionResult filtrarTablaRotacion(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaRotacion(fechaInicio, out fechaInicioParsed) || !fechaTablaRotacion(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaRotacion("Formato de fecha incorrecto.");
                }

                var data = DBTablaRotacion(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaRotacion($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaRotacion(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioDModel> DBTablaRotacion(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioDModel> rawData = new List<RemisioDModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                string baseQuery = @"SELECT * FROM VRotacion
             WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioDModel LRotacionRD = new RemisioDModel();
                        LRotacionRD.codigobarras = dr["codigobarras"].ToString();
                        LRotacionRD.productoNombre = dr["descripcion_larga"].ToString(); //la propiedad del modelo es diferente el nombre pero el mismo valor
                        LRotacionRD.almacen = dr["almacen"].ToString(); //la propiedad del modelo es diferente el nombre pero el mismo valor
                        LRotacionRD.categoria = dr["categoria"].ToString();
                        LRotacionRD.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRotacionRD.Existencia = dr["if"] != DBNull.Value ? Convert.ToInt64(dr["if"]) : 0;


                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRotacionRD.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRotacionRD.fecha = null; // o cualquier otro valor predeterminado
                        }

                        rawData.Add(LRotacionRD);
                    }
                }

                cn.Close();
            }
            var groupedData = rawData
                    .GroupBy(r => new
                    {
                        r.codigobarras,
                        r.productoNombre,
                        r.almacen,
                        r.categoria,
                        r.Existencia
                    })
                    .Select(g =>
                    {
                        return new RemisioDModel
                        {
                            codigobarras = g.Key.codigobarras,
                            productoNombre = g.Key.productoNombre,
                            almacen = g.Key.almacen,
                            categoria = g.Key.categoria,
                            Existencia = g.Key.Existencia,
                            fecha = g.Max(x => x.fecha),
                            cantidads = g.Sum(x => x.cantidads)
                        };
                    }).ToList();

            return groupedData;
        }
        private JsonResult JsonErrorTablaRotacion(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Filtrar productos NO rotados
        [HttpPost]
        public ActionResult filtrarTablaRotacionNull(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaRotacionNull(fechaInicio, out fechaInicioParsed) || !fechaTablaRotacionNull(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaRotacionNull("Formato de fecha incorrecto.");
                }

                var data = DBTablaRotacionNull(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaRotacionNull($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaRotacionNull(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioDModel> DBTablaRotacionNull(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioDModel> rawData = new List<RemisioDModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ProductosNoVendidosRemisioD", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@Almacen", almacen);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioDModel LRotacionRD = new RemisioDModel();
                        LRotacionRD.codigobarras = dr["codigobarra"].ToString();
                        LRotacionRD.productoNombre = dr["Nombre"].ToString();
                        LRotacionRD.almacen = dr["almacen"].ToString();
                        // Asegúrate de que "if" es el nombre correcto en el resultado del procedimiento almacenado
                        LRotacionRD.Existencia = dr["Existencia"] != DBNull.Value ? Convert.ToInt64(dr["Existencia"]) : 0;


                        rawData.Add(LRotacionRD);
                    }
                }

                cn.Close();
            }

            var groupedData = rawData
                    .GroupBy(r => new
                    {
                        r.codigobarras,
                        r.productoNombre,
                        r.almacen,
                        r.Existencia
                    })
                    .Select(g =>
                    {
                        return new RemisioDModel
                        {
                            codigobarras = g.Key.codigobarras,
                            productoNombre = g.Key.productoNombre,
                            almacen = g.Key.almacen,
                            Existencia = g.Key.Existencia,
                        };
                    }).ToList();


            return groupedData;
        }

        private JsonResult JsonErrorTablaRotacionNull(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }




        //Filtrar productos rotados
        [HttpPost]
        public ActionResult filtrarTablaRotacionJoy(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaRotacionJoy(fechaInicio, out fechaInicioParsed) || !fechaTablaRotacionJoy(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaRotacionJoy("Formato de fecha incorrecto.");
                }

                var data = DBTablaRotacionJoy(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaRotacionJoy($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaRotacionJoy(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioDModel> DBTablaRotacionJoy(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioDModel> rawData = new List<RemisioDModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                string baseQuery = @"SELECT * FROM joyeria.dbo.VRotacionJoy
             WHERE fecha >= @fechaInicio AND fecha <= @fechaFin AND almacen = @almacen";

                SqlCommand cmd = new SqlCommand(baseQuery, cn);

                cmd.Parameters.AddWithValue("@fechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@fechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@almacen", almacen);
                cmd.CommandType = CommandType.Text;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioDModel LRotacionRD = new RemisioDModel();
                        LRotacionRD.codigobarras = dr["codigo_barras"].ToString();
                        LRotacionRD.productoNombre = dr["descripcion_larga"].ToString(); //la propiedad del modelo es diferente el nombre pero el mismo valor
                        LRotacionRD.almacen = dr["almacen"].ToString(); //la propiedad del modelo es diferente el nombre pero el mismo valor
                        LRotacionRD.categoria = dr["categoria"].ToString();
                        LRotacionRD.cantidads = dr["cantidads"] != DBNull.Value ? Convert.ToInt64(dr["cantidads"]) : 0;
                        LRotacionRD.Existencia = dr["if"] != DBNull.Value ? Convert.ToInt64(dr["if"]) : 0;




                        //DateTime fechaUno = Convert.ToDateTime(txtDate1.Text).Date;
                        //DateTime fechaDos = Convert.ToDateTime(txtDate2.Text).Date;

                        TimeSpan difFechas = fechaFin - fechaInicio;

                        int distance_between_days = (int)difFechas.TotalDays;

                        //string dias = Convert.ToString(days);
                        double pvd = 0;
                        double rotacion_mensual = 0;

                        if (LRotacionRD.cantidads >= 0 && distance_between_days >= 0)
                        {
                            if (distance_between_days == 0)
                            {
                                distance_between_days = 1;
                            }
                            pvd = LRotacionRD.cantidads / distance_between_days;
                        }

                        if (LRotacionRD.Existencia > 0 && pvd > 0)
                        {

                            rotacion_mensual = LRotacionRD.Existencia / pvd;
                        }

                        LRotacionRD.pvd = pvd;
                        LRotacionRD.rotacion_mensual = rotacion_mensual;

                        // venta unidades / distancia en dias = promedio de ventas diaria
                        // rotacion mensual = existencia / promedio de ventas diaria = da dias de existencia / 30


                        if (DateTime.TryParse(dr["fecha"].ToString(), out DateTime fecha))
                        {
                            LRotacionRD.fecha = fecha.ToUniversalTime();
                        }
                        else
                        {
                            LRotacionRD.fecha = null; // o cualquier otro valor predeterminado
                        }

                        rawData.Add(LRotacionRD);
                    }
                }

                cn.Close();
            }
            var groupedData = rawData
                    .GroupBy(r => new
                    {
                        r.codigobarras,
                        r.productoNombre,
                        r.almacen,
                        r.categoria,
                        r.Existencia,
                        r.pvd,
                        r.rotacion_mensual
                    })
                    .Select(g =>
                    {
                        return new RemisioDModel
                        {
                            codigobarras = g.Key.codigobarras,
                            productoNombre = g.Key.productoNombre,
                            almacen = g.Key.almacen,
                            categoria = g.Key.categoria,
                            Existencia = g.Key.Existencia,
                            fecha = g.Max(x => x.fecha),
                            cantidads = g.Sum(x => x.cantidads),
                            pvd = g.Key.pvd,
                            rotacion_mensual = g.Key.rotacion_mensual,
                        };
                    }).ToList();

            return groupedData;
        }
        private JsonResult JsonErrorTablaRotacionJoy(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }


        //Filtrar productos NO rotados
        [HttpPost]
        public ActionResult filtrarTablaRotacionNullJoy(string fechaInicio, string fechaFin, string almacen)
        {
            try
            {
                DateTime fechaInicioParsed;
                DateTime fechaFinParsed;

                if (!fechaTablaRotacionNullJoy(fechaInicio, out fechaInicioParsed) || !fechaTablaRotacionNullJoy(fechaFin, out fechaFinParsed))
                {
                    return JsonErrorTablaRotacionNullJoy("Formato de fecha incorrecto.");
                }

                var data = DBTablaRotacionNullJoy(fechaInicioParsed, fechaFinParsed, almacen);

                var jsonResult = Json(data, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
                return jsonResult;
            }
            catch (Exception ex)
            {
                // Aquí puedes registrar el error en algún logger si lo tienes.
                return JsonErrorTablaRotacionNullJoy($"Error del servidor: {ex.Message}");
            }
        }
        private bool fechaTablaRotacionNullJoy(string dateStr, out DateTime dateParsed)
        {
            return DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);
        }
        private List<RemisioDModel> DBTablaRotacionNullJoy(DateTime fechaInicio, DateTime fechaFin, string almacen)
        {
            List<RemisioDModel> rawData = new List<RemisioDModel>();

            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                SqlCommand cmd = new SqlCommand("joyeria.dbo.sp_ProductosNoVendidosRemisioDJoy", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", fechaFin);
                cmd.Parameters.AddWithValue("@Almacen", almacen);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        RemisioDModel LRotacionRD = new RemisioDModel();
                        LRotacionRD.codigobarras = dr["codigo_barras"].ToString();
                        LRotacionRD.productoNombre = dr["Nombre_Largo"].ToString();
                        LRotacionRD.almacen = dr["almacen"].ToString();
                        // Asegúrate de que "if" es el nombre correcto en el resultado del procedimiento almacenado
                        LRotacionRD.Existencia = dr["Existencia"] != DBNull.Value ? Convert.ToInt64(dr["Existencia"]) : 0;

                        TimeSpan difFechas = fechaFin - fechaInicio;

                        int distance_between_days = (int)difFechas.TotalDays;

                        //string dias = Convert.ToString(days);
                        double pvd = 0;
                        double rotacion_mensual = 0;

                        if (LRotacionRD.cantidads >= 0 && distance_between_days >= 0)
                        {
                            if (distance_between_days == 0)
                            {
                                distance_between_days = 1;
                            }
                            pvd = LRotacionRD.cantidads / distance_between_days;
                        }

                        if (LRotacionRD.Existencia > 0 && pvd > 0)
                        {

                            rotacion_mensual = LRotacionRD.Existencia / pvd;
                        }

                        LRotacionRD.pvd = pvd;
                        LRotacionRD.rotacion_mensual = rotacion_mensual;

                        // venta unidades / distancia en dias = promedio de ventas diaria
                        // rotacion mensual = existencia / promedio de ventas diaria = da dias de existencia / 30
                        rawData.Add(LRotacionRD);
                    }
                }

                cn.Close();
            }

            var groupedData = rawData
                    .GroupBy(r => new
                    {
                        r.codigobarras,
                        r.productoNombre,
                        r.almacen,
                        r.Existencia,
                        r.pvd,
                        r.rotacion_mensual
                    })
                    .Select(g =>
                    {
                        return new RemisioDModel
                        {
                            codigobarras = g.Key.codigobarras,
                            productoNombre = g.Key.productoNombre,
                            almacen = g.Key.almacen,
                            Existencia = g.Key.Existencia,
                            pvd = g.Key.pvd,
                            rotacion_mensual = g.Key.rotacion_mensual,
                        };
                    }).ToList();


            return groupedData;
        }

        private JsonResult JsonErrorTablaRotacionNullJoy(string message)
        {
            return Json(new { success = false, message = message }, JsonRequestBehavior.AllowGet);
        }









        //CONTROLADORES PARA OBTENER LOS DATOS PARA EL FILTRADO DE ARRIBA FiltrarTablaR
        public ActionResult ObtenerCategorias(string ObAlmacen)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                // Obtener Categorias
                SqlCommand cmdCategoria = new SqlCommand("SELECT distinct categoria from remisioD where almacen = @almacen", cn);

                cmdCategoria.Parameters.AddWithValue("@almacen", ObAlmacen);
                cmdCategoria.CommandType = CommandType.Text;

                using (SqlDataReader drCategoria = cmdCategoria.ExecuteReader())
                {
                    List<RemisioDModel> ListaRemisioD = new List<RemisioDModel>();

                    while (drCategoria.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();
                        LRemisioD.categoria = drCategoria["categoria"].ToString();
                        ListaRemisioD.Add(LRemisioD);
                    }

                    return Json(ListaRemisioD, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //CONTROLADORES PARA OBTENER LOS DATOS PARA EL FILTRADO DE ARRIBA FiltrarTablaRJoy
        public ActionResult ObtenerCategoriasJoy(string ObAlmacen)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBJoy))
            {
                cn.Open();

                // Obtener Categorias
                SqlCommand cmdCategoria = new SqlCommand("SELECT distinct categoria from joyeria.dbo.VRemisioDJoy where almacen = @almacen", cn);

                cmdCategoria.Parameters.AddWithValue("@almacen", ObAlmacen);
                cmdCategoria.CommandType = CommandType.Text;

                using (SqlDataReader drCategoria = cmdCategoria.ExecuteReader())
                {
                    List<RemisioDModel> ListaRemisioD = new List<RemisioDModel>();

                    while (drCategoria.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();
                        LRemisioD.categoria = drCategoria["categoria"].ToString();
                        ListaRemisioD.Add(LRemisioD);
                    }

                    return Json(ListaRemisioD, JsonRequestBehavior.AllowGet);
                }
            }
        }


        public ActionResult ObtenerGrupos(string ObAlmacen, string ObCategoria)
        {
            string[] ObCategoriaArray = JsonConvert.DeserializeObject<string[]>(ObCategoria);

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                var parameters = new List<string>();

                if (ObCategoriaArray != null)
                {
                    for (int i = 0; i < ObCategoriaArray.Length; i++)
                    {
                        parameters.Add("@categoria" + i);
                    }
                }

                SqlCommand cmdGrupo = (ObCategoriaArray != null && ObCategoriaArray.Length > 0)
                    ? new SqlCommand("SELECT DISTINCT grupo FROM remisioD WHERE almacen = @almacen AND categoria IN (" + string.Join(",", parameters) + ")", cn)
                    : new SqlCommand("SELECT DISTINCT grupo FROM remisioD WHERE almacen = @almacen", cn);

                cmdGrupo.Parameters.AddWithValue("@almacen", ObAlmacen);

                if (ObCategoriaArray != null)
                {
                    for (int i = 0; i < ObCategoriaArray.Length; i++)
                    {
                        cmdGrupo.Parameters.AddWithValue("@categoria" + i, ObCategoriaArray[i]);
                    }
                }

                cmdGrupo.CommandType = CommandType.Text;

                using (SqlDataReader drGrupo = cmdGrupo.ExecuteReader())
                {
                    List<RemisioDModel> ListaRemisioD = new List<RemisioDModel>();

                    while (drGrupo.Read())
                    {
                        RemisioDModel LRemisioD = new RemisioDModel();
                        LRemisioD.grupo = drGrupo["grupo"].ToString();
                        ListaRemisioD.Add(LRemisioD);
                    }

                    return Json(ListaRemisioD, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //CONTROLADORES PARA OBTENER LOS DATOS PARA EL FILTRADO DE ARRIBA FiltrarTablaA
        public ActionResult ObtenerVendedores(string ObAlmacen)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                // Obtener Vendedores
                SqlCommand cmdVendedor = new SqlCommand("SELECT distinct nombre from VVendedor where almacen = @almacen", cn);

                cmdVendedor.Parameters.AddWithValue("@almacen", ObAlmacen);
                cmdVendedor.CommandType = CommandType.Text;

                using (SqlDataReader drVendedor = cmdVendedor.ExecuteReader())
                {
                    List<VistaVendedorModel> ListaVendedor = new List<VistaVendedorModel>();

                    while (drVendedor.Read())
                    {
                        VistaVendedorModel LVendedorView = new VistaVendedorModel();
                        LVendedorView.nombre = drVendedor["nombre"].ToString();
                        ListaVendedor.Add(LVendedorView);
                    }

                    return Json(ListaVendedor, JsonRequestBehavior.AllowGet);
                }
            }
        }






        //PROCEDIMIENTOS Y FUNCIONES PARA EL REGISTRO DE USUARIOS DE ADMIN TOTAL       
        [HttpGet]
        [ValidarSesion(idRol: 3)]
        public ActionResult RegistrarUsuario()
        {
            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View();
        }

        //PROCEDIMIENTO ALM PARA AGREGAR UN USUARIO JUNTO CON ALMACENES
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult RegistrarUsuario(UsuarioModel oUsuario, string FechaNacimientoU)
        {

            // Validar campos vacíos
            if (string.IsNullOrEmpty(oUsuario.NombreU))
            {
                return Json(new { success = false, message = "El Nombre es obligatorio" });
            }
            if (string.IsNullOrEmpty(oUsuario.ApellidoU))
            {
                return Json(new { success = false, message = "El Apellido es obligatorio" });
            }

            DateTime fechaNacimientoParsed;

            // Intenta parsear la fecha de nacimiento
            if (!DateTime.TryParseExact(FechaNacimientoU, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimientoParsed))
            {
                return Json(new { success = false, message = "Formato de fecha de nacimiento incorrecto." });
            }


            // Valida si la fecha de nacimiento está en el rango permitido
            DateTime minDate = new DateTime(1920, 1, 1);
            DateTime maxDate = DateTime.Today;
            if (fechaNacimientoParsed < minDate || fechaNacimientoParsed > maxDate)
            {
                return Json(new { success = false, message = "La fecha de nacimiento no es válida" });
            }

            if (string.IsNullOrEmpty(oUsuario.Correo))
            {
                return Json(new { success = false, message = "El Correo es obligatorio" });
            }
            if (string.IsNullOrEmpty(oUsuario.Clave))
            {
                return Json(new { success = false, message = "La Contraseña es obligatorio" });
            }
            if (string.IsNullOrEmpty(oUsuario.ConfirmarClave))
            {
                return Json(new { success = false, message = "La Confirmación de la contraseña es obligatorio" });
            }


            // Validar campos de selección
            if (oUsuario.Id_Rol == 0)
            {
                return Json(new { success = false, message = "El campo Rol es obligatorio" });
            }
            if (oUsuario.Id_Perfil == 0)
            {
                return Json(new { success = false, message = "El campo Perfil es obligatorio" });
            }


            if (oUsuario.Id_Estado == 0)
            {
                return Json(new { success = false, message = "El campo Estado es obligatorio" });
            }


            // Validar el campo de teléfono
            if (string.IsNullOrEmpty(oUsuario.Telefono) || !EsNumero(oUsuario.Telefono) || oUsuario.Telefono.Length != 10)
            {
                return Json(new { success = false, message = "El teléfono debe tener 10 dígitos numéricos" });
            }


            // Validar la fecha de nacimiento (mayores de 18 años)
            if (oUsuario.FechaNacimientoU > DateTime.Today.AddYears(-18))
            {
                return Json(new { success = false, message = "Debe ser mayor de 18 años para registrarse" });
            }


            //Confirmar si la contraseña es la misma
            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                // Actualizar la clave para la encriptación
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                return Json(new { success = false, message = "Las contraseñas no coinciden" });
            }

            //validar si el correo cumple con los parametros de un @gmail.com
            if (!IsValidEmail(oUsuario.Correo))
            {
                return Json(new { success = false, message = "El correo electrónico no es válido" });
            }

            // Variable 
            string almacenes = oUsuario.Almacenes != null ? string.Join(",", oUsuario.Almacenes) : "";

            //validacion del campo almacen
            if (string.IsNullOrEmpty(almacenes))
            {
                return Json(new { success = false, message = "El campo almacen es obligatorio" });
            }

            // Variable 
            string permisos = oUsuario.Permisos != null ? string.Join(",", oUsuario.Permisos) : "";

            //validacion del campo almacen
            if (string.IsNullOrEmpty(permisos))
            {
                return Json(new { success = false, message = "El campo permiso es obligatorio" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuarioPorAdminTotal", cn);
                cmd.Parameters.AddWithValue("NombreU", oUsuario.NombreU);
                cmd.Parameters.AddWithValue("ApellidoU", oUsuario.ApellidoU);
                oUsuario.FechaNacimientoU = fechaNacimientoParsed;// Asegúrate de que la propiedad FechaNacimientoU sea de tipo DateTime
                cmd.Parameters.AddWithValue("FechaNacimientoU", oUsuario.FechaNacimientoU);
                cmd.Parameters.AddWithValue("Telefono", oUsuario.Telefono);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.AddWithValue("Id_Rol", oUsuario.Id_Rol);
                cmd.Parameters.AddWithValue("Id_Perfil", oUsuario.Id_Perfil);
                cmd.Parameters.AddWithValue("Almacenes", almacenes);  // Enviar almacenes como cadena separada por comas
                cmd.Parameters.AddWithValue("Permisos", permisos);  // Enviar almacenes como cadena separada por comas
                cmd.Parameters.AddWithValue("Id_Estado", oUsuario.Id_Estado); // En el HTML estara predeterminado como inactivo pero cambiable
                cmd.Parameters.AddWithValue("FechaRegistroU", DateTime.Now); // Fecha actual para FechaRegistroU

                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (registrado)
                {
                    mensaje = "El usuario ha sido registrado con éxito";
                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("LUsuariosAdminTotal", "AdminPages") });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
        }

        //PROCEDIMIENTO ALM PARA Regristrar los perfiles etiquetas
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult RegistrarPerfil(PerfilModel oPerfil)
        {

            // Validar campos vacíos
            if (string.IsNullOrEmpty(oPerfil.Nombre_Perfil))
            {
                return Json(new { success = false, message = "El Nombre del perfil es obligatorio" });
            }

            // Validar campos de selección
            if (oPerfil.Id_Rol == 0)
            {
                return Json(new { success = false, message = "El campo Rol es obligatorio" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_RegistrarPerfil", cn);
                cmd.Parameters.AddWithValue("Nombre_Perfil", oPerfil.Nombre_Perfil);
                cmd.Parameters.AddWithValue("Id_Rol", oPerfil.Id_Rol);

                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (registrado)
                {
                    mensaje = "El perfil ha sido registrado con éxito";
                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("PerfilesForUsersAdminTotal", "AdminPages") });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR EL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarMiPerfilPersonalAdminTotal(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            if (string.IsNullOrEmpty(ObjUsuario.NombreU))
            {
                return Json(new { success = false, message = "El Nombre es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ApellidoU))
            {
                return Json(new { success = false, message = "El Apellido es obligatorio" });
            }

            DateTime fechaNacimientoParsed;

            // Intenta parsear la fecha de nacimiento
            if (!DateTime.TryParseExact(FechaNacimientoU, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimientoParsed))
            {
                return Json(new { success = false, message = "Formato de fecha de nacimiento incorrecto." });
            }

            // Valida si la fecha de nacimiento está en el rango permitido
            DateTime minDate = new DateTime(1920, 1, 1);
            DateTime maxDate = DateTime.Today;
            if (fechaNacimientoParsed < minDate || fechaNacimientoParsed > maxDate)
            {
                return Json(new { success = false, message = "La fecha de nacimiento no es válida" });
            }

            // Validar el campo de teléfono
            if (string.IsNullOrEmpty(ObjUsuario.Telefono) || !EsNumero(ObjUsuario.Telefono) || ObjUsuario.Telefono.Length != 10)
            {
                return Json(new { success = false, message = "El teléfono debe tener 10 dígitos numéricos" });
            }

            // Validar la fecha de nacimiento (mayores de 18 años)
            if (ObjUsuario.FechaNacimientoU > DateTime.Today.AddYears(-18))
            {
                return Json(new { success = false, message = "Debe ser mayor de 18 años para registrarse" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarMiUsuarioDatosPersonales", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("NombreU", ObjUsuario.NombreU);
                cmd.Parameters.AddWithValue("ApellidoU", ObjUsuario.ApellidoU);
                ObjUsuario.FechaNacimientoU = fechaNacimientoParsed;// Asegúrate de que la propiedad FechaNacimientoU sea de tipo DateTime
                cmd.Parameters.AddWithValue("FechaNacimientoU", ObjUsuario.FechaNacimientoU);
                cmd.Parameters.AddWithValue("Telefono", ObjUsuario.Telefono);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "Tu usuario se ha actualizado";

                    // Verifica si el usuario en sesión es el mismo que el que se está editando
                    UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
                    if (usuarioSesion != null && usuarioSesion.Id_Usuario == ObjUsuario.Id_Usuario)
                    {
                        // Actualizar la sesión con la nueva información
                        Session["NombreUser"] = ObjUsuario.NombreU;
                    }

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("InicioAdminTotal", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR EL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarMiPerfiPermisoslAdminTotal(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            // Validar campos de selección
            if (ObjUsuario.Id_Rol == 0)
            {
                return Json(new { success = false, message = "El campo Rol es obligatorio" });
            }
            if (ObjUsuario.Id_Perfil == 0)
            {
                return Json(new { success = false, message = "El campo Perfil es obligatorio" });
            }

            if (ObjUsuario.Id_Estado == 0)
            {
                return Json(new { success = false, message = "El campo Estado es obligatorio" });
            }

            //validar si el correo cumple con los parametros de un @gmail.com
            if (!IsValidEmail(ObjUsuario.Correo))
            {
                return Json(new { success = false, message = "El correo electrónico no es válido" });
            }

            // Variable 
            string almacenes = string.Join(",", ObjUsuario.Almacenes);

            //validacion del campo almacen
            if (string.IsNullOrEmpty(almacenes))
            {
                return Json(new { success = false, message = "El campo Almacen es obligatorio" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarMiUsuarioConfiguracion", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("Correo", ObjUsuario.Correo);
                cmd.Parameters.AddWithValue("Id_Rol", ObjUsuario.Id_Rol);
                cmd.Parameters.AddWithValue("Id_Perfil", ObjUsuario.Id_Perfil);
                cmd.Parameters.AddWithValue("Almacenes", almacenes);
                cmd.Parameters.AddWithValue("Id_Estado", ObjUsuario.Id_Estado);
                cmd.Parameters.AddWithValue("FechaRegistroU", ObjUsuario.FechaRegistroU);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "Tu Usuario se ha actualizado vuelva a iniciar sesión";

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("Login", "Acceso") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR EL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarUsuario(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            if (string.IsNullOrEmpty(ObjUsuario.NombreU))
            {
                return Json(new { success = false, message = "El Nombre es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ApellidoU))
            {
                return Json(new { success = false, message = "El Apellido es obligatorio" });
            }

            DateTime fechaNacimientoParsed;

            // Intenta parsear la fecha de nacimiento
            if (!DateTime.TryParseExact(FechaNacimientoU, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaNacimientoParsed))
            {
                return Json(new { success = false, message = "Formato de fecha de nacimiento incorrecto." });
            }

            // Valida si la fecha de nacimiento está en el rango permitido
            DateTime minDate = new DateTime(1920, 1, 1);
            DateTime maxDate = DateTime.Today;
            if (fechaNacimientoParsed < minDate || fechaNacimientoParsed > maxDate)
            {
                return Json(new { success = false, message = "La fecha de nacimiento no es válida" });
            }

            // Validar la fecha de nacimiento (mayores de 18 años)
            if (ObjUsuario.FechaNacimientoU > DateTime.Today.AddYears(-18))
            {
                return Json(new { success = false, message = "Debe ser mayor de 18 años para registrarse" });
            }

            if (string.IsNullOrEmpty(ObjUsuario.Correo))
            {
                return Json(new { success = false, message = "El Correo es obligatorio" });
            }

            // Validar el campo de teléfono
            if (string.IsNullOrEmpty(ObjUsuario.Telefono) || !EsNumero(ObjUsuario.Telefono) || ObjUsuario.Telefono.Length != 10)
            {
                return Json(new { success = false, message = "El teléfono debe tener 10 dígitos numéricos" });
            }

            //validar si el correo cumple con los parametros de un @gmail.com
            if (!IsValidEmail(ObjUsuario.Correo))
            {
                return Json(new { success = false, message = "El correo electrónico no es válido" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarUsuarioPorAdminTotal", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("NombreU", ObjUsuario.NombreU);
                cmd.Parameters.AddWithValue("ApellidoU", ObjUsuario.ApellidoU);
                ObjUsuario.FechaNacimientoU = fechaNacimientoParsed;// Asegúrate de que la propiedad FechaNacimientoU sea de tipo DateTime
                cmd.Parameters.AddWithValue("FechaNacimientoU", ObjUsuario.FechaNacimientoU);
                cmd.Parameters.AddWithValue("Telefono", ObjUsuario.Telefono);
                cmd.Parameters.AddWithValue("Correo", ObjUsuario.Correo);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "El usuario se ha actualizado";

                    // Verifica si el usuario en sesión es el mismo que el que se está editando
                    UsuarioModel usuarioSesion = Session["usuario"] as UsuarioModel;
                    if (usuarioSesion != null && usuarioSesion.Id_Usuario == ObjUsuario.Id_Usuario)
                    {
                        // Actualizar la sesión con la nueva información
                        Session["NombreUser"] = ObjUsuario.NombreU;
                    }

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("LUsuariosAdminTotal", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR EL PERMISO DEL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarPermisos(UsuarioModel ObjUsuario, PerfilModel ObjPerfil, RolModel ObjRol, string FechaNacimientoU)
        {

            // Validar campos de selección
            if (ObjUsuario.Id_Rol == 0)
            {
                return Json(new { success = false, message = "El campo Rol es obligatorio" });
            }
            // Validar campos de selección solo si están habilitados o tienen un valor diferente de 0
            if (ObjUsuario.Id_Perfil == 0)
            {
                return Json(new { success = false, message = "El campo Perfil es obligatorio" });
            }


            if (ObjUsuario.Id_Estado == 0)
            {
                return Json(new { success = false, message = "El campo Estado es obligatorio" });
            }

            // Variable 
            string almacenes = ObjUsuario.Almacenes != null ? string.Join(",", ObjUsuario.Almacenes) : "";

            //validacion del campo almacen
            if (string.IsNullOrEmpty(almacenes))
            {
                return Json(new { success = false, message = "El campo Almacen es obligatorio" });
            }

            // Variable 
            string permisos = ObjUsuario.Permisos != null ? string.Join(",", ObjUsuario.Permisos) : "";

            //validacion del campo almacen
            if (string.IsNullOrEmpty(permisos))
            {
                return Json(new { success = false, message = "El campo Permisos es obligatorio" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ActualizarPermisosDelUsuarioPorAdminTotal", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("Id_Rol", ObjUsuario.Id_Rol);
                if (ObjUsuario.Id_Perfil.HasValue)
                {
                    cmd.Parameters.AddWithValue("@Id_Perfil", ObjUsuario.Id_Perfil.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Id_Perfil", DBNull.Value);
                }

                cmd.Parameters.AddWithValue("Almacenes", almacenes);
                cmd.Parameters.AddWithValue("Permisos", permisos);
                cmd.Parameters.AddWithValue("Id_Estado", ObjUsuario.Id_Estado);
                cmd.Parameters.Add("Actualizado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool actualizado = Convert.ToBoolean(cmd.Parameters["Actualizado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (actualizado)
                {
                    mensaje = "El usuario se ha actualizado";

                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("LUsuariosAdminTotal", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR LA CONTRASEÑA DEL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarContraseña(UsuarioModel ObjUsuario)
        {

            if (string.IsNullOrEmpty(ObjUsuario.Clave))
            {
                return Json(new { success = false, message = "La Contraseña es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ConfirmarClave))
            {
                return Json(new { success = false, message = "La Confirmación de la contraseña es obligatorio" });
            }

            //Confirmar que la clave se actualice en encryptado
            //Confirmar si la contraseña es la misma
            if (ObjUsuario.Clave == ObjUsuario.ConfirmarClave)
            {
                // Actualizar la clave para la encriptación
                ObjUsuario.Clave = ConvertirSha256(ObjUsuario.Clave);
            }
            else
            {
                return Json(new { success = false, message = "Las contraseñas no coinciden" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_editarcontraseña", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("Clave", ObjUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (registrado)
                {
                    mensaje = "La contraseña se ha actualizado";
                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("LUsuariosAdminTotal", "AdminPages") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }

            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR EL PERFIL ETIQUETA
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarPerfil(PerfilModel ObjPerfil)
        {
            //validarCamposVacios
            if (string.IsNullOrEmpty(ObjPerfil.Nombre_Perfil))
            {
                return Json(new { success = false, message = "El Nombre del perfil es obligatorio" });
            }

            // Validar campos de selección
            if (ObjPerfil.Id_Rol == 0)
            {
                return Json(new { success = false, message = "El campo Rol es obligatorio" });
            }

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("sp_EditarPerfil", cn);
                cmd.Parameters.AddWithValue("Id_Perfil", ObjPerfil.Id_Perfil);
                cmd.Parameters.AddWithValue("Nombre_Perfil", ObjPerfil.Nombre_Perfil);
                cmd.Parameters.AddWithValue("Id_Rol", ObjPerfil.Id_Rol);

                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (registrado)
                {
                    mensaje = "El perfil ha sido actualizado con éxito";
                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("PerfilesForUsersAdminTotal", "AdminPages") });
                }
                else
                {
                    return Json(new { success = false, message = mensaje });
                }
            }
        }

        //PROCEDIMIENTO ALM PARA EDITAR LA CONTRASEÑA DEL USUARIO ACTUAL
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EditarMiContraseña(UsuarioModel ObjUsuario)
        {
            if (string.IsNullOrEmpty(ObjUsuario.Clave))
            {
                return Json(new { success = false, message = "La Contraseña es obligatorio" });
            }
            if (string.IsNullOrEmpty(ObjUsuario.ConfirmarClave))
            {
                return Json(new { success = false, message = "La Confirmación de la contraseña es obligatorio" });
            }

            // Validación de la contraseña actual
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_ObtenerClaveActual", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.Add("@ClaveActualOutput", SqlDbType.NVarChar, 256).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();

                string claveActualBD = cmd.Parameters["@ClaveActualOutput"].Value.ToString();

                if (string.IsNullOrEmpty(claveActualBD) || ConvertirSha256(ObjUsuario.ClaveActual) != claveActualBD)
                {
                    return Json(new { success = false, message = "La contraseña actual es incorrecta" });
                }
            }

            if (ObjUsuario.Clave != ObjUsuario.ConfirmarClave)
            {
                return Json(new { success = false, message = "Las contraseñas no coinciden" });
            }

            ObjUsuario.Clave = ConvertirSha256(ObjUsuario.Clave); // Convertir nueva clave a SHA256

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("sp_editarcontraseña", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", ObjUsuario.Id_Usuario);
                cmd.Parameters.AddWithValue("Clave", ObjUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                bool registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                string mensaje = cmd.Parameters["Mensaje"].Value.ToString();

                if (registrado)
                {
                    mensaje = "La contraseña se ha actualizado vuelva a iniciar sesión";
                    return Json(new { success = true, message = mensaje, redirectToUrl = Url.Action("CerrarSesion", "Acceso") });
                }
                else
                {
                    mensaje = "Error (verificar)";
                    return Json(new { success = false, message = mensaje });
                }
            }
        }


        //PROCEDIMIENTO ALM PARA ELIMINAR EL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EliminarUsuario(int IdUsuario)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarUsuarioWebPorAdmin", cn);
                cmd.Parameters.AddWithValue("Id_Usuario", IdUsuario);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("LUsuariosAdminTotal", "AdminPages");
        }


        //PROCEDIMIENTO ALM PARA ELIMINAR EL USUARIO
        [HttpPost]
        [ValidarSesion(idRol: 3)]
        public ActionResult EliminarPerfil(int IdPerfil)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarPerfil", cn);
                cmd.Parameters.AddWithValue("Id_Perfil", IdPerfil);
                cmd.CommandType = CommandType.StoredProcedure;
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("PerfilesForUsersAdminTotal", "AdminPages");
        }

        //Para validar el telefono que solo tega 10 digitos numericos
        private bool EsNumero(string valor)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(valor, @"^\d{10}$");
        }


        //para darle validacion al registro de correo que sea por estos formatos "@gmail.com"
        private bool IsValidEmail(string correo)
        {
            string regexPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
            return Regex.IsMatch(correo, regexPattern);
        }

        //Para convertir la contraseña encryotada
        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("X2"));
            }
            return Sb.ToString();

        }


        //PARA OBTENER LOS LAS SELECT DEL AGREGAR USUARIO(Llamado en el script)

        public ActionResult ObtenerRoles(int idRoles)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("select * from rolesweb where Id_Rol = @idRoles", cn);
                cmd.Parameters.AddWithValue("@idRoles", idRoles);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var ListaRolesView = new List<RolModel>();
                    while (dr.Read())
                    {
                        RolModel ViewRol = new RolModel();
                        ViewRol.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);
                        ViewRol.Nombre_Rol = dr["Nombre_Rol"].ToString();

                        ListaRolesView.Add(ViewRol);
                    }

                    return Json(ListaRolesView, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult ObtenerPerfiles(int idRol)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("select * from perfilesUserWeb where Id_Rol = @idRol", cn);
                cmd.Parameters.AddWithValue("@idRol", idRol);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var ListaPerfilView = new List<PerfilModel>();
                    while (dr.Read())
                    {
                        PerfilModel ViewPerfil = new PerfilModel();
                        ViewPerfil.Id_Perfil = Convert.ToInt32(dr["Id_Perfil"]);
                        ViewPerfil.Nombre_Perfil = dr["Nombre_Perfil"].ToString();
                        ViewPerfil.Id_Rol = Convert.ToInt32(dr["Id_Rol"]);

                        ListaPerfilView.Add(ViewPerfil);
                    }

                    return Json(ListaPerfilView, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult ObtenerAlmacenes()
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("select * from almacenes where nombreserver <> 'N'", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var ListaAlmacenView = new List<almacenesModel>();
                    while (dr.Read())
                    {
                        almacenesModel ViewAlmacen = new almacenesModel();
                        ViewAlmacen.Id_Almacen = Convert.ToInt32(dr["Id_Almacen"]);
                        ViewAlmacen.Almacen = dr["Almacen"].ToString(); //Id por sucursal
                        ViewAlmacen.Nombre = dr["Nombre"].ToString();
                        ViewAlmacen.nombreserver = dr["nombreserver"].ToString(); //para poder excluir los que tengan N

                        ListaAlmacenView.Add(ViewAlmacen);
                    }

                    return Json(ListaAlmacenView, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult ObtenerPermisos()
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("select * from permisosweb", cn);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var ListaPermisoView = new List<PermisosModel>();
                    while (dr.Read())
                    {
                        PermisosModel ViewPermiso = new PermisosModel();
                        ViewPermiso.Id_Permiso = Convert.ToInt32(dr["Id_Permiso"]);
                        ViewPermiso.Nombre_Permiso = dr["Nombre_Permiso"].ToString();

                        ListaPermisoView.Add(ViewPermiso);
                    }

                    return Json(ListaPermisoView, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult ObtenerEstados(int idEstado)
        {
            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("select * from estadoweb where Id_Estado = @idEstado", cn);
                cmd.Parameters.AddWithValue("@idEstado", idEstado);
                cmd.CommandType = CommandType.Text;
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    var ListaEstadoView = new List<EstadoModel>();
                    while (dr.Read())
                    {
                        EstadoModel ViewEstado = new EstadoModel();
                        ViewEstado.Id_Estado = Convert.ToInt32(dr["Id_Estado"]);
                        ViewEstado.Nombre_Estado = dr["Nombre_Estado"].ToString();

                        ListaEstadoView.Add(ViewEstado);
                    }

                    return Json(ListaEstadoView, JsonRequestBehavior.AllowGet);
                }
            }
        }

        //funcion para el admin, revisar
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VReporteRegistroEfectivoORI(string IdMoneda)
        {

            ViewBag.ActivePage = "VValoresEntregados";

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
                    almacenes.Add(new Tuple<int, string>(Convert.ToInt32(reader["AlmacenId"]), almacenNombre));
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

            DateTime Fecha = DateTime.Now;

            ViewBag.Fecha = Fecha.ToString("yy-MM-dd");






            var valoresEntregados = new List<ValoresEntregados>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.ValoresEntregados WHERE FechaInsercion=@Fecha", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));

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
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {

                        registroValoresEntregados.Add(new RegistroValoresEntregados
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yy-MM-dd"),
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
            ViewBag.Almacen = 211;

            ViewBag.IdMoneda = IdMoneda;
            //return RedirectToRoute("usuario-reporte-registro-efectivo");

            return View("/Views/AdminPages/VValoresEntregados.cshtml");
            //falta traer el almacen de la sesion

        }

        // rutas de supervisores tiendas

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
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

            return View("/Views/AdminPages/VCuadreEfectivo.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
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


            return View("/Views/AdminPages/VCuadreVoucher.cshtml");

        }

        [HttpPost]
        [ValidarSesion(idRol: 1, permisos: 1)]
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
        [ValidarSesion(idRol: 1, permisos: 1)]
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
        [ValidarSesion(idRol: 1, permisos: 1)]
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


            return RedirectToRoute("admin-caja-cuadre-voucher");

        }


        //[HttpPost]
        //[ValidarSesion(idRol: 3, permisos: 1)]
        //public ActionResult ActualizarCuadreMonedas()
        //{
        //    string Fecha = "";
        //    int IdAlmacen = 0;
        //    float Recepcion = 0;
        //    int IdMoneda = 0;
        //    int IdSummary = 0;

        //    foreach (string key in Request.Form.AllKeys)
        //    {
        //        if (key.StartsWith("fecha"))
        //        {
        //            Fecha = Request.Form[key];
        //        }
        //        if (key.StartsWith("idAlmacen"))
        //        {
        //            IdAlmacen = Convert.ToInt32(Request.Form[key]);
        //        }
        //        if (key.StartsWith("recepcion"))
        //        {
        //            Recepcion = Convert.ToSingle(Request.Form[key]);
        //        }
        //        if (key.StartsWith("idMoneda"))
        //        {
        //            IdMoneda = Convert.ToInt32(Request.Form[key]);
        //        }
        //        if (key.StartsWith("idSummary"))
        //        {
        //            IdSummary = Convert.ToInt32(Request.Form[key]);
        //        }
        //    }

        //    DateTime FechaParsed;

        //    if (!DateTime.TryParseExact(Fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out FechaParsed))
        //    {
        //        return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
        //    }

        //    using (SqlConnection cn = new SqlConnection(conexionDBHoka))
        //    {
        //        cn.Open();

        //        SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ActualizarRecepcionMoneda", cn);

        //        cmd.Parameters.AddWithValue("Fecha", FechaParsed.ToString("yyyy-MM-dd"));
        //        cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
        //        cmd.Parameters.AddWithValue("Recepcion", Recepcion);
        //        cmd.Parameters.AddWithValue("IdMoneda", IdMoneda);
        //        cmd.Parameters.AddWithValue("IdSummary", IdSummary);

        //        cmd.CommandType = CommandType.StoredProcedure;

        //        cmd.ExecuteNonQuery();
        //    }


        //    return RedirectToRoute("super-caja-cuadre-efectivo");

        //}

        [HttpPost]
        [ValidarSesion(idRol: 1, permisos: 1)]
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


            return RedirectToRoute("admin-caja-cuadre-efectivo");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VGestionarEfectivo()
        {

            ViewBag.ActivePage = "VGestionarEfectivo";

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

            return View("/Views/AdminPages/VGestionarEfectivo.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VGestionarVouchers()
        {

            ViewBag.ActivePage = "VGestionarVouchers";

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

            return View("/Views/AdminPages/VGestionarVouchers.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFecha(string IdAlmacen)
        {

            ViewBag.ActivePage = "VGestionarEfectivo";

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


            return View("/Views/AdminPages/VCajaAlmacenesFecha.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaVoucher(string IdAlmacen)
        {

            ViewBag.ActivePage = "VGestionarVouchers";

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

                string baseQuery = @"SELECT distinct Fecha FROM ingresos.dbo.RegistroDiaVoucher WHERE IdAlmacen = @IdAlmacen group by Fecha";
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


            return View("/Views/AdminPages/VCajaAlmacenesFechaVoucher.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMoneda(string IdAlmacen, string Fecha)
        {

            ViewBag.ActivePage = "VGestionarEfectivo";

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

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMonedaVoucher(string IdAlmacen, string Fecha)
        {

            ViewBag.ActivePage = "VGestionarVouchers";

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


            var vouchers = new List<Vouchers>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM ingresos.dbo.Vouchers", cn);
                using (SqlDataReader reader = cmdRemisioPago.ExecuteReader())
                {
                    while (reader.Read())
                    {


                        vouchers.Add(new Vouchers
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString()
                        });

                    }
                }
            }

            ViewBag.Vouchers = vouchers;
            ViewBag.Fecha = Fecha;
            ViewBag.IdAlmacen = IdAlmacen;


            return View();

        }

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMonedaEfectivo(string IdAlmacen, string Fecha, string IdMoneda)
        {

            ViewBag.ActivePage = "VGestionarEfectivo";

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
                            FechaInsercion = Convert.ToDateTime(reader["FechaInsercion"]).ToString("yyyy-MM-dd"),
                            IdRegistroValoresEntregados = Convert.ToInt32(reader["IdRegistroValoresEntregados"])
                        });

                    }
                }
            }

            var registroValoresEntregados = new List<RegistroValoresEntregados>();
            int IdRegistroValoresEntregados = 0;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.RegistroValoresEntregados WHERE Fecha=@Fecha", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        IdRegistroValoresEntregados = Convert.ToInt32(reader["Id"]);

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
            ViewBag.IdRegistroValoresEntregados = IdRegistroValoresEntregados;
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

        //
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMonedaEfectivoVoucher(string IdAlmacen, string Fecha, string IdVoucher)
        {

            ViewBag.ActivePage = "VGestionarVouchers";

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

            var vouchers = new List<Vouchers>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM ingresos.dbo.Vouchers", cn);
                using (SqlDataReader reader = cmdRemisioPago.ExecuteReader())
                {
                    while (reader.Read())
                    {


                        vouchers.Add(new Vouchers
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString()
                        });

                    }
                }
            }

            ViewBag.Vouchers = vouchers;

            //DateTime Fecha = DateTime.Now;



            var registroVouchers = new List<RegistroVouchers>();

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.RegistroVouchers WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {

                        registroVouchers.Add(new RegistroVouchers
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            Importe = Convert.ToInt32(reader["Importe"]),

                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            IdVoucher = Convert.ToInt32(reader["IdVoucher"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            IdRegistroDiaVoucher = Convert.ToInt32(reader["IdRegistroDiaVoucher"]),
                            Referencia = reader["Referencia"].ToString()
                        });

                    }
                }
            }

            var registroDiaVoucher = new List<RegistroDiaVoucher>();

            int IdRegistroDiaVoucher = 0;

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM ingresos.dbo.RegistroDiaVoucher WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha);
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        IdRegistroDiaVoucher = Convert.ToInt32(reader["Id"]);

                        registroDiaVoucher.Add(new RegistroDiaVoucher
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            IdVoucher = Convert.ToInt32(reader["IdVoucher"]),
                        });

                    }
                }
            }

            //Response.Write(Fecha);

            //var json1 = new JavaScriptSerializer().Serialize(registroVouchers);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //return null;

            //List<MonedaDenominacion> monedaDenominacion = new List<MonedaDenominacion>();
            //MonedaDenominacion monedaDenominacion = new MonedaDenominacion();


            ViewBag.RegistroVouchers = registroVouchers;
            ViewBag.RegistroDiaVoucher = registroDiaVoucher;
            ViewBag.Almacen = IdAlmacen;
            ViewBag.IdRegistroDiaVoucher = IdRegistroDiaVoucher;
            //Response.Write(IdVoucher);
            //return null;

            ViewBag.IdVoucher = Convert.ToInt32(IdVoucher);

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

            //return null;
            return View();

        }
        //

        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VReporteRegistroVentas()
        {

            ViewBag.ActivePage = "VReporteRegistroVentas";

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

            return View("/Views/AdminPages/VReporteRegistroVentas.cshtml");

        }

        [HttpPost]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult getSalesByCategories()
        {
            string fecha = "";
            int idAlmacen = 0;

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("fecha"))
                {
                    fecha = Request.Form[key];
                }
                if (key.StartsWith("idAlmacen"))
                {
                    idAlmacen = Convert.ToInt32(Request.Form[key]);
                }
            }

            DateTime fechaParsed;

            if (!DateTime.TryParseExact(fecha, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
            {
                return Json(new { success = false, message = "Error de formato de fecha" }, JsonRequestBehavior.AllowGet);
            }

            //Convert.ToDateTime(reader["FechaInsercion"]).ToString("yy-MM-dd"),

            var ventasCategoria = new List<VentasCategoria>();

            //var json1 = new JavaScriptSerializer().Serialize(fechaParsed.ToString("yyyy-MM-dd"));
            //Response.Write("yourObject:" + json1 + "<br/>");



            //return null;


            if (idAlmacen > 0)
            {

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
                                VentaTienda = reader["VentaTienda"] != DBNull.Value ? Convert.ToInt32(reader["VentaTienda"]) : 0,
                                Comisiones = Convert.ToSingle(reader["Comisiones"]),
                                NetoVenta = Convert.ToSingle(reader["NetoVenta"]),
                                Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                                Estado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0,

                            });

                        }
                    }
                }
            }
            var jsonResult = Json(ventasCategoria, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
            return jsonResult;

        }


    }
}
