using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria
{
    public class EsAlmacenCaratulaVentaCategoria : EsEstructura
    {
        public EsAlmacenCaratulaVentaCategoria()
        {
            CaratulaVentaCategoria = new EnAlmacenCaratulaVentaCategoria();
            CaratulasVentaCategoria = new HashSet<EnAlmacenCaratulaVentaCategoria>();
        }

        public EnAlmacenCaratulaVentaCategoria CaratulaVentaCategoria { get; set; }
        public ICollection<EnAlmacenCaratulaVentaCategoria> CaratulasVentaCategoria { get; set; }

        #region
        public bool B_ConsultarCategoria { get; set; }
        #endregion
    }
}
