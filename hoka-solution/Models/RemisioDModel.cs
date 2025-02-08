using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioDModel
    {
        public string codigobarras { get; set; }
        public int producto { get; set; } // por si queremos conocer el codigo 
        public string productoNombre { get; set; }
        public string almacen { get; set; }
        public string categoria { get; set; } //columna linea
        public string grupo { get; set; }
        public double cantidads { get; set; } //columna unidades vendidas
        public double cantidadsrotacion { get; set; } //columna unidades vendidas

        public double stotal { get; set; } //columna venta real
        public double vcosto { get; set; } // formula es cantidads x costo
        public double costo { get; set; }
        public double Existencia { get; set; }
        public double VInventario { get; set; }
        public float preciopub { get; set; }
        public double impuesto { get; set; }
        public double iva { get; set; }
        public double utilidad { get; set; } // sirve para venta real - venta costo
        public double depor { get; set; } // validadcion de a excepcion de solo 1 f
        public string deportiva { get; set; } // validadcion de a excepcion de solo 1 f
        public string depor_string { get; set; } 
        public double fijo { get; set; } //validacion cuando solo hay una f
        public DateTime? fecha { get; set; }
        public DateTime? ultVentafecha { get; set; }
        public string ultVentaDias { get; set; }
        public int? ultVentaDiasNumeric { get; set; }

        public double pvd { get; set; }
        public double rotacion_mensual { get; set; }

        // Nuevas propiedades agregadas
        public double total_cantidads { get; set; }
        public double total_stotal { get; set; }
        public double total_fijo { get; set; }
        public double total_depor { get; set; }


        //tabla remisioM
        public double total { get; set; }
        public double descuento { get; set; }
        public double VentaReal { get; set; }
        public double VentaSinIVA { get; set; }
        public double VentaIVA { get; set; }
        public double VentaConIVA { get; set; }

        //tabla almaproductos
        public double ultcost { get; set; }
        
    }


}


/*public string descripcion_larga { get; set; }*/  //se utilizaba para obtener el nombre
/*public string deportiva { get; set; }*/ //se hace una configuracion para sacar la deportiva pero luego lo vemos