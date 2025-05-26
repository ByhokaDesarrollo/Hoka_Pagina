using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using hoka_cli.Models.Ingresos.AlmacenCaratula;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo;
using hoka_cli.Models.Utileria.BaseDatos;
using System;
using System.Configuration;
using System.IO;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher
{
    public class SvAlmacenCaratulaVoucherCrear
    {
        private EsAlmacenCaratulaVoucher _estructuraEntidad;
        private DBCHokaIngresos _dbcHokaIngresos;
        private RpAlmacenCaratula _rpAlmacenCaratula;
        private RpAlmacenCaratulaVoucher _rpAlmacenCaratulaVoucher;
        private RpAlmacenCaratulaVoucherVoucher _rpAlmacenCaratulaVoucherVoucher;
        private RpAlmacenCaratulaVoucherRecibo _rpAlmacenCaratulaVoucherRecibo;
        private EnAlmacenCaratula _caratula;
        public EnAlmacenCaratulaVoucher Entidad;
        private readonly string _rutaBase = ConfigurationManager.AppSettings["RutaAlmacenCaratulaVoucherRecibo"];

        public SvAlmacenCaratulaVoucherCrear(
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

        public void Crear()
        {
            ValidarEntidades();
            MapearEntidades();
            using (var Transaccion = _dbcHokaIngresos.Database.BeginTransaction())
            {
                try
                {
                    Entidad.FechaCaptura = SvConsultarFechaActual.Consultar(0);
                    _rpAlmacenCaratulaVoucher.InsertEntity(Entidad);
                    _rpAlmacenCaratulaVoucher.SaveChanges();
                    foreach (var voucher in Entidad.Vouchers)
                    {
                        voucher.AlmacenCaratulaVoucherId = Entidad.AlmacenCaratulaVoucherId;
                    }
                    _rpAlmacenCaratulaVoucherVoucher.InsertEntities(Entidad.Vouchers);
                    _rpAlmacenCaratulaVoucherVoucher.SaveChanges();
                    foreach (var voucher in Entidad.Vouchers)
                    {
                        if (voucher.Recibos?.Count > 0)
                        {
                            foreach (var denominacion in voucher.Recibos)
                            {
                                denominacion.AlmacenCaratulaVoucherVoucherId = voucher.AlmacenCaratulaVoucherVoucherId;
                            }
                            _rpAlmacenCaratulaVoucherRecibo.InsertEntities(voucher.Recibos);
                            _rpAlmacenCaratulaVoucherRecibo.SaveChanges();
                        }
                    }

                    _caratula.EstatusCaratulaVoucher = 2;
                    _caratula.AlmacenCaratulaEstatusId = 2;
                    _caratula.B_ConsultarCaratula = true;
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
            bool b_Recibos = Entidad.Vouchers.Any(v => v.Recibos.Any(r => !string.IsNullOrEmpty(r.Archivo)));
            if (b_Recibos)
            {
                try
                {
                    var recibosValidos = Entidad.Vouchers.Where(x => x.Recibos != null && x.Recibos.Count > 0);
                    var vouchersEvidencia = recibosValidos.Where(x => x.Recibos.Any(y => !string.IsNullOrEmpty(y.Archivo)));
                    foreach (var voucher in vouchersEvidencia)
                    {
                        var recibosEvidencia = voucher.Recibos.Where(x => x.Archivo != null);
                        foreach (var recibo in recibosEvidencia)
                        {
                            if (!string.IsNullOrEmpty(recibo.Archivo))
                            {
                                string base64Data = recibo.Archivo.Contains(",")
                                    ? recibo.Archivo.Split(',')[1]
                                    : recibo.Archivo;
                                byte[] archivoBytes = Convert.FromBase64String(base64Data);

                                if (!Directory.Exists(_rutaBase))
                                    Directory.CreateDirectory(_rutaBase);

                                string nombreArchivo = $"{recibo.AlmacenCaratulaVoucherReciboId}_{recibo.ArchivoNombre}";
                                string rutaArchivo = Path.Combine(_rutaBase, nombreArchivo);
                                File.WriteAllBytes(rutaArchivo, archivoBytes);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    string mensaje = e.InnerException != null ? e.InnerException.Message : e.Message;
                    throw new Exception(mensaje);
                }
            }
        }

        public void ValidarEntidades()
        {
            if (Entidad.Vouchers?.Count == 0)
                throw new SvAlmacenCaratulaVoucherVoucherExceptionCrear(
                    "Se requiere el detalle Vouchers en " +
                    "EsAlmacenCaratulaVoucher.AlmacenCaratulaVoucher.Vouchers");

            foreach (var voucher in Entidad.Vouchers)
            {
                if (voucher.Recibos?.Count == 0)
                {
                    throw new SvAlmacenCaratulaVoucherReciboExceptionCrear(
                        "Se requiere el detalle Vouchers en " +
                        "EsAlmacenCaratulaVoucher.AlmacenCaratulaVoucher.Vouchers.Recibos");
                }
            }
        }

        public void MapearEntidades() 
        {
            // Insertar codigo
        }
    }
}
