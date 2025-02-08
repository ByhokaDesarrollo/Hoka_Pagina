using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEstatus;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo;
using System.Data.Entity;

namespace hoka_cli.Context.EntityFramework
{
    public class DBCHokaIngresos : DbContext
    {
        private string _connection;

        public DBCHokaIngresos(string cadenaConexion)
            : base(cadenaConexion)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<DBCHokaIngresos>(null);
        }

        public DbSet<EnAlmacenCaratula> AlmacenCaratula { get; set; }
        public DbSet<EnAlmacenCaratulaEstatus> AlmacenCaratulaEstatus { get; set; }
        public DbSet<EnAlmacenCaratulaVenta> AlmacenCaratulaVenta { get; set; }
        public DbSet<EnAlmacenCaratulaVentaCategoria> AlmacenCaratulaVentaCategoria { get; set; }
        public DbSet<EnAlmacenCaratulaEfectivo> AlmacenCaratulaEfectivo { get; set; }
        public DbSet<EnAlmacenCaratulaEfectivoMoneda> AlmacenCaratulaEfectivoMoneda { get; set; }
        public DbSet<EnAlmacenCaratulaEfectivoDenominacion> AlmacenCaratulaEfectivoDenominacion { get; set; }
        public DbSet<EnAlmacenCaratulaVoucher> AlmacenCaratulaVoucher { get; set; }
        public DbSet<EnAlmacenCaratulaVoucherVoucher> AlmacenCaratulaVoucherVoucher { get; set; }
        public DbSet<EnAlmacenCaratulaVoucherRecibo> AlmacenCaratulaVoucherRecibo { get; set; }
    }
}
