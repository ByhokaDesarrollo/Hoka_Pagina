using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratula
{
    public class RpAlmacenCaratula : EnWorkRepository<EnAlmacenCaratula>
    {
        public RpAlmacenCaratula(DbContext context) : base(context) { }
    }
}
