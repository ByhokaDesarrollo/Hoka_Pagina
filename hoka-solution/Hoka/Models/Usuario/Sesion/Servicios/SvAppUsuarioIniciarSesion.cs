using hoka.HokaCli.Models.Compuadmo.Usuario;
using hoka_cli.HokaApp.Usuario.Sesion;
using Newtonsoft.Json;
using HokaCli_EsUsuario = hoka_cli.Models.Compuadmo.Usuario.EsUsuario;

namespace hoka.Hoka.Models.Usuario.Sesion.Servicios
{
    public static class SvAppUsuarioIniciarSesion
    {
        public static EnUsuario IniciarSesion(EsUsuario estructura)
        {
            EsUsuario Estructura = EstablecerEntidad(
                estructura.Usuario.Correo,
                estructura.Usuario.Clave);
            HokaCli_EsUsuario EntidadParametro = ConvertirParametro(Estructura);
            string jsonEntidad = SvUsuarioIniciarSesion.IniciarSesion(EntidadParametro);
            EnUsuario Entidad = JsonConvert.DeserializeObject<EnUsuario>(jsonEntidad);
            return Entidad;
        }

        private static EsUsuario EstablecerEntidad(string correo, string clave)
        {
            EsUsuario Estructura = new EsUsuario()
            {
                Usuario = new EnUsuario()
                {
                    Correo = correo,
                    Clave = clave
                },
                B_ConsultarRol = true,
                B_ConsultarPerfil = true,
                B_ConsultarPermisoAlmacenCaratula = true
            };
            return Estructura;
        }

        private static HokaCli_EsUsuario ConvertirParametro(object estructura)
        {
            string jsonEstructura = JsonConvert.SerializeObject(estructura);
            HokaCli_EsUsuario ObjetoConvertido =
                JsonConvert.DeserializeObject<HokaCli_EsUsuario>(jsonEstructura);
            return ObjetoConvertido;
        }
    }
}