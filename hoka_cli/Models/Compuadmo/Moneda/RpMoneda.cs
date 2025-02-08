using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Moneda
{
    public class RpMoneda : EnWorkRepository<EnMoneda>
    {
        public RpMoneda(DbContext context) : base(context) { }
    }
}
