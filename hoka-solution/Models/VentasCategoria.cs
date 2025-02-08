using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class VentasCategoria
    {
        public int Id { get; set; } 
        public int IdRegistroVentas { get; set; }
        public string Categoria { get; set; }
        public float Venta { get; set; }
        public float VentaTienda { get; set; }
        public float Comisiones { get; set; }
        public float NetoVenta { get; set; }
     
        public string Fecha { get; set; }

        public int Estado { get; set; }

    }
}