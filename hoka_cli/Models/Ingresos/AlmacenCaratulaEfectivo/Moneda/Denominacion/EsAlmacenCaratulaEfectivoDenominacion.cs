using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion
{
    public class EsAlmacenCaratulaEfectivoDenominacion : EsEstructura
    {
        public EsAlmacenCaratulaEfectivoDenominacion()
        {
            Denominacion = new EnAlmacenCaratulaEfectivoDenominacion();
            Denominaciones = new HashSet<EnAlmacenCaratulaEfectivoDenominacion>();
        }

        public EnAlmacenCaratulaEfectivoDenominacion Denominacion { get; set; }
        public ICollection<EnAlmacenCaratulaEfectivoDenominacion> Denominaciones { get; set; }

        #region
        public bool B_ConsultarMonedaDenominacion { get; set; }
        #endregion
    }
}
