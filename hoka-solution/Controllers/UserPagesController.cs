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
using System.Globalization;
using hoka.Permisos;
using System.Web.WebPages;
using System.Web.Script.Serialization;
using hoka_cli.Context.Conexiones;

namespace hoka.Controllers
{
    public class UserPagesController : Controller
    {
        private string _conexionHokaCompuadmo;
        private string _conexionHokaIngresos;
        private string _conexionHokaJoyeria;

        // NOTA???: ¿SIRVE?
        private static List<UsuarioModel> ELIMINAR_ListaUsers = new List<UsuarioModel>();
        private static List<RemisioDModel> ELIMINAR_ListaRemisioD = new List<RemisioDModel>();
        private static List<RolModel> ELIMINAR_ListaRoles = new List<RolModel>();
        private static List<EstadoModel> ELIMINAR_ListaEstados = new List<EstadoModel>();

        public UserPagesController()
        {
            bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();
            _conexionHokaCompuadmo = SvConexionHokaCompuadmoConsultar.Consultar(conexionDBDEV);
            _conexionHokaIngresos = SvConexionHokaIngresosConsultar.Consultar(conexionDBDEV);
            _conexionHokaJoyeria = SvConexionHokaJoyeriaConsultar.Consultar(conexionDBDEV);
        }

        [ValidarSesion(idRol: 3)]
        public ActionResult Index()
        {
            return View();
        }

