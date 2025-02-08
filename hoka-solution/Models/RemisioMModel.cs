using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioMModel
    {

        //parametros para la vista VRemisioMWeb

        public string folio_remision { get; set; }
        public string folio_factura { get; set; }
        public string almacen { get; set; }
        public double tipo_cambio { get; set; }
        public double stotal { get; set; }
        public DateTime fecha { get; set; }
        public string observaciones { get; set; }

    }
}