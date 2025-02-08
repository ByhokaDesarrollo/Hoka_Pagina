using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratula
{
    public class EsAlmacenCaratula : EsEstructura
    {
        public EsAlmacenCaratula()
        {
            Caratula = new EnAlmacenCaratula();
            Caratulas = new HashSet<EnAlmacenCaratula>();
        }

        public EnAlmacenCaratula Caratula { get; set; }
        public ICollection<EnAlmacenCaratula> Caratulas { get; set; }

        #region Propiedades
        public bool B_ConsultarAlmacen { get; set; }
        public bool B_ConsultarUsuario { get; set; }
        public bool B_ConsultarEstatus { get; set; }
        public bool B_ConsultarCaratulaVenta { get; set; }
        public bool B_ConsultarCaratulaEfectivo { get; set; }
        public bool B_ConsultarCaratulaVoucher { get; set; }
        public bool B_ConsultarCaratula { get; set; }
        #endregion
    }
}
