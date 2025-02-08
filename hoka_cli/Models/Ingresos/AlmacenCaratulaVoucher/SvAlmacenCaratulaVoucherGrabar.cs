using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class SvAlmacenCaratulaVoucherGrabar
    {
        private EsAlmacenCaratulaVoucher _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVoucher Entidad;

        public SvAlmacenCaratulaVoucherGrabar(
            EsAlmacenCaratulaVoucher esAlmacenCaratulaVoucher)
        {
            _estructuraEntidad = esAlmacenCaratulaVoucher ??
                throw new SvAlmacenCaratulaVoucherExceptionCrear(
                    "Se requiere la estructura EsAlmacenCaratulaVoucher");
            Entidad = esAlmacenCaratulaVoucher?.CaratulaVoucher ??
                throw new SvAlmacenCaratulaVoucherExceptionCrear(
                    "Se requiere el encabezado CaratulaVoucher en EsAlmacenCaratulaVoucher.AlmacenCaratulaVoucher");

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
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    _caratula.EstatusCaratulaVoucher = 3;
                    bool actualizarEstatusConsulta =
                        _caratula.EstatusCaratulaVenta == 3 &&
                        _caratula.EstatusCaratulaEfectivo == 3;
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
    }
}
