using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class SvAlmacenCaratulaVentaGrabar
    {
        private EsAlmacenCaratulaVenta _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVenta Entidad;

        public SvAlmacenCaratulaVentaGrabar(
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
        }

        public void Grabar()
        {
            ValidarEntidades();
            ObtenerEntidades();
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    _caratula.EstatusCaratulaVenta = 3;
                    bool actualizarEstatusConsulta =
                        _caratula.EstatusCaratulaEfectivo == 3 &&
                        _caratula.EstatusCaratulaVoucher == 3;
                    if (actualizarEstatusConsulta)
                        _caratula.AlmacenCaratulaEstatusId = 3;
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
            // NA
        }

        public void ObtenerEntidades()
        {
            // NA
        }
    }
}
