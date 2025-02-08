using hoka_cli.Models.Compuadmo.Almacen;
using hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka_cli.Models.Compuadmo.Catalogo.Almacen;
using hoka_cli.Models.Compuadmo.Categoria;
using hoka_cli.Models.Compuadmo.Moneda;
using hoka_cli.Models.Compuadmo.Moneda.Denominacion;
using hoka_cli.Models.Compuadmo.Moneda.Tipo;
using hoka_cli.Models.Compuadmo.Usuario;
using hoka_cli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka_cli.Models.Compuadmo.UsuarioPerfil;
using hoka_cli.Models.Compuadmo.UsuarioRol;
using hoka_cli.Models.Compuadmo.Voucher;
using System.Data.Entity;

namespace hoka_cli.Context.EntityFramework
{
    public class DBCHokaCompuadmo : DbContext
    {
        private string _connection;

        public DBCHokaCompuadmo(string cadenaConexion)
            : base(cadenaConexion)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<DBCHokaCompuadmo>(null);
        }

        #region Usuario

        public DbSet<EnUsuario> VW_Usuario { get; set; }
        public DbSet<EnUsuarioRol> VW_UsuarioRol { get; set; } // OK
        public DbSet<EnUsuarioPerfil> VW_UsuarioPerfil { get; set; }
        public DbSet<EnUsuarioPermisoAlmacenCaratula> UsuarioPermisoAlmacenCaratula { get; set; }

        #endregion

        public DbSet<EnAlmacen> Almacen { get; set; }
        public DbSet<EnCategoria> VW_Categoria { get; set; }
        public DbSet<EnAlmacenCategoriaVenta> AlmacenCategoriaVenta { get; set; }
        public DbSet<EnMoneda> Moneda { get; set; }
        public DbSet<EnMonedaTipo> MonedaTipo { get; set; }
        public DbSet<EnMonedaDenominacion> MonedaDenominacion { get; set; }
        public DbSet<EnVoucher> Voucher { get; set; }

        #region Catalogos

        public DbSet<EnCatalogoAlmacen> VW_CatalogoAlmacen { get; set; }

        #endregion
    }
}
