using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo;
using System;
using System.Collections.Generic;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class SvAlmacenCaratulaVoucherActualizar
    {
        private EsAlmacenCaratulaVoucher _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaVoucher _rpAlmacenCaratulaVoucher;
        private RpAlmacenCaratulaVoucherVoucher _rpAlmacenCaratulaVoucherVoucher;
        private RpAlmacenCaratulaVoucherRecibo _rpAlmacenCaratulaVoucherRecibo;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVoucher Entidad;
        public EnAlmacenCaratulaVoucher EntidadBaseDatos;
        public ICollection<EnAlmacenCaratulaVoucherVoucher> EntidadesVoucherActualizar;
        public ICollection<EnAlmacenCaratulaVoucherRecibo> EntidadesReciboCrear;
        public ICollection<EnAlmacenCaratulaVoucherRecibo> EntidadesReciboActualizar;
        public ICollection<EnAlmacenCaratulaVoucherRecibo> EntidadesReciboEliminar;

        public SvAlmacenCaratulaVoucherActualizar(
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
            _rpAlmacenCaratulaVoucher =
                SvAlmacenCaratulaVoucherIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _rpAlmacenCaratulaVoucherVoucher =
                SvAlmacenCaratulaVoucherVoucherIniciarRepositorio.IniciarRepositorio(
                    dbContext: _dbcHokaIngresos);
            _rpAlmacenCaratulaVoucherRecibo =
                SvAlmacenCaratulaVoucherReciboIniciarRepositorio.IniciarRepositorio(
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
                    _rpAlmacenCaratulaVoucher.UpdateEntity(EntidadBaseDatos);
                    _rpAlmacenCaratulaVoucher.SaveChanges();
                    if (EntidadesVoucherActualizar?.Count > 0)
                    {
                        _rpAlmacenCaratulaVoucherVoucher.UpdateEntities(EntidadesVoucherActualizar);
                        _rpAlmacenCaratulaVoucherVoucher.SaveChanges();
                    }
                    if (EntidadesReciboCrear?.Count > 0)
                    {
                        _rpAlmacenCaratulaVoucherRecibo.InsertEntities(EntidadesReciboCrear);
                        _rpAlmacenCaratulaVoucherRecibo.SaveChanges();
                    }
                    if (EntidadesReciboActualizar?.Count > 0)
                    {
                        _rpAlmacenCaratulaVoucherRecibo.UpdateEntities(EntidadesReciboActualizar);
                        _rpAlmacenCaratulaVoucherRecibo.SaveChanges();
                    }
                    if (EntidadesReciboEliminar?.Count > 0)
                    {
                        _rpAlmacenCaratulaVoucherRecibo.DeleteEntities(EntidadesReciboEliminar);
                        _rpAlmacenCaratulaVoucherRecibo.SaveChanges();
                    }

                    _rpAlmacenCaratula.UpdateEntity(_caratula);
                    _rpAlmacenCaratula.SaveChanges();
                    Transaccion.Commit();
                }
                catch (Exception e)
                {
                    Transaccion.Rollback();
                    string mensaje = e.InnerException != null ? e.InnerException.Message : e.Message;
                    throw new SvAlmacenCaratulaVoucherExceptionActualizar(mensaje);
                }
            }
        }

        public void ValidarEntidades()
        {
            if (Entidad.Vouchers?.Count == 0)
                throw new SvAlmacenCaratulaVoucherVoucherExceptionActualizar(
                    "Se requiere el detalle Vouchers en " +
                    "EsAlmacenCaratulaVoucher.AlmacenCaratulaVoucher.Vouchers");

            foreach (var voucher in Entidad.Vouchers)
            {
                if (voucher.Recibos?.Count == 0)
                {
                    throw new SvAlmacenCaratulaVoucherReciboExceptionActualizar(
                        "Se requiere el detalle Vouchers en " +
                        "EsAlmacenCaratulaVoucher.AlmacenCaratulaVoucher.Vouchers.Recibos");
                }
            }
        }

        public void ObtenerEntidades()
        {
            EntidadBaseDatos = _rpAlmacenCaratulaVoucher.Get(Entidad.AlmacenCaratulaVoucherId) ??
                throw new SvAlmacenCaratulaVoucherExceptionActualizar(
                    $"No se encontron registros en la base de datos. " +
                    $"AlmacenCaratulaVoucherId: {Entidad.AlmacenCaratulaVoucherId}"
                );
            EntidadesVoucherActualizar = new HashSet<EnAlmacenCaratulaVoucherVoucher>();
            EntidadesReciboCrear = new HashSet<EnAlmacenCaratulaVoucherRecibo>();
            EntidadesReciboActualizar = new HashSet<EnAlmacenCaratulaVoucherRecibo>();
            EntidadesReciboEliminar = new HashSet<EnAlmacenCaratulaVoucherRecibo>();
        }

        public void MapearEntidades()
        {
            // AlmacenCaratulaVoucher
            if (Entidad.ImporteTotal != EntidadBaseDatos.ImporteTotal)
                EntidadBaseDatos.ImporteTotal = Entidad.ImporteTotal;
            // AlmacenCaratulaVoucherVoucher
            foreach (var voucher in Entidad.Vouchers)
            {
                EnAlmacenCaratulaVoucherVoucher voucherBaseDatos =
                    _rpAlmacenCaratulaVoucherVoucher.Get(voucher.AlmacenCaratulaVoucherVoucherId) ??
                    throw new SvAlmacenCaratulaVoucherVoucherExceptionActualizar(
                        $"No se encontron registros en la base de datos. " +
                        $"AlmacenCaratulaVoucherVoucherId: {voucher.AlmacenCaratulaVoucherVoucherId}"
                    );
                bool B_AgregarVoucher = false;
                if (voucher.TipoCambio > 0 &&
                    voucher.TipoCambio != voucherBaseDatos.TipoCambio)
                {
                    voucherBaseDatos.TipoCambio = voucher.TipoCambio;
                    B_AgregarVoucher = true;
                }
                if (voucher.ImporteTotal > 0 &&
                    voucher.ImporteTotal != voucherBaseDatos.ImporteTotal)
                {
                    voucherBaseDatos.ImporteTotal = voucher.ImporteTotal;
                    B_AgregarVoucher = true;
                }
                if (voucher.ImporteTotalMXN > 0 &&
                    voucher.ImporteTotalMXN != voucherBaseDatos.ImporteTotalMXN)
                {
                    voucherBaseDatos.ImporteTotalMXN = voucher.ImporteTotalMXN;
                    B_AgregarVoucher = true;
                }
                if (B_AgregarVoucher)
                {
                    EntidadesVoucherActualizar.Add(voucherBaseDatos);
                    foreach (var recibo in voucher.Recibos)
                    {
                        if (recibo.AlmacenCaratulaVoucherReciboId == 0)
                        {
                            if (recibo.Consecutivo <= 0)
                                throw new SvAlmacenCaratulaVoucherExceptionCrear(
                                    "El valor Consecutivo debe ser mayor a 0");
                            if (recibo.Importe <= 0)
                                throw new SvAlmacenCaratulaVoucherExceptionCrear(
                                    "El valor Importe debe ser mayor a 0");
                            if (recibo.ImporteMXN <= 0)
                                throw new SvAlmacenCaratulaVoucherExceptionCrear(
                                    "El valor ImporteMXN debe ser mayor a 0");
                            recibo.AlmacenCaratulaVoucherVoucherId = voucher.AlmacenCaratulaVoucherVoucherId;
                            EntidadesReciboCrear.Add(recibo);
                        }
                        if (recibo.AlmacenCaratulaVoucherReciboId > 0 && !recibo.B_Eliminar)
                        {
                            EnAlmacenCaratulaVoucherRecibo reciboBaseDatos =
                                _rpAlmacenCaratulaVoucherRecibo.Get(recibo.AlmacenCaratulaVoucherReciboId) ??
                                throw new SvAlmacenCaratulaVoucherReciboExceptionActualizar(
                                    $"No se encontron registros en la base de datos. " +
                                    $"AlmacenCaratulaVoucherReciboId: {recibo.AlmacenCaratulaVoucherReciboId}"
                                );
                            bool B_AgregarRecibo = false;
                            if (recibo.Consecutivo > 0 &&
                                recibo.Consecutivo != reciboBaseDatos.Consecutivo)
                            {
                                reciboBaseDatos.Consecutivo = recibo.Consecutivo;
                                B_AgregarRecibo = true;
                            }
                            if (recibo.Importe > 0 &&
                                recibo.Importe != reciboBaseDatos.Importe)
                            {
                                reciboBaseDatos.Importe = recibo.Importe;
                                B_AgregarRecibo = true;
                            }
                            if (recibo.ImporteMXN > 0 &&
                                recibo.ImporteMXN != reciboBaseDatos.ImporteMXN) {   
                                reciboBaseDatos.ImporteMXN = recibo.ImporteMXN;
                                B_AgregarRecibo = true;
                            }
                            if (B_AgregarRecibo)
                                EntidadesReciboActualizar.Add(reciboBaseDatos);
                        }
                        if (recibo.AlmacenCaratulaVoucherReciboId > 0 && recibo.B_Eliminar)
                        {
                            EnAlmacenCaratulaVoucherRecibo reciboBaseDatos =
                                _rpAlmacenCaratulaVoucherRecibo.Get(recibo.AlmacenCaratulaVoucherReciboId) ??
                                throw new SvAlmacenCaratulaVoucherReciboExceptionActualizar(
                                    $"No se encontron registros en la base de datos. " +
                                    $"AlmacenCaratulaVoucherReciboId: {recibo.AlmacenCaratulaVoucherReciboId}"
                                );
                            EntidadesReciboEliminar.Add(reciboBaseDatos);
                        }
                    }
                }
            }
        }
    }
}
