using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioMPagoModel
    {
        public string folio_factura { get; set; }
        public double? total { get; set; }
        public string NombreMoneda { get; set; }
        public string NombreAlmacen { get; set; }
        public DateTime? fecha_pago { get; set; }

    }
}