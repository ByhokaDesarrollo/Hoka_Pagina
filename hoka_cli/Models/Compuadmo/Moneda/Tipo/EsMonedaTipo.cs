using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Moneda.Tipo
{
    public class EsMonedaTipo : EsEstructura
    {
        public EsMonedaTipo()
        {
            MonedaTipo = new EnMonedaTipo();
            MonedaTipos = new HashSet<EnMonedaTipo>();
        }

        public EnMonedaTipo MonedaTipo { get; set; }
        public ICollection<EnMonedaTipo> MonedaTipos { get; set; }
    }
}
