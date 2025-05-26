using hoka_cli.Models.Compuadmo.Usuario;
using Newtonsoft.Json;

namespace hoka_cli.HokaApp.Compuadmo.Usuario.Sesion
{
    public static class SvUsuarioIniciarSesion
    {
        public static string IniciarSesion(EsUsuario estructura)
        {
            SvUsuario Servicio = new SvUsuario();
            Servicio.ServicioMaestro("IniciarSesion", estructura);
            EnUsuario Resultado = Servicio.Estructura.Usuario;
            string jsonRespuesta = JsonConvert.SerializeObject(Resultado);
            return jsonRespuesta;
        }
    }
}
