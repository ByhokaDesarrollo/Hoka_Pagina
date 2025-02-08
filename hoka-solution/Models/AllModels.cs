using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class AllModels
    {
        public List<UsuarioModel> UsuariosM { get; set; }
        public UsuarioModel UsuarioMNoList { get; set; }
        public List<RolModel> RolesM { get; set; }
        public RolModel RolesMNoList { get; set; }
        public List<PerfilModel> PerfilM { get; set; }
        public PerfilModel PerfilMNoList { get; set; }
        public List<EstadoModel> EstadoUM { get; set; }
        public EstadoModel EstadoUMNoList { get; set; }
        public List<almacenesModel> AlmacenesM { get; set; }
        public almacenesModel AlmacenesMNoList { get; set; }
        public List<UsuariosAlmacenesModel> UsuariosAlmacenesM { get; set; }
        public UsuariosAlmacenesModel UsuariosAlmacenesMNoList { get; set; }
        public List<PermisosModel> PermisosM { get; set; }
        public PermisosModel PermisosMNoList { get; set; }
        public List<UsuariosPermisosModel> UsuariosPermisosM { get; set; }
        public UsuariosPermisosModel UsuariosPermisosMNoList { get; set; }
    }
}