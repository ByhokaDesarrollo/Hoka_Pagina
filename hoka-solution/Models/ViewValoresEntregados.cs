using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class ViewValoresEntregados
    {
        public int Id { get; set; }
        public float ImporteMonedaPesos { get; set; }
        public float ImporteMonedaOrigen { get; set; }
        public float IdRegistroValoresEntregados { get; set; }
        public string MonedaNombre { get; set; }
        public int IdMoneda { get; set; }

    }
}