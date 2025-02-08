using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Almacen
{
    public class EsAlmacen : EsEstructura
    {
        public EsAlmacen()
        {
            Almacen = new EnAlmacen();
            Almacenes = new HashSet<EnAlmacen>();
        }

        public EnAlmacen Almacen { get; set; }
        public ICollection<EnAlmacen> Almacenes { get; set; }
    }
}
