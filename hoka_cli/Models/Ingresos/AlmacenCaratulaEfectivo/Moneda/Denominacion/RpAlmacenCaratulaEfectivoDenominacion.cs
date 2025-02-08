using hoka_cli.Context.EntityFramework.Entities;
using System.Data.Entity;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion
{
    public class RpAlmacenCaratulaEfectivoDenominacion : EnWorkRepository<EnAlmacenCaratulaEfectivoDenominacion>
    {
        public RpAlmacenCaratulaEfectivoDenominacion(DbContext context) : base(context) { }
    }
}
