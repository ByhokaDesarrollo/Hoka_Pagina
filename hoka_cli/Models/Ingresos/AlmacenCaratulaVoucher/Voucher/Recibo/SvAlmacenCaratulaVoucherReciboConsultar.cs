using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using static System.Data.Entity.Infrastructure.Design.Executor;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVoucher.Voucher.Recibo
{
    public class SvAlmacenCaratulaVoucherReciboConsultar
    {
        private RpAlmacenCaratulaVoucherRecibo _rpAlmacenCaratulaVoucherRecibo;
        private EsAlmacenCaratulaVoucherRecibo _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaVoucherRecibo> _entidades;
        private readonly string _rutaBase = ConfigurationManager.AppSettings["RutaAlmacenCaratulaVoucherRecibo"];

        public SvAlmacenCaratulaVoucherReciboConsultar(
            RpAlmacenCaratulaVoucherRecibo repositorio,
            EsAlmacenCaratulaVoucherRecibo esAlmacenCaratulaVoucherRecibo)
        {
            _rpAlmacenCaratulaVoucherRecibo = repositorio;
            _estructuraEntidad = esAlmacenCaratulaVoucherRecibo;
            _entidades = new HashSet<EnAlmacenCaratulaVoucherRecibo>();
        }

        public ICollection<EnAlmacenCaratulaVoucherRecibo> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            if (_estructuraEntidad.ConsultarArchivo)
            {
                ConsultarEvidencia();
            }

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad?.Consulta ?? null;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaVoucherRecibo FiltroEntidad = _estructuraEntidad?.Recibo ?? null;
            IEnumerable<EnAlmacenCaratulaVoucherRecibo> Query = _rpAlmacenCaratulaVoucherRecibo._dbSet;
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaVoucherReciboId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaVoucherReciboId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVoucherReciboId == FiltroEntidad.AlmacenCaratulaVoucherReciboId);
                if (FiltroEntidad.AlmacenCaratulaVoucherVoucherId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVoucherVoucherId == FiltroEntidad.AlmacenCaratulaVoucherVoucherId);
                if (FiltroEntidad.Consecutivo > 0)
                    Query = Query.Where(q => q.Consecutivo == FiltroEntidad.Consecutivo);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaVoucherReciboId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            // Insertar Codigo
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            //foreach (var entidad in _entidades)
            //{
            //    Insertar Codigo
            //}
        }

        private void ConsultarEvidencia()
        {
            foreach (var entidad in _entidades)
            {
                string archivoNombre = Directory.GetFiles(_rutaBase, $"{entidad.AlmacenCaratulaVoucherReciboId}_*").FirstOrDefault();
                if (!string.IsNullOrEmpty(archivoNombre))
                { 
                    string archivoRuta = Path.Combine(_rutaBase, archivoNombre);
                    string archivoNombreWeb = Path.GetFileName(archivoRuta);
                    string archivoExtension = Path.GetExtension(archivoRuta);
                    //byte[] archivoBytes = File.ReadAllBytes(archivoRuta);

                    //entidad.Archivo = Convert.ToBase64String(archivoBytes);
                    entidad.ArchivoNombre = archivoNombreWeb;
                    entidad.ArchivoTipo = archivoExtension;
                }
            }
        }
    }
}
