using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class ValoresEntregados
    {
        public int Id { get; set; }
        public float Cantidad { get; set; }
        public double Importe { get; set; }
        public int IdMonedaDenominacion { get; set; }
        public string FechaInsercion { get; set; }
        public float TipoCambio { get; set; }
        public int IdAlmacen {  get; set; }
        public int IdMoneda { get; set;}
        public int IdRegistroValoresEntregados { get; set; }

    }
}