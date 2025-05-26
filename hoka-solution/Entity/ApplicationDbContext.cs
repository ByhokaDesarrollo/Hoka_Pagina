using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using hoka.Entity.Data;

namespace hoka.Entity
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("CadenaConexionHokaCompuadmo")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        // DbSets
        public DbSet<Reembolso> Reembolsos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Almacen> Almacenes { get; set; }
        public DbSet<Conceptos> Conceptos { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Reembolso>().ToTable("Reembolsos");
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Reembolso>().Property(r => r.ImporteSinIva).HasPrecision(18, 2);
            modelBuilder.Entity<Reembolso>().Property(r => r.ImporteIva).HasPrecision(18, 2);
            modelBuilder.Entity<Reembolso>().Property(r => r.RetencionIsr).HasPrecision(18, 2);
            modelBuilder.Entity<Reembolso>().Property(r => r.RetencionIva).HasPrecision(18, 2);
            modelBuilder.Entity<Reembolso>().Property(r => r.Total).HasPrecision(18, 2);

            modelBuilder.Entity<Proveedor>().ToTable("provedores");
            modelBuilder.Entity<Proveedor>().Property(p => p.NombreRazonSocial).HasColumnName("nombre_rasonsocial");
            modelBuilder.Entity<Proveedor>().Property(p => p.ProvedorId).HasColumnName("provedor"); // Mapeo alternativo sin usar anotaciones 

            modelBuilder.Entity<Almacen>().ToTable("almacenes");
            modelBuilder.Entity<Almacen>().Property(a => a.IdAlmacen).HasColumnName("Almacen");
            modelBuilder.Entity<Almacen>().Property(a => a.Nombre).HasColumnName("Nombre");

            modelBuilder.Entity<Conceptos>().ToTable("Conceptos");
            modelBuilder.Entity<Conceptos>().Property(c => c.Id).HasColumnName("id");
            modelBuilder.Entity<Conceptos>().Property(c => c.TipoConcepto).HasColumnName("TipoConcepto");
            modelBuilder.Entity<Conceptos>().Property(c => c.Concepto).HasColumnName("Concepto");

            base.OnModelCreating(modelBuilder);
        }
    }
}
