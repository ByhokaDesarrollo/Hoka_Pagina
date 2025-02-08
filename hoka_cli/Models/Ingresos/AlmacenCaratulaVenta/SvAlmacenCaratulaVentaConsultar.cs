using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta
{
    public class SvAlmacenCaratulaVentaConsultar
    {
        private RpAlmacenCaratulaVenta _rpAlmacenCaratulaVenta;
        private EsAlmacenCaratulaVenta _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaVenta> _entidades;

        public SvAlmacenCaratulaVentaConsultar(
            RpAlmacenCaratulaVenta repositorio,
            EsAlmacenCaratulaVenta esAlmacenCaratulaVenta)
        {
            _rpAlmacenCaratulaVenta = repositorio;
            _estructuraEntidad = esAlmacenCaratulaVenta;
            _entidades = new HashSet<EnAlmacenCaratulaVenta>();
        }

        public ICollection<EnAlmacenCaratulaVenta> Consultar()
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
            EnAlmacenCaratulaVenta FiltroEntidad = _estructuraEntidad.CaratulaVenta;
            IEnumerable<EnAlmacenCaratulaVenta> Query = _rpAlmacenCaratulaVenta._dbSet;
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
                    Query = Query.Where(x => x.AlmacenCaratulaVentaId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaVentaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVentaId == FiltroEntidad.AlmacenCaratulaVentaId);
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
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaVentaId);
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarCategoria)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                if (_estructuraEntidad.B_ConsultarCategoria)
                    entidad.Categorias = ConsultarCategoriasVenta(entidad.AlmacenCaratulaVentaId);
            }
        }

        private ICollection<EnAlmacenCaratulaVentaCategoria> ConsultarCategoriasVenta(int almacenCaratulaVentaId)
        {
            RpAlmacenCaratulaVentaCategoria rpAlmacenCaratulaVentaCategoria = SvAlmacenCaratulaVentaCategoriaIniciarRepositorio.IniciarRepositorio();
            EsAlmacenCaratulaVentaCategoria esAlmacenCaratulaVentaCategoria = new EsAlmacenCaratulaVentaCategoria()
            {
                CaratulaVentaCategoria = new EnAlmacenCaratulaVentaCategoria()
                {
                    AlmacenCaratulaVentaId = almacenCaratulaVentaId
                },
                B_ConsultarCategoria = true
            };
            SvAlmacenCaratulaVentaCategoriaConsultar servicio =
                new SvAlmacenCaratulaVentaCategoriaConsultar(rpAlmacenCaratulaVentaCategoria, esAlmacenCaratulaVentaCategoria);
            ICollection<EnAlmacenCaratulaVentaCategoria> AlmacenCaratulaVentaCategorias = servicio.Consultar();
            return AlmacenCaratulaVentaCategorias;
        }
    }
}
