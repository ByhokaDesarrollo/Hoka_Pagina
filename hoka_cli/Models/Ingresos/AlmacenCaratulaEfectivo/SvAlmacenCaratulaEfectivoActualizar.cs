using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class SvAlmacenCaratulaEfectivoActualizar
    {
        private EsAlmacenCaratulaEfectivo _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaEfectivo _rpAlmacenCaratulaEfectivo;
        private RpAlmacenCaratulaEfectivoMoneda _rpAlmacenCaratulaEfectivoMoneda;
        private RpAlmacenCaratulaEfectivoDenominacion _rpAlmacenCaratulaEfectivoDenominacion;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaEfectivo Entidad;
        public EnAlmacenCaratulaEfectivo EntidadBaseDatos;
        public ICollection<EnAlmacenCaratulaEfectivoMoneda> EntidadesMonedaActualizar;
        public ICollection<EnAlmacenCaratulaEfectivoDenominacion> EntidadesDenominacionActualizar;

        public SvAlmacenCaratulaEfectivoActualizar(
            EsAlmacenCaratulaEfectivo esAlmacenCaratulaEfectivo)
        {
            _estructuraEntidad = esAlmacenCaratulaEfectivo ??
                throw new SvAlmacenCaratulaEfectivoExceptionCrear(
                    "Se requiere la estructura EsAlmacenCaratulaEfectivo");
            Entidad = esAlmacenCaratulaEfectivo?.CaratulaEfectivo ??
                throw new SvAlmacenCaratulaEfectivoExceptionCrear(
                    "Se requiere el encabezado CaratulaEfectivo en EsAlmacenCaratulaEfectivo.AlmacenCaratulaEfectivo");

            _dbcHokaIngresos = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
            _rpAlmacenCaratula =
                SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _caratula = _rpAlmacenCaratula.Get(Entidad.AlmacenCaratulaId) ??
                throw new SvAlmacenCaratulaException(
                    "No se ha definido la Caratula a capturar");
            _rpAlmacenCaratulaEfectivo =
                SvAlmacenCaratulaEfectivoIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _rpAlmacenCaratulaEfectivoMoneda =
                SvAlmacenCaratulaEfectivoMonedaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _rpAlmacenCaratulaEfectivoDenominacion =
                SvAlmacenCaratulaEfectivoDenominacionIniciarRepositorio.IniciarRepositorio(
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
                    _rpAlmacenCaratulaEfectivo.UpdateEntity(EntidadBaseDatos);
                    _rpAlmacenCaratulaEfectivo.SaveChanges();
                    _rpAlmacenCaratulaEfectivoMoneda.UpdateEntities(EntidadesMonedaActualizar);
                    _rpAlmacenCaratulaEfectivoMoneda.SaveChanges();
                    _rpAlmacenCaratulaEfectivoDenominacion.UpdateEntities(EntidadesDenominacionActualizar);
                    _rpAlmacenCaratulaEfectivoDenominacion.SaveChanges();

                    _rpAlmacenCaratula.UpdateEntity(_caratula);
                    _rpAlmacenCaratula.SaveChanges();

                    Transaccion.Commit();

                    EntidadBaseDatos.Monedas = EntidadesMonedaActualizar;
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
            if (Entidad.Monedas?.Count == 0)
                throw new SvAlmacenCaratulaEfectivoMonedaExceptionCrear(
                    "Se requiere el detalle Monedas en " +
                    "EsAlmacenCaratulaEfectivo.AlmacenCaratulaEfectivo.Monedas");

            foreach (var moneda in Entidad.Monedas)
            {
                if (moneda.Denominaciones?.Count == 0)
                {
                    throw new SvAlmacenCaratulaEfectivoDenominacionExceptionCrear(
                        "Se requiere el detalle Monedas en " +
                        "EsAlmacenCaratulaEfectivo.AlmacenCaratulaEfectivo.Monedas.MonedaDenominaciones");
                }
            }
        }

        public void ObtenerEntidades()
        {
            EntidadBaseDatos = _rpAlmacenCaratulaEfectivo.Get(Entidad.AlmacenCaratulaEfectivoId);
            EntidadesMonedaActualizar = new HashSet<EnAlmacenCaratulaEfectivoMoneda>();
            EntidadesDenominacionActualizar = new HashSet<EnAlmacenCaratulaEfectivoDenominacion>();
        }

        public void MapearEntidades()
        {
            if (Entidad.ImporteTotal != EntidadBaseDatos.ImporteTotal)
            {
                EntidadBaseDatos.ImporteTotal = Entidad.ImporteTotal;
                EntidadBaseDatos.FechaCaptura = SvConsultarFechaActual.Consultar(0);
            }

            foreach (var moneda in Entidad.Monedas)
            {
                EnAlmacenCaratulaEfectivoMoneda EntidadMonedaBaseDatos =
                    _rpAlmacenCaratulaEfectivoMoneda.Get(moneda.AlmacenCaratulaEfectivoMonedaId);
                if (EntidadMonedaBaseDatos != null)
                {
                    bool B_AgregarMoneda = false;
                    if (moneda.TipoCambio != EntidadMonedaBaseDatos.TipoCambio)
                    {
                        EntidadMonedaBaseDatos.TipoCambio = moneda.TipoCambio;
                        B_AgregarMoneda = true;
                    }
                    if (moneda.ImporteTotal != EntidadMonedaBaseDatos.ImporteTotal)
                    {
                        EntidadMonedaBaseDatos.ImporteTotal = moneda.ImporteTotal;
                        B_AgregarMoneda = true;
                    }
                    if (moneda.ImporteTotalMXN != EntidadMonedaBaseDatos.ImporteTotalMXN)
                    {
                        EntidadMonedaBaseDatos.ImporteTotalMXN = moneda.ImporteTotalMXN;
                        B_AgregarMoneda = true;
                    }
                    if (B_AgregarMoneda)
                    { 
                        EntidadesMonedaActualizar.Add(EntidadMonedaBaseDatos);
                        Expression<Func<EnAlmacenCaratulaEfectivoDenominacion, bool>> predicate =
                            q =>
                            q.AlmacenCaratulaEfectivoMonedaId == moneda.AlmacenCaratulaEfectivoMonedaId;
                        ICollection<EnAlmacenCaratulaEfectivoDenominacion> EntidadesDenominacionBaseDatos =
                            _rpAlmacenCaratulaEfectivoDenominacion.Find(predicate).ToHashSet();
                        foreach (var denominacionBaseDatos in EntidadesDenominacionBaseDatos)
                        {
                            EnAlmacenCaratulaEfectivoDenominacion EntidadDenominacionAuxiliar =
                                moneda.Denominaciones
                                    .Where(
                                        q =>
                                        q.AlmacenCaratulaEfectivoDenominacionId == denominacionBaseDatos.AlmacenCaratulaEfectivoDenominacionId)
                                    .Single();
                            bool B_AgregarDenominacion = false;
                            if (EntidadDenominacionAuxiliar.Cantidad != denominacionBaseDatos.Cantidad)
                            {
                                denominacionBaseDatos.Cantidad = EntidadDenominacionAuxiliar.Cantidad;
                                B_AgregarDenominacion = true;
                            }
                            if (EntidadDenominacionAuxiliar.Importe != denominacionBaseDatos.Importe)
                            {
                                denominacionBaseDatos.Importe = EntidadDenominacionAuxiliar.Importe;
                                B_AgregarDenominacion = true;
                            }
                            if (EntidadDenominacionAuxiliar.ImporteMXN != denominacionBaseDatos.ImporteMXN)
                            {
                                denominacionBaseDatos.ImporteMXN = EntidadDenominacionAuxiliar.ImporteMXN;
                                B_AgregarDenominacion = true;
                            }
                            if (B_AgregarDenominacion)
                                EntidadesDenominacionActualizar.Add(denominacionBaseDatos);
                        }
                    }
                }
            }
        }
    }
}
