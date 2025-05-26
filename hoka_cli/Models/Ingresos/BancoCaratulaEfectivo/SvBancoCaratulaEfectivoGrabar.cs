using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo;
using System;

namespace hoka_cli.Models.Ingresos.BancoCaratulaEfectivo
{
    public class SvBancoCaratulaEfectivoGrabar
    {
        private EsAlmacenCaratulaEfectivo _estructuraEntidad;
        private DBCHokaIngresos _dbCHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaEfectivo Entidad;

        public SvBancoCaratulaEfectivoGrabar(
            EsAlmacenCaratulaEfectivo esAlmacenCaratulaEfectivo)
        {
            _estructuraEntidad = esAlmacenCaratulaEfectivo ??
                throw new SvAlmacenCaratulaEfectivoExceptionCrear(
                    "Se requiere la estructura EsAlmacenCaratulaEfectivo");
            Entidad = esAlmacenCaratulaEfectivo?.CaratulaEfectivo ??
                throw new SvAlmacenCaratulaEfectivoExceptionCrear(
                    "Se requiere el encabezado CaratulaEfectivo en EsAlmacenCaratulaEfectivo.AlmacenCaratulaEfectivo");

            _dbCHokaIngresos = SvEstablecerConexionDBCHokaIngresos.EstablecerConexion();
            _rpAlmacenCaratula =
                SvAlmacenCaratulaIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbCHokaIngresos);
            _caratula = _rpAlmacenCaratula.Get(Entidad.AlmacenCaratulaId) ??
                throw new SvAlmacenCaratulaException(
                    "No se ha definido la Caratula a capturar");
        }

        public void Grabar()
        {
            ValidarEntidades();
            ObtenerEntidades();
            using (var Transaccion = _dbCHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    _caratula.EstatusCaratulaEfectivo = 4;
                    bool actualizarEstatusConsulta =
                        _caratula.EstatusCaratulaVenta == 3 &&
                        _caratula.EstatusCaratulaVoucher == 3 &&
                        _caratula.EstatusCaratulaEfectivo == 3;
                    if (actualizarEstatusConsulta)
                        _caratula.AlmacenCaratulaEstatusId = 4;
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
