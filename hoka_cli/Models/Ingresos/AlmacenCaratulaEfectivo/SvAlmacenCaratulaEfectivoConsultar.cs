using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo.Moneda;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaEfectivo
{
    public class SvAlmacenCaratulaEfectivoConsultar
    {
        private RpAlmacenCaratulaEfectivo _rpAlmacenCaratulaEfectivo;
        private EsAlmacenCaratulaEfectivo _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaEfectivo> _entidades;

        public SvAlmacenCaratulaEfectivoConsultar(
            RpAlmacenCaratulaEfectivo repositorio,
            EsAlmacenCaratulaEfectivo esAlmacenCaratulaEfectivo)
        {
            _rpAlmacenCaratulaEfectivo = repositorio;
            _estructuraEntidad = esAlmacenCaratulaEfectivo;
            _entidades = new HashSet<EnAlmacenCaratulaEfectivo>();
        }

        public ICollection<EnAlmacenCaratulaEfectivo> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (_entidades?.Count > 0)
                if (consultarPropiedades)
                    ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad.Consulta;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaEfectivo FiltroEntidad = _estructuraEntidad.CaratulaEfectivo;
            IEnumerable<EnAlmacenCaratulaEfectivo> Query = _rpAlmacenCaratulaEfectivo._dbSet;
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.FechaInicio != null)
                    Query = Query.Where(q => q.FechaCaptura >= FiltroConsulta.FechaInicio);
                if (FiltroConsulta.FechaFin != null)
                    Query = Query.Where(q => q.FechaCaptura >= FiltroConsulta.FechaFin);
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaEfectivoId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaEfectivoId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaEfectivoId == FiltroEntidad.AlmacenCaratulaEfectivoId);
                if (FiltroEntidad.AlmacenCaratulaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaId == FiltroEntidad.AlmacenCaratulaId);
                if (FiltroEntidad.UsuarioId > 0)
                    Query = Query.Where(q => q.UsuarioId == FiltroEntidad.UsuarioId);
                if (FiltroEntidad.FechaCaptura != null)
                    Query = Query.Where(q => q.FechaCaptura?.ToString("yyyyMMdd") == FiltroEntidad.FechaCapturaFormatoFecha);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaEfectivoId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarMoneda)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                if (_estructuraEntidad.B_ConsultarMoneda)
                    entidad.Monedas = ConsultarMonedas(entidad.AlmacenCaratulaEfectivoId);
            }
        }

        private ICollection<EnAlmacenCaratulaEfectivoMoneda> ConsultarMonedas(int AlmacenCaratulaEfectivoId)
        {
            RpAlmacenCaratulaEfectivoMoneda rpAlmacenCaratulaEfectivoMoneda = SvAlmacenCaratulaEfectivoMonedaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaEfectivoMoneda esAlmacenCaratulaEfectivoMoneda = new EsAlmacenCaratulaEfectivoMoneda()
            {
                Moneda = new EnAlmacenCaratulaEfectivoMoneda()
                {
                    AlmacenCaratulaEfectivoId = AlmacenCaratulaEfectivoId
                },
                B_ConsultarMoneda = true,
                B_ConsultarMonedaDenominacion = true
            };
            SvAlmacenCaratulaEfectivoMonedaConsultar servicio =
                new SvAlmacenCaratulaEfectivoMonedaConsultar(rpAlmacenCaratulaEfectivoMoneda, esAlmacenCaratulaEfectivoMoneda);
            ICollection<EnAlmacenCaratulaEfectivoMoneda> AlmacenCaratulaEfectivoMonedas = servicio.Consultar();
            return AlmacenCaratulaEfectivoMonedas;
        }
    }
}
