using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class VMCuadreVoucher
    {

        public int Id { get; set; }
        public string VoucherNombre { get; set; }
        public double Importe { get; set; }
        public int Cantidad { get; set; }

        public double Recepcion { get; set; }

        //IdSummary de cuadrevouchersummary

        public int IdSummary { get; set; }
        //
        public int IdVoucher { get; set; }
        //
        public string Fecha { get; set; }
    }
}