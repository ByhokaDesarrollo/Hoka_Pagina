using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RemisioDJoyModel
    {
        public string folio_factura { get; set; }
        public string codigo_barras { get; set; }
        public string Nombre_largo { get; set; } // por si queremos conocer el codigo
        public string almacen { get; set; }
        public string categoria { get; set; }
        public double cantidads { get; set; } //columna unidades vendidas
        public double peso { get; set; } //columna unidades vendidas
        public double stotal { get; set; } //columna venta real
        
        public double total { get; set; }
        public string deportiva { get; set; } // validadcion de a excepcion de solo 1 f
        public DateTime fecha { get; set; }
        public double Existencia { get; set; } // este es el if
        public double ExistenciaGms { get; set; } // este es el ifk
        public double saldo { get; set; }
        public double tipo_cambio { get; set; }


        //son las propiedades que no estan en la base de datos

        public double costo { get; set; }
        public double sumaStotal { get; set; }



        public double VentaReal { get; set; }
        public double total_depor { get; set; }
        public double vcosto { get; set; }
        public double utilidad { get; set; }
        public double utiladidad { get; set; }

        public double impuesto { get; set; }
        public double VentaSinIVA { get; set; }
        public double VentaIVA { get; set; }
        public double VentaConIVA { get; set; }
        public double ValorInventario { get; set; }

    }

}
