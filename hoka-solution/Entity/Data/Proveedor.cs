using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace hoka.Entity.Data
{

    public class Proveedor
    {
        [Key]
        [Column("provedor")]
        public int ProvedorId { get; set; }

        [Column("nombre_rasonsocial")]
        public string NombreRazonSocial { get; set; }

    }
}
