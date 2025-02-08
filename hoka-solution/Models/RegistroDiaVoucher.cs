using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class RegistroDiaVoucher
    {

        public int Id { get; set; }

        public int IdAlmacen { get; set; }
        public string Fecha { get; set; }
        public int IdVoucher { get; set; }
    }
}