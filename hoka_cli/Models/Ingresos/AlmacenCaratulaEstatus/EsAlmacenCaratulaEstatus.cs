using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus
{
    public class EsAlmacenCaratulaEstatus : EsEstructura
    {
        public EsAlmacenCaratulaEstatus()
        {
            CaratulaEstatus = new EnAlmacenCaratulaEstatus();
            CaratulasEstatus = new HashSet<EnAlmacenCaratulaEstatus>();
        }

        public EnAlmacenCaratulaEstatus CaratulaEstatus { get; set; }
        public ICollection<EnAlmacenCaratulaEstatus> CaratulasEstatus { get; set; }
    }
}
