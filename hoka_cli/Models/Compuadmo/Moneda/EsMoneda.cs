using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    public class EsMoneda : EsEstructura
    {
        public EsMoneda()
        {
            Moneda = new EnMoneda();
            Monedas = new HashSet<EnMoneda>();
        }

        public EnMoneda Moneda { get; set; }
        public ICollection<EnMoneda> Monedas { get; set; }

        #region
        public bool B_ConsultarMonedaTipo { get; set; }
        public bool B_ConsultarDenominacion { get; set; }
        public bool B_ConsultarMonedaCero { get; set; }
        #endregion
    }
}
