using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class ViewStoresModel
    {
        public DateTime x_date { get; set; }
        public string agency_name { get; set; }
        public string hotel { get; set; }
        public string guide { get; set; }
        public string pax { get; set; }
        public string description { get; set; }
        public string product_code { get; set; }

        public string reference { get; set; }
        public float total { get; set; }
        public float folio_remision { get; set; }
        public int sucursal { get; set; }

        public string nom_guia { get; set; }

        public double totalx { get; set; }
    }
}