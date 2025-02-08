using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Moneda.Denominacion
{
    public class EsMonedaDenominacion : EsEstructura
    {
        public EsMonedaDenominacion()
        {
            MonedaDenominacion = new EnMonedaDenominacion();
            MonedaDenominaciones = new HashSet<EnMonedaDenominacion>();
        }

        public EnMonedaDenominacion MonedaDenominacion { get; set; }
        public ICollection<EnMonedaDenominacion> MonedaDenominaciones { get; set; }

        #region
        public bool B_ConsultarMonedaCero { get; set; }
        #endregion
    }
}
