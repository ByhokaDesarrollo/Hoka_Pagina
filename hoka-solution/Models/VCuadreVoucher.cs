using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class VCuadreVoucher
    {

        public int Id { get; set; }
        public string VoucherNombre { get; set; }
        public double Importe { get; set; }
        public int Cantidad { get; set; }

    }
}