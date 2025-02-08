using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace hoka.Models
{
    public class ProductoSinRemisionModel
    {
        // Propiedades del modelo (ajústalas según tu base de datos) 
        public string codigobarra { get; set; }
        public string Producto { get; set; }
        public string Nombre { get; set; }
        public string Categoria { get; set; }
        public string Proveedor { get; set; }
        public string preciopub { get; set; }
        public string grupo { get; set; }
        public string Existencias { get; internal set; }

        /* public string  { get; set; }
          public string  { get; set; }
          public string  { get; set; }
          public string  { get; set; }*/



        // Agrega más propiedades según tu modelo

        // Puedes agregar métodos adicionales si es necesario
    }
}