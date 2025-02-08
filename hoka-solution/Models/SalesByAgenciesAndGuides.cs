using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class SalesByAgenciesAndGuides
    {
        public DateTime fecha { get; set; }
        public string folio_remision { get; set; }
        public string estatus { get; set; }
        public double total { get; set; }
        public double descuento { get; set; }
        public float matricula { get; set; }
        public string agencia { get; set; }
        public string nom_guia { get; set; }

        public int producto { get; set; }
        public double stotal { get; set; }
        public double calendarios { get; set; }
        public string descripcion_larga { get; set; }

        public double total_venta { get; set; }
        public double venta_tienda { get; set; }

        public string tipo_venta { get; set; }
        public double cantidad_vendida { get; set; }

        //agrupacion de datos para la vista
        public string Group_estatus { get; set; }
        public double Group_total { get; set; }

        public double Group_descuento { get; set; }
        public float Group_matricula { get; set; }
        public string Group_agencia { get; set; }
        public string Group_nom_guia { get; set; }

        public int Group_producto { get; set; }
        public double Group_stotal { get; set; }
        public string Group_descripcion_larga { get; set; }

        public string Group_folio_remision { get; set; }

        public string Group_tipo_venta { get; set; }

        public double Group_cantidad_vendida { get; set; }
        public double Group_calendarios { get; set; }
        public double Group_venta_tienda { get; set; }

        public double Group_total_venta { get; set; }
        public DateTime Group_fecha { get; set; }

        //
        public int Group_pax { get; set; }



    }
}