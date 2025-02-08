using hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta;
using System.Data.Entity;

namespace hoka_cli.Context.EntityFramework
{
    public class DBCHokaJoyeria : DbContext
    {
        private string _connection;

        public DBCHokaJoyeria(string cadenaConexion)
            : base(cadenaConexion)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<DBCHokaJoyeria>(null);
        }

        public DbSet<EnAlmacenCategoriaVenta> AlmacenCategoriaVenta { get; set; }
    }
}
