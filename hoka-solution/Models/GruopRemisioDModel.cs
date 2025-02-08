using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class GruopRemisioDModel
    {

        public string Group_codigobarras { get; set; }
        public int producto { get; set; } // por si queremos conocer el codigo 
        public string Group_productoNombre { get; set; }
        public string Group_almacen { get; set; }
        public string Group_categoria { get; set; } //columna linea
        public string Group_grupo { get; set; }
        public double Group_costo { get; set; } //columna unidades vendidas
        public double stotal { get; set; } //columna venta real
        public double Group_vcosto { get; set; } // formula es cantidads x costo
        public double costo { get; set; }
        public double Group_Existencia { get; set; }
        public double Group_VInventario { get; set; }
        public float Group_preciopub { get; set; }
        public double Group_utilidad { get; set; } // sirve para venta real - venta costo
        public double depor { get; set; } // validadcion de a excepcion de solo 1 f
        public string deportiva { get; set; } // validadcion de a excepcion de solo 1 f
        public double fijo { get; set; } //validacion cuando solo hay una f
        public DateTime Gruop_fecha { get; set; }
        // Nuevas propiedades agregadas
        public double Group_cantidads { get; set; }
        public double Group_peso { get; set; }
        public double Group_total_cantidads { get; set; }
        public double total_stotal { get; set; }
        public double Group_total_fijo { get; set; }
        public double Group_total_depor { get; set; }

        public string Depor_string { get; set; }

        //tabla remisioM
        public double total { get; set; }
        public double descuento { get; set; }
        public double Group_VentaReal { get; set; }
        public double Group_VentaSinIVA { get; set; }
        public double Group_VentaIVA { get; set; }
        public double Group_VentaConIVA { get; set; }
        public double Group_cantidadsrotacion { get; set; }
        public double Group_impuesto { get; set; }
        public double Group_iva { get; set; }



        //tabla vendedores

        public string Group_nombre { get; set; }
        public double Group_VentatotalporvendedorSum { get; set; }
        public double Group_TotalcomisionSum { get; set; }

        // tabla almaproductos
        public double ultcost { get; set; }

    }
}