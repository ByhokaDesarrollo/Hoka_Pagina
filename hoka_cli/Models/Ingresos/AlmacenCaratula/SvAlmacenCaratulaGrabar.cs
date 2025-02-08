using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using System;

namespace hoka_cli.Models.Ingresos.AlmacenCaratula
{
    public class SvAlmacenCaratulaGrabar
    {
        private DBCHokaIngresos _dbcHokaIngresos;
        private EsAlmacenCaratula _estructuraEntidad;
        private EnAlmacenCaratula _caratula;
        private RpAlmacenCaratula _rpAlmacenCaratula;

        public SvAlmacenCaratulaGrabar(
            EsAlmacenCaratula EsAlmacenCaratula)
        {
            _estructuraEntidad = EsAlmacenCaratula ??
                throw new SvAlmacenCaratulaExceptionGrabar(
                    "Se requiere la estructura EsAlmacenCaratula");
            _dbcHokaIngresos = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
            _rpAlmacenCaratula = SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio(_dbcHokaIngresos);
            _caratula = _rpAlmacenCaratula.Get(_estructuraEntidad.Caratula.AlmacenCaratulaId) ??
                throw new SvAlmacenCaratulaException("No se ha definido la Caratula a capturar");
        }

        public EnAlmacenCaratula Grabar()
        {
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    _caratula.AlmacenCaratulaEstatusId = 3;
                    _caratula.B_ConsultarCaratula = true;
                    _caratula.EstatusCaratulaVenta = 3;
                    _caratula.EstatusCaratulaEfectivo = 3;
                    _caratula.EstatusCaratulaVoucher = 3;
                    _rpAlmacenCaratula.UpdateEntity(_caratula);
                    _rpAlmacenCaratula.SaveChanges();

                    Transaccion.Commit();
                    return _caratula;
                }
                catch (Exception e)
                {
                    Transaccion.Rollback();
                    string mensaje = e.InnerException?.Message ?? e.Message;
                    throw new Exception(mensaje);
                }
            }
        }
    }
}
