using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class SvAlmacenCaratulaVentaActualizar
    {
        private EsAlmacenCaratulaVenta _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaVenta _rpAlmacenCaratulaVenta;
        private RpAlmacenCaratulaVentaCategoria _rpAlmacenCaratulaVentaCategoria;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVenta Entidad;
        public EnAlmacenCaratulaVenta EntidadBaseDatos;
        public ICollection<EnAlmacenCaratulaVentaCategoria> EntidadesCategoriaActualizar;

        public SvAlmacenCaratulaVentaActualizar(
            EsAlmacenCaratulaVenta esAlmacenCaratulaVenta)
        {
            _estructuraEntidad = esAlmacenCaratulaVenta ??
                throw new SvAlmacenCaratulaVentaExceptionCrear(
                    "Se requiere la estructura EsAlmacenCaratulaVenta");
            Entidad = esAlmacenCaratulaVenta.CaratulaVenta ??
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

        public void Actualizar()
        {
            ValidarEntidades();
            ObtenerEntidades();
            MapearEntidades();
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    _rpAlmacenCaratulaVenta.UpdateEntity(EntidadBaseDatos);
                    _rpAlmacenCaratulaVenta.SaveChanges();
                    _rpAlmacenCaratulaVentaCategoria.UpdateEntities(EntidadesCategoriaActualizar);
                    _rpAlmacenCaratulaVentaCategoria.SaveChanges();

                    _rpAlmacenCaratula.UpdateEntity(_caratula);
                    _rpAlmacenCaratula.SaveChanges();

                    Transaccion.Commit();

                    EntidadBaseDatos.Categorias = EntidadesCategoriaActualizar;
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

        public void ObtenerEntidades()
        {
            EntidadBaseDatos = _rpAlmacenCaratulaVenta.Get(Entidad.AlmacenCaratulaVentaId);
            Expression<Func<EnAlmacenCaratulaVentaCategoria, bool>> predicate =
                q =>
                q.AlmacenCaratulaVentaId == EntidadBaseDatos.AlmacenCaratulaVentaId;
           
            EntidadesCategoriaActualizar = new HashSet<EnAlmacenCaratulaVentaCategoria>();
        }

        public void MapearEntidades()
        {
            if (Entidad.VentaSistemaTotal != EntidadBaseDatos.VentaSistemaTotal)
            {
                EntidadBaseDatos.VentaSistemaTotal = Entidad.VentaSistemaTotal;
                EntidadBaseDatos.FechaCaptura = SvConsultarFechaActual.Consultar(0);
            }
            if (Entidad.VentaNetaTotal != EntidadBaseDatos.VentaNetaTotal)
            {
                EntidadBaseDatos.VentaNetaTotal = Entidad.VentaNetaTotal;
                EntidadBaseDatos.FechaCaptura = SvConsultarFechaActual.Consultar(0);
            }
            if (Entidad.VentaDiferencia != EntidadBaseDatos.VentaDiferencia)
            {
                EntidadBaseDatos.VentaDiferencia = Entidad.VentaDiferencia;
                EntidadBaseDatos.FechaCaptura = SvConsultarFechaActual.Consultar(0);
            }

            foreach (var categoriaVenta in Entidad.Categorias)
            {
                EnAlmacenCaratulaVentaCategoria EntidadCategoriaBaseDatos =
                    _rpAlmacenCaratulaVentaCategoria.Get(categoriaVenta.AlmacenCaratulaVentaCategoriaId);
                if (EntidadCategoriaBaseDatos != null)
                {
                    bool B_Agregar = false;
                    if (categoriaVenta.VentaSistema != EntidadCategoriaBaseDatos.VentaSistema)
                    {
                        EntidadCategoriaBaseDatos.VentaSistema = categoriaVenta.VentaSistema;
                        B_Agregar = true;
                    }
                    if (categoriaVenta.VentaNeta != EntidadCategoriaBaseDatos.VentaNeta)
                    {
                        EntidadCategoriaBaseDatos.VentaNeta = categoriaVenta.VentaNeta;
                        B_Agregar = true;
                    }
                    if (categoriaVenta.Comision != EntidadCategoriaBaseDatos.Comision)
                    {
                        EntidadCategoriaBaseDatos.Comision = categoriaVenta.Comision;
                        B_Agregar = true;
                    }
                    if (B_Agregar)
                        EntidadesCategoriaActualizar.Add(EntidadCategoriaBaseDatos);
                }
            }
        }
    }
}
