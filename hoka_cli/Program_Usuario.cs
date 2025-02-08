using hoka_cli.Models.Compuadmo.Usuario;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_Usuario
    {
        static void AMain(string[] args)
        {
            #region Usuario
            // Usuario
            RpUsuario rpUsuario = SvUsuarioIniciarRepositorio.IniciarRepositorio();
            EsUsuario esUsuario = new EsUsuario()
            {
                Consulta = new EsConsulta()
                {
                    //TablaRegistroEstatusId = 1
                },
                Usuario = new EnUsuario()
                {
                },
                B_ConsultarRol = true,
                B_ConsultarPerfil = true
            };
            SvUsuarioConsultar servicio = new SvUsuarioConsultar(rpUsuario, esUsuario);
            ICollection<EnUsuario> Usuarioes = servicio.Consultar();
            string jsonUsuarioes = JsonConvert.SerializeObject(Usuarioes);
            Console.WriteLine(jsonUsuarioes);
            Console.ReadKey();
            #endregion
        }
    }
}
