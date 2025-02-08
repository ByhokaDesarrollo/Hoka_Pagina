using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula
{
    [Table("UsuarioPermisoAlmacenCaratula")]
    public class EnUsuarioPermisoAlmacenCaratula
    {
        [Key]
        public int UsuarioPermisoId { get; set; }
        public int UsuarioId { get; set; }
        public int AlmacenId { get; set; }
        public bool B_CaratulaVentaCrear { get; set; }
        public bool B_CaratulaVentaActualizar { get; set; }
        public bool B_CaratulaVentaConsultar { get; set; }
        public bool B_CaratulaEfectivoCrear { get; set; }
        public bool B_CaratulaEfectivoActualizar { get; set; }
        public bool B_CaratulaEfectivoConsultar { get; set; }
        public bool B_CaratulaVoucherCrear { get; set; }
        public bool B_CaratulaVoucherActualizar { get; set; }
        public bool B_CaratulaVoucherConsultar { get; set; }
        public bool B_CaratulaConsultar { get; set; }
        public bool B_CaratulaBloquear { get; set; }
        public bool B_CaratulaDesbloquear { get; set; }
        public bool B_Activo { get; set; }
    }
}
