using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class VistaVendedorModel
    {
        public DateTime fecha { get; set; }
        public string nombre { get; set; }
        public string folio_remision { get; set; } 
        public string almacen { get; set; }
        public int cuantosint { get; set; }
        public double totalventa { get; set; }
        public double comisiondepor { get; set; }
        public double comisionfijo { get; set; }
        public double ventatotalporvendedor { get; set; }
        public double totalcomision { get; set; }

    }
}