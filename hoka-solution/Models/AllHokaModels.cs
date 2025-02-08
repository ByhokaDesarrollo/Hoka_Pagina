using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hoka.Models
{
    public class AllHokaModels
    {


        public List<RemisioMModel> RemisioMM { get; set; } //tabla remisioM
        public RemisioMModel RemisioMMNoList { get; set; }
        public List<RemisioMPagoModel> RemisioMPagoM { get; set; } //tabla tipo de pago
        public RemisioMPagoModel RemisioMPagoMNoList { get; set; }
        public List<RemisioDyMModel> RemisioDyMM { get; set; } // tabla remisioD productos
        public RemisioDyMModel RemisioDyMMNoList { get; set; }
        public List<RemisioMVendedorModel> RemisioMVendedorM { get; set; } //tabla vendedores
        public RemisioMVendedorModel RemisioMVendedorMNoList { get; set; }
    }
}