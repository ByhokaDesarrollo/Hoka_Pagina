using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    public class EsAlmacenCategoriaVenta : EsEstructura
    {
        public EsAlmacenCategoriaVenta()
        {
            AlmacenCategoriaVenta = new EnAlmacenCategoriaVenta();
            AlmacenesCategoriaVenta = new HashSet<EnAlmacenCategoriaVenta>();
        }

        public EnAlmacenCategoriaVenta AlmacenCategoriaVenta { get; set; }
        public ICollection<EnAlmacenCategoriaVenta> AlmacenesCategoriaVenta { get; set; }

        #region Propiedades
        public bool B_ConsultarAlmacen { get; set; }
        public bool B_ConsultarCategoria { get; set; }
        #endregion
    }
}
