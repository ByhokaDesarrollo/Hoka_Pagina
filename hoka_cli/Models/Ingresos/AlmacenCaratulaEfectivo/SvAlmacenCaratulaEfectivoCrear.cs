using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda.Denominacion;
using hoka_cli.Models.Utileria.BaseDatos;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class SvAlmacenCaratulaEfectivoCrear
    {
        private EsAlmacenCaratulaEfectivo _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaEfectivo _rpAlmacenCaratulaEfectivo;
        private RpAlmacenCaratulaEfectivoMoneda _rpAlmacenCaratulaEfectivoMoneda;
        private RpAlmacenCaratulaEfectivoDenominacion _rpAlmacenCaratulaEfectivoDenominacion;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaEfectivo Entidad;

        public SvAlmacenCaratulaEfectivoCrear(
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

        public void Crear()
        {
            ValidarEntidades();
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    Entidad.FechaCaptura = SvConsultarFechaActual.Consultar(0);
                    _rpAlmacenCaratulaEfectivo.InsertEntity(Entidad);
                    _rpAlmacenCaratulaEfectivo.SaveChanges();
                    foreach (var moneda in Entidad.Monedas)
                    {
                        moneda.AlmacenCaratulaEfectivoId = Entidad.AlmacenCaratulaEfectivoId;
                    }
                    _rpAlmacenCaratulaEfectivoMoneda.InsertEntities(Entidad.Monedas);
                    _rpAlmacenCaratulaEfectivoMoneda.SaveChanges();
                    foreach(var moneda in Entidad.Monedas)
                    {
                        foreach (var denominacion in moneda.Denominaciones)
                        {
                            denominacion.AlmacenCaratulaEfectivoMonedaId = moneda.AlmacenCaratulaEfectivoMonedaId;
                        }
                        _rpAlmacenCaratulaEfectivoDenominacion.InsertEntities(moneda.Denominaciones);
                        _rpAlmacenCaratulaEfectivoDenominacion.SaveChanges();
                    }

                    _caratula.EstatusCaratulaEfectivo = 2;
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
    }
}
