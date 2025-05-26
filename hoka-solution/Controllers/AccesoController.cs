using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using hoka.Permisos;
using hoka.Models;
using System.Web.WebPages;
using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka.AppServicios.Compuadmo.Usuario.Sesion;

namespace hoka.Controllers
{
    public class AccesoController : Controller
    {

        private static string DBHoka = ConfigurationManager.ConnectionStrings["CadenaConexionHokaCompuadmo"].ToString();
        private static string DBJoy = ConfigurationManager.ConnectionStrings["CadenaConexionHokaJoyeria"].ToString();

        private static string DBHokaTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaCompuadmo"].ToString();
        private static string DBJoyTEST = ConfigurationManager.ConnectionStrings["CadenaConexionPruebaHokaJoyeria"].ToString();

        private static bool conexionDBDEV = ConfigurationManager.AppSettings["ENV_DB_DEV"].AsBool();

        private static string conexionDBHoka;
        private static string conexionDBJoy;

        public AccesoController()
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

        [ValidarSesion(idRol: 3)]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Acceso
        public ActionResult Login()
        {

            return View();
        }

        private int? ConvertToNullableInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }
            return null;
        }

        [HttpPost]
        public ActionResult Login(UsuarioModel oUsuario)
        {
            string mensaje = "¡Bienvenido!";

            // <VL> Validar campos
            if (string.IsNullOrEmpty(oUsuario.Correo) ||
                string.IsNullOrEmpty(oUsuario.Clave))
            {
                if (string.IsNullOrEmpty(oUsuario.Correo))
                    mensaje = "Se requiere un Correo válido";
                if (string.IsNullOrEmpty(oUsuario.Clave))
                    mensaje = "Se requiere una Clave válida.";
                return Json(new { success = false, message = mensaje });
            }
            EsUsuario esUsuario = new EsUsuario()
            {
                Usuario = new EnUsuario()
                {
                    Correo = oUsuario.Correo,
                    Clave = oUsuario.Clave
                }
            };
            EnUsuario Usuario = SvAppUsuarioIniciarSesion.IniciarSesion(esUsuario);
            Session["SsUsuario"] = Usuario;
            // </VL>

            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection cn = new SqlConnection(conexionDBHoka))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("@Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("@Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {

                    List<int?> almacenesId = new List<int?>();
                    List<int?> permisosId = new List<int?>();
                    while (reader.Read())
                    {

                        // Verifica si el resultado contiene la columna 'Mensaje'
                        if (reader.GetSchemaTable().Rows.OfType<DataRow>().Any(r => r["ColumnName"].Equals("Mensaje")))
                        {
                            string mensajebd = reader["Mensaje"].ToString();
                            // Aquí puedes manejar el mensaje de error devuelto por tu procedimiento almacenado.
                            // Por ejemplo, puedes devolver una respuesta JSON con el mensaje de error:
                            return Json(new { success = false, message = mensajebd });
                        }

                        // Si no hay mensaje de error, continua con el procesamiento normal:
                        int idUsuario = Convert.ToInt32(reader["IDUsuario"]);
                        int idRol = Convert.ToInt32(reader["IDRol"]);
                        int? idPerfil = reader["IDPerfil"] != DBNull.Value ? Convert.ToInt32(reader["IDPerfil"]) : (int?)null;
                        int? idAlmacen = ConvertToNullableInt(reader["IDAlmacenes"].ToString());
                        int? idPermiso = ConvertToNullableInt(reader["IDPermisos"].ToString());

                        if (idAlmacen.HasValue)
                        {
                            almacenesId.Add(idAlmacen.Value);
                        }

                        if (idPermiso.HasValue)
                        {
                            permisosId.Add(idPermiso.Value);
                        }

                        int idEstado = Convert.ToInt32(reader["IDEstado"]);
                        almacenesId.Add(idAlmacen ?? 0);  // Asume que un valor de null se traduce en 0

                        // Crear una instancia de UsuarioSesion y asignar los valores correspondientes
                        UsuarioModel usuarioSesion = new UsuarioModel
                        {
                            Correo = oUsuario.Correo,
                            Clave = oUsuario.Clave,
                            Id_Usuario = idUsuario,
                            Id_Rol = idRol,
                            Id_Perfil = idPerfil.HasValue ? idPerfil.Value : 0,  // O el valor que desees por defecto
                            Id_Almacen = idAlmacen ?? 0,
                            Id_Permiso = idPermiso ?? 0,
                            Id_Estado = idEstado,
                        };

                        // Guardar la instancia de UsuarioSesion en la sesión
                        Session["NombreUser"] = reader["NombreUser"].ToString();
                        Session["NombreRol"] = reader["NombreRol"].ToString();  // Asumiendo que 'NombreRol' es el nombre de la columna en la BD.
                        Session["NombrePerfil"] = reader["NombrePerfil"].ToString();  // Asumiendo que 'NombrePerfil' es el nombre de la columna en la BD.
                        Session["usuario"] = usuarioSesion;
                        Session["almacenesId"] = almacenesId.Distinct().ToList();  // Filtrar duplicados
                        Session["permisosId"] = permisosId.Distinct().ToList();

                    }

                    Session["almacenesId"] = almacenesId;

                    mensaje = "¡Bienvenido de vuelta!";

                    UsuarioModel usuarioSesionPrimero = Session["usuario"] as UsuarioModel;

                    // Verificar el estado del usuario
                    if (usuarioSesionPrimero.Id_Estado == 1) // Estado ACTIVO
                    {
                        // Realizar las redirecciones basadas en los valores de Id_Rol, Id_Ciudad e Id_Tienda
                        if (usuarioSesionPrimero.Id_Rol == 1) // ADMIN
                        {
                            return Json(new { success = true, message = mensaje, redirectToUrl = "/AdminPages/InicioAdmin" });
                        }
                        else if (usuarioSesionPrimero.Id_Rol == 2) //USUARIOS
                        {
                            return Json(new { success = true, message = mensaje, redirectToUrl = "/UserPages/InicioUser" });
                        }
                        else if (usuarioSesionPrimero.Id_Rol == 3) //AdminTotal
                        {
                            return Json(new { success = true, message = mensaje, redirectToUrl = "/AdminPages/InicioAdminTotal" });
                        }
                        else
                        {
                            // Redirigir a una página de error en caso de que los valores no coincidan
                            mensaje = "no tiene acceso a esta pagina";
                            return Json(new { success = false, message = mensaje, redirectToUrl = "/Home/ErrorView?" });
                        }
                    }
                    else if (usuarioSesionPrimero.Id_Estado == 2)
                    {
                        // Redirigir a una página de error en caso de que el estado no sea activo
                        return Json(new { success = false, message = "Tu cuenta está inactiva.", redirectToUrl = "/Acceso/Login" });
                    }
                }
                else
                {
                    mensaje = "Tu clave es incorrecto";
                    return Json(new { success = false, message = mensaje, redirectToUrl = "/Acceso/Login" });
                }
            }

            return Json(new { success = false, message = "Error en el inicio de sesion", redirectToUrl = "/Acceso/Login" });
        }

        public ActionResult CerrarSesion()
        {
            // Limpiar la sesión
            // <VL> Cerrar Sesion
            Session["SsUsuario"] = null;
            // </VL>
            Session["usuario"] = null;

            // Establecer encabezados para evitar el almacenamiento en caché
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();

            return RedirectToAction("Login", "Acceso");
        }

        //Para encryptar contraseñas
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



    }
}