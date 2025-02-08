using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class EsAlmacenCaratulaVenta : EsEstructura
    {
        public EsAlmacenCaratulaVenta()
        {
            CaratulaVenta = new EnAlmacenCaratulaVenta();
            CaratulasVenta = new HashSet<EnAlmacenCaratulaVenta>();
        }

        public EnAlmacenCaratulaVenta CaratulaVenta { get; set; }
        public ICollection<EnAlmacenCaratulaVenta> CaratulasVenta { get; set; }

        #region Propiedades
        public bool B_ConsultarCategoria { get; set; }
        #endregion
    }
}
