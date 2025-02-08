using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Moneda.Denominacion
{
    public class RpMonedaDenominacion : EnWorkRepository<EnMonedaDenominacion>
    {
        public RpMonedaDenominacion(DbContext context) : base(context) { }
    }
}
