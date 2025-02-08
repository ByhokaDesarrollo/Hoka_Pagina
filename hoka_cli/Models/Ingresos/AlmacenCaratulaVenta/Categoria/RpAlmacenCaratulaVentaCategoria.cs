using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria
{
    public class RpAlmacenCaratulaVentaCategoria : EnWorkRepository<EnAlmacenCaratulaVentaCategoria>
    {
        public RpAlmacenCaratulaVentaCategoria(DbContext context) : base(context) { }
    }
}
