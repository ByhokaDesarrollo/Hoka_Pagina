using hoka_cli.Models.Compuadmo.UsuarioPerfil;
using hoka_cli.Struct;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_UsuarioPerfil
    {
        static void AMain(string[] args)
        {
            #region UsuarioPerfil
            // UsuarioPerfil
            RpUsuarioPerfil rpUsuarioPerfil = SvUsuarioPerfilIniciarRepositorio.IniciarRepositorio();
            EsUsuarioPerfil esUsuarioPerfil = new EsUsuarioPerfil()
            {
                Consulta = new EsConsulta()
                {
                    TablaRegistroEstatusId = 1
                },
                UsuarioPerfil = new EnUsuarioPerfil()
                {
                },
                B_ConsultarRol = true
            };
            SvUsuarioPerfilConsultar servicio = new SvUsuarioPerfilConsultar(rpUsuarioPerfil, esUsuarioPerfil);
            ICollection<EnUsuarioPerfil> UsuarioPerfiles = servicio.Consultar();
            string jsonUsuarioPerfiles = JsonConvert.SerializeObject(UsuarioPerfiles);
            Console.WriteLine(jsonUsuarioPerfiles);
            Console.ReadKey();
            #endregion
        }
    }
}
