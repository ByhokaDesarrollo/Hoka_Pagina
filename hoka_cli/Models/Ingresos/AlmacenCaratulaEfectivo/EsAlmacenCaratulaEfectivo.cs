using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class EsAlmacenCaratulaEfectivo : EsEstructura
    {
        public EsAlmacenCaratulaEfectivo()
        {
            CaratulaEfectivo = new EnAlmacenCaratulaEfectivo();
            CaratulasEfectivo = new HashSet<EnAlmacenCaratulaEfectivo>();
        }

        public EnAlmacenCaratulaEfectivo CaratulaEfectivo { get; set; }
        public ICollection<EnAlmacenCaratulaEfectivo> CaratulasEfectivo { get; set; }

        #region Propiedades
        public bool B_ConsultarMoneda { get; set; }
        #endregion
    }
}
