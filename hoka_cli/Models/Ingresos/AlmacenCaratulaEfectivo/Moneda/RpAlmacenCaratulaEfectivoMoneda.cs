using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda
{
    public class RpAlmacenCaratulaEfectivoMoneda : EnWorkRepository<EnAlmacenCaratulaEfectivoMoneda>
    {
        public RpAlmacenCaratulaEfectivoMoneda(DbContext context) : base(context) { }
    }
}
