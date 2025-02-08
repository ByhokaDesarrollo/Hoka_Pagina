using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria;
using hoka_cli.Models.Utileria.BaseDatos;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class SvAlmacenCaratulaVentaCrear
    {
        private EsAlmacenCaratulaVenta _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaVenta _rpAlmacenCaratulaVenta;
        private RpAlmacenCaratulaVentaCategoria _rpAlmacenCaratulaVentaCategoria;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVenta Entidad;

        public SvAlmacenCaratulaVentaCrear(
            EsAlmacenCaratulaVenta esAlmacenCaratulaVenta)
        {
            _estructuraEntidad = esAlmacenCaratulaVenta ??
                throw new SvAlmacenCaratulaVentaExceptionCrear(
                    "Se requiere la estructura EsAlmacenCaratulaVenta");
            Entidad = esAlmacenCaratulaVenta?.CaratulaVenta ??
                throw new SvAlmacenCaratulaVentaExceptionCrear(
                    "Se requiere el encabezado CaratulaVenta en EsAlmacenCaratulaVenta.AlmacenCaratulaVenta");

            _dbcHokaIngresos = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
            _rpAlmacenCaratula =
                SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _caratula = _rpAlmacenCaratula.Get(Entidad.AlmacenCaratulaId) ??
                throw new SvAlmacenCaratulaException(
                    "No se ha definido la Caratula a capturar");
            _rpAlmacenCaratulaVenta =
                SvAlmacenCaratulaVentaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _rpAlmacenCaratulaVentaCategoria =
                SvAlmacenCaratulaVentaCategoriaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
        }

        public void Crear()
        {
            ValidarEntidades();
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    Entidad.FechaCaptura = SvConsultarFechaActual.Consultar(0);
                    _rpAlmacenCaratulaVenta.InsertEntity(Entidad);
                    _rpAlmacenCaratulaVenta.SaveChanges();
                    foreach (var categoriaVenta in Entidad.Categorias)
                    {
                        categoriaVenta.AlmacenCaratulaVentaId = Entidad.AlmacenCaratulaVentaId;
                    }
                    _rpAlmacenCaratulaVentaCategoria.InsertEntities(Entidad.Categorias);
                    _rpAlmacenCaratulaVentaCategoria.SaveChanges();

                    _caratula.UsuarioId = Entidad.UsuarioId;
                    _caratula.EstatusCaratulaVenta = 2;
                    _rpAlmacenCaratula.UpdateEntity(_caratula);
                    _rpAlmacenCaratula.SaveChanges();

                    Transaccion.Commit();
                }
                catch (Exception e)
                {
                    Transaccion.Rollback();
                    string mensaje = e.InnerException != null ? e.InnerException.Message : e.Message;
                    throw new Exception(mensaje);
                }
            }
        }

        public void ValidarEntidades()
        {
            if (Entidad.Categorias?.Count == 0)
                throw new SvAlmacenCaratulaVentaCategoriaExceptionCrear(
                    "Se requiere el detalle CategoriaVenta en EsAlmacenCaratulaVenta.AlmacenCaratulaVenta.CategoriasVenta");
        }
    }
}
