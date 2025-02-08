using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioDyMModel
    {
        //parametros para la vista VRemisioDyMWeb
        public string folio_remision { get; set; }
        public string folio_factura { get; set; }
        public string descripcion_larga { get; set; }
        public string codigobarras { get; set; }
        public string codigo_barras { get; set; }
        public string NombreAlmacen { get; set; }
        public double? cantidads { get; set; }
        public string deportiva { get; set; }
        public DateTime? fecha { get; set; }

    }
}