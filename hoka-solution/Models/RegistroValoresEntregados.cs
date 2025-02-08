using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RegistroValoresEntregados
    {
        public int Id { get; set; }
        
        public int IdAlmacen {  get; set; }
        public string Fecha { get; set;}
        public int IdMoneda { get; set; }
        public float TipoCambio { get; set; }
        public int Estado {  get; set; }

    }
}