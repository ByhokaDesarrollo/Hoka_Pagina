using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RegistroVouchers
    {

        public int Id { get; set; }

        public float Importe { get; set; }

        public int IdAlmacen { get; set; }
        public string Fecha { get; set; }

        public int IdRegistroDiaVoucher { get; set; }
        public int IdVoucher { get; set; }
        public int Cantidad { get; set; }
        public string Referencia { get; set; }

    }
}