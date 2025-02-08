using hoka_cli.Models.Compuadmo.UsuarioRol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace hoka_cli
{
    internal class Program_UsuarioRol
    {
        static void AMain(string[] args)
        {
            #region UsuarioRol
            // UsuarioRol
            RpUsuarioRol rpUsuarioRol = SvUsuarioRolIniciarRepositorio.IniciarRepositorio();
            EsUsuarioRol esUsuarioRol = new EsUsuarioRol()
            {
                //Consulta = new EsConsulta()
                //{
                //    TablaRegistroEstatusId = 1
                //}
            };
            SvUsuarioRolConsultar servicioUsuarioRol = new SvUsuarioRolConsultar(rpUsuarioRol, esUsuarioRol);
            ICollection<EnUsuarioRol> UsuarioRoles = servicioUsuarioRol.Consultar();
            string jsonUsuarioRoles = JsonConvert.SerializeObject(UsuarioRoles);
            Console.WriteLine(jsonUsuarioRoles);
            Console.ReadKey();
            #endregion
        }
    }
}
