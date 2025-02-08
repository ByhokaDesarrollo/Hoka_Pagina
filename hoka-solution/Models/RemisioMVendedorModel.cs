using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioMVendedorModel
    {
        //parametros para la vista VRemisioMVendedor

        //esta folio factura, NombreAlmacen y fecha

        public string folio_factura { get; set; }
        public string NombreVendedor { get; set; } //Nombre del vendedor, cambiar de ser necesario
        public string NombreAlmacen { get; set; }
        public DateTime? fecha { get; set; }

    }
}