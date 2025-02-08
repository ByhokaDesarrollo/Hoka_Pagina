using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda
{
    public class EsAlmacenCaratulaEfectivoMoneda : EsEstructura
    {
        public EsAlmacenCaratulaEfectivoMoneda()
        {
            Moneda = new EnAlmacenCaratulaEfectivoMoneda();
            Monedas = new HashSet<EnAlmacenCaratulaEfectivoMoneda>();
        }

        public EnAlmacenCaratulaEfectivoMoneda Moneda { get; set; }
        public ICollection<EnAlmacenCaratulaEfectivoMoneda> Monedas { get; set; }

        #region
        public bool B_ConsultarMoneda { get; set; }
        public bool B_ConsultarMonedaDenominacion { get; set; }
        #endregion
    }
}
