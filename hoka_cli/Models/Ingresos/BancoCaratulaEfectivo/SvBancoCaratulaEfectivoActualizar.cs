using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace hoka_cli.Models.Ingresos.BancoCaratulaEfectivo
{
    public class SvBancoCaratulaEfectivoActualizar
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

        public SvBancoCaratulaEfectivoActualizar(
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
            if (Entidad.ImporteTotalBanco != EntidadBaseDatos.ImporteTotalBanco)
            {
                var fechaActual = SvConsultarFechaActual.Consultar(0);
                if (EntidadBaseDatos.FechaCapturaBanco == null)
                    EntidadBaseDatos.FechaCapturaBanco = fechaActual;
                if (EntidadBaseDatos.FechaCapturaBanco != null)
                    EntidadBaseDatos.FechaActualizacionBanco = fechaActual;
                EntidadBaseDatos.ImporteTotalBanco = Entidad.ImporteTotalBanco;
            }
            if (Entidad.ImporteTotalDiferencia != EntidadBaseDatos.ImporteTotalDiferencia)
            {
                EntidadBaseDatos.ImporteTotalDiferencia = Entidad.ImporteTotalDiferencia;
            }

            foreach (var moneda in Entidad.Monedas)
            {
                EnAlmacenCaratulaEfectivoMoneda EntidadMonedaBaseDatos =
                    _rpAlmacenCaratulaEfectivoMoneda.Get(moneda.AlmacenCaratulaEfectivoMonedaId);
                if (EntidadMonedaBaseDatos != null)
                {
                    bool B_AgregarMoneda = false;
                    if (moneda.ImporteTotalBanco != EntidadMonedaBaseDatos.ImporteTotalBanco)
                    {
                        EntidadMonedaBaseDatos.ImporteTotalBanco = moneda.ImporteTotalBanco;
                        B_AgregarMoneda = true;
                    }
                    if (moneda.ImporteTotalMXNBanco != EntidadMonedaBaseDatos.ImporteTotalMXNBanco)
                    {
                        EntidadMonedaBaseDatos.ImporteTotalMXNBanco = moneda.ImporteTotalMXNBanco;
                        B_AgregarMoneda = true;
                    }

                    if (moneda.ImporteTotalDiferencia != EntidadMonedaBaseDatos.ImporteTotalDiferencia)
                    {
                        EntidadMonedaBaseDatos.ImporteTotalDiferencia = moneda.ImporteTotalDiferencia;
                        B_AgregarMoneda = true;
                    }
                    if (moneda.ImporteTotalMXNDiferencia != EntidadMonedaBaseDatos.ImporteTotalMXNDiferencia)
                    {
                        EntidadMonedaBaseDatos.ImporteTotalMXNDiferencia = moneda.ImporteTotalMXNDiferencia;
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
                            if (EntidadDenominacionAuxiliar.CantidadBanco != denominacionBaseDatos.CantidadBanco)
                            {
                                denominacionBaseDatos.CantidadBanco = EntidadDenominacionAuxiliar.CantidadBanco;
                                B_AgregarDenominacion = true;
                            }
                            if (EntidadDenominacionAuxiliar.ImporteBanco != denominacionBaseDatos.ImporteBanco)
                            {
                                denominacionBaseDatos.ImporteBanco = EntidadDenominacionAuxiliar.ImporteBanco;
                                B_AgregarDenominacion = true;
                            }
                            if (EntidadDenominacionAuxiliar.ImporteMXNBanco != denominacionBaseDatos.ImporteMXNBanco)
                            {
                                denominacionBaseDatos.ImporteMXNBanco = EntidadDenominacionAuxiliar.ImporteMXNBanco;
                                B_AgregarDenominacion = true;
                            }

                            if (EntidadDenominacionAuxiliar.ImporteDiferencia != denominacionBaseDatos.ImporteDiferencia)
                            {
                                denominacionBaseDatos.ImporteDiferencia = EntidadDenominacionAuxiliar.ImporteDiferencia;
                                B_AgregarDenominacion = true;
                            }
                            if (EntidadDenominacionAuxiliar.ImporteMXNDiferencia != denominacionBaseDatos.ImporteMXNDiferencia)
                            {
                                denominacionBaseDatos.ImporteMXNDiferencia = EntidadDenominacionAuxiliar.ImporteMXNDiferencia;
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
