using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Catalogo.Almacen
{
    public class EsCatalogoAlmacen : EsEstructura
    {
        public EsCatalogoAlmacen()
        {
            CatalogoAlmacen = new EnCatalogoAlmacen();
            CatalogoAlmacenes = new HashSet<EnCatalogoAlmacen>();
        }

        public EnCatalogoAlmacen CatalogoAlmacen { get; set; }
        public ICollection<EnCatalogoAlmacen> CatalogoAlmacenes { get; set; }
    }
}
