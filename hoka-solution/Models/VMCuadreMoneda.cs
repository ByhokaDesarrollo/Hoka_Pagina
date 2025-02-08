using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class VMCuadreMoneda
    {

        public int Id { get; set; }
        public string MonedaNombre { get; set; }
        public double ImporteMonedaPesos { get; set; }
        public double ImporteMonedaOrigen { get; set; }
        public int Cantidad { get; set; }

        public double Recepcion { get; set; }

        //IdSummary de cuadrevouchersummary

        public int IdSummary { get; set; }
        //
        public int IdMoneda { get; set; }
        //
        public string Fecha { get; set; }
    }
}