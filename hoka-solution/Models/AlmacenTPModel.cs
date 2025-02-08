using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class AlmacenTPModel
    {

        public string categoria { get; set; }
        public double total { get; set; }
        public string moneda { get; set; }
        public DateTime fecha { get; set; }
        public string almacen { get; set; } 
        public string folio_remision { get; set; }

    }
}