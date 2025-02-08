using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class EsAlmacenCaratulaVoucher : EsEstructura
    {
        public EsAlmacenCaratulaVoucher()
        {
            CaratulaVoucher = new EnAlmacenCaratulaVoucher();
            CaratulasVoucher = new HashSet<EnAlmacenCaratulaVoucher>();
        }

        public EnAlmacenCaratulaVoucher CaratulaVoucher { get; set; }
        public ICollection<EnAlmacenCaratulaVoucher> CaratulasVoucher { get; set; }

        #region Propiedades
        public bool B_ConsultarVoucher { get; set; }
        #endregion
    }
}
