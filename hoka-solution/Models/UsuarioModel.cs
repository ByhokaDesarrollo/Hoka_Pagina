using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace hoka.Models
{
    public class UsuarioModel
    {
        public int Id_Usuario { get; set; }
        public string NombreU { get; set; }
        public string ApellidoU { get; set; }
        public DateTime FechaNacimientoU { get; set; }
        public string Telefono { get; set; }

        [EmailAddress(ErrorMessage = "El campo Correo no es una dirección de correo electrónico válida")]
        public string Correo { get; set; }
        public string ClaveActual { get; set; }
        public string Clave { get; set; }
        public string ConfirmarClave { get; set; }
        public int Id_Rol { get; set; }
        public int? Id_Perfil { get; set; }
        public int? Id_Almacen { get; set; }
        public int? Id_Permiso { get; set; }
        public string Nombre { get; set; }
        public string Almacen { get; set; }
        public string Permiso { get; set; }
        public List<int?> Almacenes { get; set; }  // Lista de ID de los almacenes
        public List<int?> Permisos { get; set; }  // Lista de ID de los permisos

        public int Id_Estado { get; set; }
        public DateTime FechaRegistroU { get; set; }

    }
}