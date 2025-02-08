using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Compuadmo.Moneda.Tipo
{
    public class RpMonedaTipo : EnWorkRepository<EnMonedaTipo>
    {
        public RpMonedaTipo(DbContext context) : base(context) { }
    }
}
