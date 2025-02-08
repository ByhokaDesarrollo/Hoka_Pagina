using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Categoria;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Ingresos.AlmacenCaratulaVenta.Categoria
{
    public class SvAlmacenCaratulaVentaCategoriaConsultar
    {
        private RpAlmacenCaratulaVentaCategoria _rpAlmacenCaratulaVentaCategoria;
        private EsAlmacenCaratulaVentaCategoria _estructuraEntidad;
        private ICollection<EnAlmacenCaratulaVentaCategoria> _entidades;

        public SvAlmacenCaratulaVentaCategoriaConsultar(
            RpAlmacenCaratulaVentaCategoria repositorio,
            EsAlmacenCaratulaVentaCategoria esAlmacenCaratulaVentaCategoria)
        {
            _rpAlmacenCaratulaVentaCategoria = repositorio;
            _estructuraEntidad = esAlmacenCaratulaVentaCategoria;
            _entidades = new HashSet<EnAlmacenCaratulaVentaCategoria>();
        }

        public ICollection<EnAlmacenCaratulaVentaCategoria> Consultar()
        {
            ObtenerEntidades();

            bool consultarPropiedades = ConsultarPropiedades();
            if (consultarPropiedades)
                ObtnerPropiedades();

            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad.Consulta;
            EnPaginacion FiltroPaginacion = _estructuraEntidad?.Consulta?.Paginacion ?? null;
            EnAlmacenCaratulaVentaCategoria FiltroEntidad = _estructuraEntidad.CaratulaVentaCategoria;
            IEnumerable<EnAlmacenCaratulaVentaCategoria> Query = _rpAlmacenCaratulaVentaCategoria._dbSet;
            if (FiltroConsulta != null)
            {
                // NA
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenCaratulaVentaCategoriaId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenCaratulaVentaCategoriaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVentaCategoriaId == FiltroEntidad.AlmacenCaratulaVentaCategoriaId);
                if (FiltroEntidad.AlmacenCaratulaVentaId > 0)
                    Query = Query.Where(q => q.AlmacenCaratulaVentaId == FiltroEntidad.AlmacenCaratulaVentaId);
                if (FiltroEntidad.CategoriaId > 0)
                    Query = Query.Where(q => q.CategoriaId == FiltroEntidad.CategoriaId);
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenCaratulaVentaCategoriaId);
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
                entidad.Categoria = _estructuraEntidad.B_ConsultarCategoria
                    ? ConsultarCategoria(entidad.CategoriaId)
                    : null;
            }
        }

        private EnCategoria ConsultarCategoria(int categoriaId)
        {
            RpCategoria rpCategoria = SvCategoriaIniciarRepositorio.IniciarRepositorio();
            EsCategoria esCategoria = new EsCategoria()
            {
                Categoria = new EnCategoria()
                {
                    CategoriaId = categoriaId
                },
                B_ConsultarCategoriaCero = true
            };
            SvCategoriaConsultar servicio = new SvCategoriaConsultar(rpCategoria, esCategoria);
            EnCategoria Categoria = servicio.Consultar().Single();
            return Categoria;
        }
    }
}