        [ValidarSesion(idRol: 2)]
        public ActionResult InicioUser()
        {

            ViewBag.ActivePage = "InicioUser";

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

        //CONFIGURACION DEL PERFIL DEL USUARIO        
        [HttpGet]
        [ValidarSesion(idRol: 2)]
        public ActionResult MiPerfilUser()
        {
            ViewBag.ActivePage = "MiPerfilUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

        //PAGINA DE REPORTE DE VENTA POR PRODUCTO
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VProductosUser()
        {
            ViewBag.ActivePage = "VProductosUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
        [ValidarSesion(idRol: 2, permisos: 2)]
        public ActionResult VAlmacenUser()
        {

            ViewBag.ActivePage = "VAlmacenUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

        //PAGINA DE REPORTE DE VENTA POR VENDEDOR
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 3)]
        public ActionResult VVendedorUser()
        {

            ViewBag.ActivePage = "VVendedorUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

        //PAGINA DE REPORTE DE VENTAS POR PRODUCTO JOYERIA (FILTROS)
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 4)]
        public ActionResult VProductosJoyUser()
        {

            ViewBag.ActivePage = "VProductosJoyUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 5)]
        public ActionResult GProductosUser()
        {

            ViewBag.ActivePage = "GProductosUser";

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

        //PAGINA DE REPORTE DE VENTA POR TIPO DE PAGO
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 7)]
        public ActionResult VTipoPagoUser()
        {
            ViewBag.ActivePage = "VTipoPagoUser";


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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

        //BOTON DESPEGABLE DE CONSULTAS
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 8)]
        public ActionResult CTicketsUser()
        {
            ViewBag.ActivePage = "CTicketsUser";


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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
        [ValidarSesion(idRol: 2, permisos: 8)]
        public ActionResult DetalleTicketUser(string idFolio)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsUser";

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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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


        //BOTON DESPEGABLE DE CONSULTAS PARA JOYERIA

        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 9)]
        public ActionResult CTicketsJoyUser()
        {
            ViewBag.ActivePage = "CTicketsJoyUser";


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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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


        //VISTA SABER LOS DETALLES DEL TICKET DE LA VISTA CTicketsUser
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 9)]
        public ActionResult DetalleTicketJoyUser(string idFolio, string almacen)
        {

            ViewBag.idFolio = idFolio;
            ViewBag.Title = "Ticket: " + ViewBag.idFolio;
            ViewBag.ActivePage = "CTicketsJoyUser";

            if (Session["usuario"] == null) // Si no hay usuario autenticado
            {
                return RedirectToAction("Login", "Acceso"); // Redirige a la página de inicio de sesión
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            if (idFolio == null)
                return RedirectToAction("DetalleTicketJoyUser", "UserPages");

            using (SqlConnection cn = new SqlConnection(_conexionHokaJoyeria))
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
                        SqlCommand cmdRemisioMObser = new SqlCommand("SELECT * FROM VRemisioMWebJoy WHERE folio_factura = @folio and almacen = @almacen", cn);
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
        [ValidarSesion(idRol: 2, permisos: 12)]
        public ActionResult CRotacionPUser()
        {
            ViewBag.ActivePage = "CRotacionPUser";


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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
        [ValidarSesion(idRol: 2, permisos: 13)]
        public ActionResult CRotacionPJoyUser()
        {
            ViewBag.ActivePage = "CRotacionPJoyUser";


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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

        [HttpPost]
        public ActionResult procesarValoresEntregados()
        {

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            //List<Tuple<string, string,string>> data = new List<Tuple<string, string, string>>();
            List<string> denominacionList = new List<string>();
            List<string> cantidadList = new List<string>();
            List<string> importeList = new List<string>();

            int index = 0;

            double TipoCambio = 0;
            int IdMoneda = 0;
            int IdAlmacen = 0;

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
            }

            Response.Write("index" + index);
            Response.Write(TipoCambio);

            List<Tuple<int>> almacenes = new List<Tuple<int>>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }


            //return null;

            DateTime Fecha = DateTime.Now;
            Fecha.ToString("yyyy-MM-dd");

            bool IsValid = false;
            int IdRegistroValoresEntregados = 0;

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
                //Response.Write("insertar");

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

                        using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
            //return RedirectToRoute("usuario-reporte-registro-efectivo");
            return RedirectToRoute("usuario-reporte-registro-efectivo-monedas");
            //return Json(new { success = false, message = "asd" }, JsonRequestBehavior.AllowGet);
        }

        // nuevo contenido

        /// <summary>
        /// Usuario:    hugouser@gmail.com
        /// Vista:      usuario/caja-reporte-ventas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VReporteRegistroVentas()
        {
            ViewBag.ActivePage = "VReporteRegistroVentas";

            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];
            // Redirige al usuario a la página de inicio de sesión si no está autenticado
            if (usuarioSesion == null)
                return RedirectToAction("Login", "Acceso");

            List<Tuple<int>> almacenes = new List<Tuple<int>>();
            var IdAlmacen = 0;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();


            ViewBag.Almacenes = almacenes;

            DateTime FechaHoy = DateTime.Now;
            bool ExistingRecord = false;

            int RegistroVentaCategoriasId = 0;
            int RegistroVentaCategoriasEstado = 0;

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();

                //SqlCommand cmd = new SqlCommand("SELECT Id FROM hoka_ingresos.dbo.RegistroVentasCategorias WHERE Fecha = @Fecha AND IdAlmacen IN (select [value] from string_split(@IdsAlmacen, ','))", cn);
                SqlCommand cmd = new SqlCommand("SELECT Id,Estado FROM hoka_ingresos.dbo.RegistroVentasCategorias WHERE Fecha = @Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaHoy.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExistingRecord = true;
                        RegistroVentaCategoriasId = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0;
                        RegistroVentaCategoriasEstado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0;

                    }
                }
            }

            ViewBag.ExistingRecord = ExistingRecord;

            if (ExistingRecord)
            {
                //fecha fancy
                return VReporteRegistroVentasUpdate(FechaHoy.ToString("dd/MM/yyyy"), IdAlmacen, RegistroVentaCategoriasEstado, RegistroVentaCategoriasId);
            }
            else
            {
                return VReporteRegistroVentasCreate();
            }

        }

        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VReporteRegistroVentasCreate()
        {

            ViewBag.ActivePage = "VReporteRegistroVentas";

            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];
            if (usuarioSesion == null)
            {
                // Redirige al usuario a la página de inicio de sesión si no está autenticado
                return RedirectToAction("Login", "Acceso");
            }

            List<Tuple<int>> almacenes = new List<Tuple<int>>();
            var IdAlmacen = 0;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            //List<int> almacenes = new List<int>();
            //string idsC = "";

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
            //    cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cn.Open();

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        var almacenId = Convert.ToInt32(reader["AlmacenId"]);
            //        idsC = idsC + almacenId + ",";
            //    }
            //}

            //idsC = idsC.TrimEnd(',');

            //Response.Write(idsC);

            //var json1 = new JavaScriptSerializer().Serialize(idsC);
            //Response.Write("yourObject:" + json1 + "<br/>");

            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Almacenes = almacenes;

            DateTime FechaHoy = DateTime.Now;
            bool ExistingRecord = false;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();

                //SqlCommand cmd = new SqlCommand("SELECT Id FROM hoka_ingresos.dbo.RegistroVentasCategorias WHERE Fecha = @Fecha AND IdAlmacen IN (select [value] from string_split(@IdsAlmacen, ','))", cn);
                SqlCommand cmd = new SqlCommand("SELECT Id FROM hoka_ingresos.dbo.RegistroVentasCategorias WHERE Fecha = @Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaHoy.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExistingRecord = true;
                    }
                }
            }

            ViewBag.ExistingRecord = ExistingRecord;

            var ventasCategoria = new List<VentasCategoria>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {

                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.VentasCategorias WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaHoy.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

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


            //VReporteRegistroVentasUpdate();




            return View("/Views/UserPages/VReporteRegistroVentasCreate.cshtml");

        }


        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VReporteRegistroVentasUpdate(string Fecha, int IdAlmacen, int RegistroVentaCategoriasEstado, int RegistroVentaCategoriasId)
        {

            ViewBag.ActivePage = "VReporteRegistroVentas";


            // Almacenes se pasa al ViewBag para ser usado en la vista
            ViewBag.Fecha = Fecha;

            ViewBag.IdAlmacen = IdAlmacen;
            ViewBag.RegistroVentaCategoriasEstado = RegistroVentaCategoriasEstado;
            ViewBag.RegistroVentaCategoriasId = RegistroVentaCategoriasId;

            return View("/Views/UserPages/VReporteRegistroVentasUpdate.cshtml");

        }

        [ValidarSesion(idRol: 2, permisos: 1)]
        [HttpGet]
        public ActionResult VCajaReporteEfectivoMoneda()
        {
            ViewBag.ActivePage = "VCajaReporteEfectivoMonedas";

            var monedas = new List<Monedas>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Monedas", cn);
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

            return View("/Views/UserPages/VCajaReporteEfectivoMonedas.cshtml");
        }

        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VReporteRegistroEfectivo(string IdMoneda)
        {

            ViewBag.ActivePage = "VCajaReporteEfectivoMonedas";


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

            List<Tuple<int>> almacenes = new List<Tuple<int>>();
            int IdAlmacen = 0;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error.";
                return View("/Views/UserPages/UserError.cshtml");
            }

            ///

            var monedas = new List<Monedas>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Monedas", cn);
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

            ViewBag.Fecha = Fecha.ToString("yyyy-MM-dd");






            var valoresEntregados = new List<ValoresEntregados>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.ValoresEntregados WHERE FechaInsercion=@Fecha", cn);
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
                            FechaInsercion = Convert.ToDateTime(reader["FechaInsercion"]).ToString("yyyy-MM-dd"),
                            IdRegistroValoresEntregados = Convert.ToInt32(reader["IdRegistroValoresEntregados"])
                        });

                    }
                }
            }

            var registroValoresEntregados = new List<RegistroValoresEntregados>();

            int IdRegistroValoresEntregados = 0;
            int RegistroValoresEntregadosEstado = 0;

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.RegistroValoresEntregados WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", Fecha.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);


                using (SqlDataReader reader = cmd.ExecuteReader())
                {


                    while (reader.Read())
                    {
                        IdRegistroValoresEntregados = Convert.ToInt32(reader["Id"]);
                        RegistroValoresEntregadosEstado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0;

                        registroValoresEntregados.Add(new RegistroValoresEntregados
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            IdMoneda = Convert.ToInt32(reader["IdMoneda"]),
                            TipoCambio = Convert.ToSingle(reader["TipoCambio"]),
                            Estado = reader["Estado"] != DBNull.Value ? Convert.ToInt32(reader["Estado"]) : 0,
                        });

                    }
                }
            }

            ViewBag.RegistroValoresEntregadosEstado = RegistroValoresEntregadosEstado;
            ViewBag.IdRegistroValoresEntregados = IdRegistroValoresEntregados;

            Response.Write(IdRegistroValoresEntregados);

            //var json1 = new JavaScriptSerializer().Serialize(valoresEntregados);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //return null;

            //List<MonedaDenominacion> monedaDenominacion = new List<MonedaDenominacion>();
            //MonedaDenominacion monedaDenominacion = new MonedaDenominacion();
            var monedaDenominacion = new List<MonedaDenominacion>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.MonedaDenominacion", cn);
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

            ViewBag.IdMoneda = IdMoneda;
            //return RedirectToRoute("usuario-reporte-registro-efectivo");

            if (IdRegistroValoresEntregados > 0)
            {
                //return View("/Views/UserPages/VCajaAlmacenesFechaMonedaEfectivo.cshtml");
                return View("/Views/UserPages/VValoresEntregadosUpdate.cshtml");

            }
            else
            {
                return View("/Views/UserPages/VValoresEntregadosCreate.cshtml");
            }

            //falta traer el almacen de la sesion

        }

        [HttpPost]
        public ActionResult procesarVentasEntregadas()
        {

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            List<string> categoriasList = new List<string>();
            List<string> ventasList = new List<string>();
            List<string> comisionesList = new List<string>();
            List<string> netoVentaList = new List<string>();
            List<string> ventaTiendaList = new List<string>();

            int index = 0;
            int Estado = 0;

            foreach (string key in Request.Form.AllKeys)
            {
                if (key.StartsWith("categorias"))
                {
                    categoriasList.Add(Request.Form[key]);
                    index++;
                }
                if (key.StartsWith("ventas"))
                {
                    ventasList.Add(Request.Form[key]);
                }
                if (key.StartsWith("comisiones"))
                {
                    comisionesList.Add(Request.Form[key]);
                }
                if (key.StartsWith("netoventa"))
                {
                    netoVentaList.Add(Request.Form[key]);
                }
                if (key.StartsWith("ventatienda"))
                {
                    ventaTiendaList.Add(Request.Form[key]);
                }
                if (key.StartsWith("estado"))
                {
                    Estado = Convert.ToInt32(Request.Form[key]);
                }
            }

            //Response.Write("index" + index);

            // obtener IdAlmacen de la sesion
            List<Tuple<int>> almacenes = new List<Tuple<int>>();
            int IdAlmacen = 0;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }

            //Response.Write(usuarioSesion.Id_Usuario);
            //Response.Write("asd");
            //Response.Write(almacenes.Count());
            //Response.Write("asd");
            //var json1 = new JavaScriptSerializer().Serialize(almacenes);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //return null;

            DateTime Fecha = DateTime.Now;

            bool IsValid = false;
            int IdRegistroVentas = 0;

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioVenta", cn);

                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("Fecha", Fecha.ToString("yyyy-MM-dd"));

                cmd.Parameters.Add("IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("IdInsertadoRegistroVentasCategorias", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                IsValid = Convert.ToBoolean(cmd.Parameters["IsValid"].Value);
                IdRegistroVentas = Convert.ToInt32(cmd.Parameters["IdInsertadoRegistroVentasCategorias"].Value);

            }

            if (IsValid && almacenes.Count() == 1)
            {
                //Response.Write("insertar");

                for (int i = 0; i < index; i++)
                {

                    //Response.Write("for");
                    //Response.Write(Fecha);

                    using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
                    {

                        cn.Open();

                        SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_RegistrarVentas", cn);
                        cmd.Parameters.AddWithValue("IdRegistroVentas", IdRegistroVentas);

                        cmd.Parameters.AddWithValue("Categoria", categoriasList[i]);
                        cmd.Parameters.AddWithValue("Venta", ventasList[i]);
                        cmd.Parameters.AddWithValue("Comisiones", comisionesList[i]);
                        cmd.Parameters.AddWithValue("NetoVenta", netoVentaList[i]);
                        cmd.Parameters.AddWithValue("VentaTienda", ventaTiendaList[i]);
                        cmd.Parameters.AddWithValue("Fecha", Fecha.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                        cmd.Parameters.AddWithValue("Estado", Estado);

                        cmd.Parameters.Add("Insertado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();


                    }

                }

            }

            //return null;

            //return RedirectToAction("VReporteRegistroVentas", "AdminPages");
            return RedirectToRoute("usuario-reporte-registro-ventas");
            //return View("/Views/UserPages/VReporteRegistroVentas.cshtml");


        }

        // vouchers
        // nuevo contenido
        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VVouchers()
        {

            ViewBag.ActivePage = "VVouchers";

            var vouchers = new List<Vouchers>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Vouchers", cn);
                using (SqlDataReader reader = cmd.ExecuteReader())
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

            return View("/Views/UserPages/VVouchers.cshtml");

        }

        [HttpGet]
        [ValidarSesion(idRol: 2, permisos: 1)]
        public ActionResult VVouchersCaptura(string IdVoucher)
        {

            ViewBag.ActivePage = "VVouchers";

            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

            //int IdAlmacen = 0;
            //List<Tuple<int>> almacenes = new List<Tuple<int>>();


            //string idsC = "";

            //using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            //{
            //    SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
            //    cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
            //    cmd.CommandType = CommandType.StoredProcedure;

            //    cn.Open();

            //    SqlDataReader reader = cmd.ExecuteReader();

            //    while (reader.Read())
            //    {
            //        var almacenId = Convert.ToInt32(reader["AlmacenId"]);
            //        idsC = idsC + almacenId + ",";
            //    }
            //}

            //idsC = idsC.TrimEnd(',');

            List<Tuple<int>> almacenes = new List<Tuple<int>>();
            int IdAlmacen = 0;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }


            var vouchers = new List<Vouchers>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Vouchers", cn);
                using (SqlDataReader reader = cmd.ExecuteReader())
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
            ViewBag.IdVoucher = IdVoucher;

            DateTime FechaHoy = DateTime.Now;
            bool ExistingRecord = false;
            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("SELECT Id FROM hoka_ingresos.dbo.RegistroDiaVoucher WHERE Fecha = @Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaHoy.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ExistingRecord = true;
                    }
                }
            }

            ViewBag.ExistingRecord = ExistingRecord;

            var registroVouchers = new List<RegistroVouchers>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.RegistroVouchers WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
                cmd.Parameters.AddWithValue("@Fecha", FechaHoy.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@IdAlmacen", IdAlmacen);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    while (reader.Read())
                    {

                        registroVouchers.Add(new RegistroVouchers
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Importe = Convert.ToSingle(reader["Importe"]),
                            IdAlmacen = Convert.ToInt32(reader["IdAlmacen"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            IdRegistroDiaVoucher = Convert.ToInt32(reader["IdRegistroDiaVoucher"]),
                            IdVoucher = Convert.ToInt32(reader["IdVoucher"]),
                            Cantidad = Convert.ToInt32(reader["Cantidad"]),
                            Referencia = reader["Referencia"].ToString()

                        });

                    }
                }
            }

            ViewBag.RegistroVouchers = registroVouchers;

            if (ExistingRecord)
            {
                //copiar funcion
                return VCajaAlmacenesFechaMonedaEfectivoVoucher(IdAlmacen, FechaHoy.ToString("yyyy-MM-dd"), IdVoucher);
            }
            else
            {
                return View("/Views/UserPages/VVouchersCaptura.cshtml");
            }

        }

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
                        using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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
                        //InsertarValoresDeEfectivoVoucher(IdVoucher, index, denominacionList, cantidadList, importeList, idRegstroVoucherList, referenciaList, IdAlmacenParam, FechaParam);
                    }
                }
            }
            //return null;
            //return RedirectToAction("VCajaAlmacenesFechaMonedaEfectivoVoucher", new { IdAlmacen = IdAlmacenParam, Fecha = FechaParam, IdVoucher = IdVoucherParam });
            return RedirectToRoute("usuario-vouchers-captura", new { IdVoucher = IdVoucherParam });
        }

        //
        [HttpPost]
        public ActionResult VSalesRecord()
        {

            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];
            List<Tuple<int, string>> almacenes = new List<Tuple<int, string>>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

            //Response.Write(almacenes.Count());

            //var json = new JavaScriptSerializer().Serialize(almacenes);
            //Response.Write("yourObject:" + json + "<br/>");

            //si solo tiene un almacen asignado entonces procedemos, ya que si tiene mas asignados no podriamos saber a donde quiere insertar los datos
            if (almacenes.Count() == 1)
            {
                DateTime fechaParsed;
                string fechaParam = "2024-05-01";
                string almacen = "100";

                //
                if (!DateTime.TryParseExact(fechaParam, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaParsed))
                {
                    return Json(new { success = false, message = "Formato de fecha incorrecto." }, JsonRequestBehavior.AllowGet);
                }

                double TotalJoyeria = 0;

                using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
                {
                    string baseQuery = @"SELECT * FROM hoka_joyeria.dbo.VObtenerTotalProductosJoyeria WHERE fecha = @fecha AND almacen= @almacen";

                    SqlCommand cmd = new SqlCommand(baseQuery, cn);
                    cmd.Parameters.AddWithValue("@almacen", almacen);
                    cmd.Parameters.AddWithValue("@fecha", fechaParsed);
                    cmd.CommandType = CommandType.Text;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        List<RemisioDModel> rawData = new List<RemisioDModel>();

                        while (dr.Read())
                        {
                            TotalJoyeria = dr["totalJoyeria"] != DBNull.Value ? Convert.ToInt64(dr["totalJoyeria"]) : 0;
                        }
                    }
                }

                using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
                {
                    string baseQuery = @"SELECT * FROM VReportSalesRecord WHERE fecha = @fecha AND almacen = @almacen";

                    SqlCommand cmd = new SqlCommand(baseQuery, cn);

                    cmd.Parameters.AddWithValue("@fecha", fechaParsed);
                    cmd.Parameters.AddWithValue("@almacen", almacen);
                    cmd.CommandType = CommandType.Text;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        List<RemisioDModel> rawData = new List<RemisioDModel>();

                        while (dr.Read())
                        {
                            RemisioDModel LRemisioD = new RemisioDModel();


                            LRemisioD.categoria = dr["categoria"].ToString();
                            LRemisioD.stotal = dr["stotal"] != DBNull.Value ? Convert.ToInt64(dr["stotal"]) : 0;

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

                            // Agregar o sumar la venta real a la categoría correspondiente
                            if (!dataProcessed[fecha].ContainsKey(categoria))
                            {
                                dataProcessed[fecha][categoria] = 0;
                            }
                            dataProcessed[fecha][categoria] += ventaReal;

                            // Aquí agregamos o sumamos la venta real al total importe para la fecha
                            if (!dataProcessed[fecha].ContainsKey("Group_TotalImporte"))
                            {
                                dataProcessed[fecha]["Group_TotalImporte"] = 0;
                            }
                            dataProcessed[fecha]["Group_TotalImporte"] += ventaReal;
                        }

                        List<object> finalData = new List<object>();
                        var knownCategories = new List<string> {
                "ALCOHOL", "SOUVENIR", "ARTESANIAS","ARTESANIAS PREMIUM", "TEXTIL", "FARMACIA",
                "CERVEZA", "REFRESCOS", "AGUAS", "ABARROTES", "ENERGETICOS", "RTD", "SC","Group_TotalImporte"
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

                            var record = new
                            {
                                Gruop_fecha = date,
                                Group_Alcohol = dataProcessed[date].ContainsKey("ALCOHOL") ? dataProcessed[date]["ALCOHOL"] : 0,
                                Group_Souvenir = dataProcessed[date].ContainsKey("SOUVENIR") ? dataProcessed[date]["SOUVENIR"] : 0,
                                Group_Artesanias = dataProcessed[date].ContainsKey("ARTESANIAS") ? dataProcessed[date]["ARTESANIAS"] : 0,
                                Group_Artesanias_Premium = dataProcessed[date].ContainsKey("ARTESANIAS PREMIUM") ? dataProcessed[date]["ARTESANIAS PREMIUM"] : 0,
                                Group_Textil = dataProcessed[date].ContainsKey("TEXTIL") ? dataProcessed[date]["TEXTIL"] : 0,
                                Group_Farmacia = dataProcessed[date].ContainsKey("FARMACIA") ? dataProcessed[date]["FARMACIA"] : 0,
                                Group_Cerveza = dataProcessed[date].ContainsKey("CERVEZA") ? dataProcessed[date]["CERVEZA"] : 0,
                                Group_Refrescos = dataProcessed[date].ContainsKey("REFRESCOS") ? dataProcessed[date]["REFRESCOS"] : 0,
                                Group_Aguas = dataProcessed[date].ContainsKey("AGUAS") ? dataProcessed[date]["AGUAS"] : 0,
                                Group_Abarrotes = dataProcessed[date].ContainsKey("ABARROTES") ? dataProcessed[date]["ABARROTES"] : 0,
                                Group_Energeticos = dataProcessed[date].ContainsKey("ENERGETICOS") ? dataProcessed[date]["ENERGETICOS"] : 0,
                                Group_RTD = dataProcessed[date].ContainsKey("RTD") ? dataProcessed[date]["RTD"] : 0,
                                Group_SC = dataProcessed[date].ContainsKey("SC") ? dataProcessed[date]["SC"] : 0,
                                Group_TotalImporte = dataProcessed[date].ContainsKey("Group_TotalImporte") ? dataProcessed[date]["Group_TotalImporte"] : 0,
                                Group_SinCategoria = sinCategoria,
                                Group_Joyeria = TotalJoyeria
                            };

                            finalData.Add(record);
                        }

                        var jsonResult = Json(finalData, JsonRequestBehavior.AllowGet);
                        jsonResult.MaxJsonLength = int.MaxValue;
                        return jsonResult;
                    }
                }

            }
            else
            {
                var jsonResult = Json(null, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }



        }
        //
        [HttpPost]
        public ActionResult procesarVoucher()
        {
            int IdAlmacen = 0;

            // Obtiene el usuario actualmente autenticado desde la sesión
            UsuarioModel usuarioSesion = (UsuarioModel)Session["usuario"];

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

            List<Tuple<int>> almacenes = new List<Tuple<int>>();

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                SqlCommand cmd = new SqlCommand("sp_ObtenerAlmacenesPorUsuario", cn);
                cmd.Parameters.AddWithValue("@Id_Usuario", usuarioSesion.Id_Usuario);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdAlmacen = Convert.ToInt32(reader["AlmacenId"]);
                    almacenes.Add(new Tuple<int>(IdAlmacen));
                }
            }

            if (almacenes.Count == 0 || almacenes.Count > 1)
            {
                ViewBag.Mensaje = "Error. Verificar Permisos.";
                return View("/Views/UserPages/UserError.cshtml");
            }

            List<string> importeList = new List<string>();
            List<string> referenciaList = new List<string>();
            int IdVoucher = 0;
            int index = 0;

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
            }

            var json3 = new JavaScriptSerializer().Serialize(importeList);
            Response.Write("yourObject:" + json3 + "<br/>");
            Response.Write(index);

            DateTime Fecha = DateTime.Now;
            Fecha.ToString("yyyy-MM-dd");

            bool IsValid = false;
            int IdRegistroDiaVoucher = 0;

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();

                SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_ValidarRegistroPrevioVoucher", cn);

                cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                cmd.Parameters.AddWithValue("IdVoucher", IdVoucher);
                cmd.Parameters.AddWithValue("Fecha", Fecha.ToString("yyyy-MM-dd"));

                cmd.Parameters.Add("IsValid", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("IdRegistroDiaVoucher", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.ExecuteNonQuery();

                IsValid = Convert.ToBoolean(cmd.Parameters["IsValid"].Value);
                IdRegistroDiaVoucher = Convert.ToInt32(cmd.Parameters["IdRegistroDiaVoucher"].Value);

            }

            if (IsValid && IdRegistroDiaVoucher > 0)
            {
                Response.Write("siu");
                Response.Write(IdRegistroDiaVoucher);

                for (int i = 0; i < index; i++)
                {
                    using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
                    {
                        cn.Open();

                        SqlCommand cmd = new SqlCommand("ingresos.dbo.sp_RegistrarVoucher", cn);

                        cmd.Parameters.AddWithValue("Importe", importeList[i]);
                        cmd.Parameters.AddWithValue("Cantidad", 1);
                        cmd.Parameters.AddWithValue("IdAlmacen", IdAlmacen);
                        cmd.Parameters.AddWithValue("IdVoucher", IdVoucher);
                        cmd.Parameters.AddWithValue("Fecha", Fecha.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("Referencia", referenciaList[i]);
                        cmd.Parameters.AddWithValue("IdRegistroDiaVoucher", IdRegistroDiaVoucher);

                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToRoute("usuario-vouchers");

        }
        //

        [HttpPost]
        [ValidarSesion(idRol: 2, permisos: 1)]
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



            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.VentasCategorias WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
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
                            Fecha = Convert.ToDateTime(reader["Fecha"]).ToString("yyyy-MM-dd"),
                            VentaTienda = Convert.ToSingle(reader["VentaTienda"]),
                        });

                    }
                }
            }

            var jsonResult = Json(ventasCategoria, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue; // Ajuste a un valor grande como sea necesario
            return jsonResult;

        }

        //
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Monedas", cn);
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.ValoresEntregados WHERE FechaInsercion=@Fecha", cn);
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.RegistroValoresEntregados WHERE Fecha=@Fecha", cn);
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.MonedaDenominacion", cn);
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
        //
        [HttpGet]
        [ValidarSesion(idRol: 1, permisos: 1)]
        public ActionResult VCajaAlmacenesFechaMonedaEfectivoVoucher(int IdAlmacen, string Fecha, string IdVoucher)
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmdRemisioPago = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.Vouchers", cn);
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.RegistroVouchers WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
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

            using (SqlConnection cn = new SqlConnection(_conexionHokaCompuadmo))
            {
                cn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM hoka_ingresos.dbo.RegistroDiaVoucher WHERE Fecha=@Fecha AND IdAlmacen=@IdAlmacen", cn);
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

            ViewBag.IdRegistroDiaVoucher = IdRegistroDiaVoucher;

            //Response.Write(Fecha);

            //var json1 = new JavaScriptSerializer().Serialize(registroVouchers);
            //Response.Write("yourObject:" + json1 + "<br/>");

            //return null;

            //List<MonedaDenominacion> monedaDenominacion = new List<MonedaDenominacion>();
            //MonedaDenominacion monedaDenominacion = new MonedaDenominacion();


            ViewBag.RegistroVouchers = registroVouchers;
            ViewBag.RegistroDiaVoucher = registroDiaVoucher;
            ViewBag.Almacen = IdAlmacen;

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
            //return View();
            return View("/Views/UserPages/VCajaAlmacenesFechaMonedaEfectivoVoucher.cshtml");
        }
        //
    }
}