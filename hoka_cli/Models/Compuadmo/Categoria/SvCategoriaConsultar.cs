using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Categoria
{
    public class SvCategoriaConsultar
    {
        private RpCategoria _rpCategoria;
        private EsCategoria _estructuraEntidad;
        private ICollection<EnCategoria> _entidades;

        public SvCategoriaConsultar(RpCategoria repositorio, EsCategoria esCategoria)
        {
            _rpCategoria = repositorio;
            _estructuraEntidad = esCategoria;
            _entidades = new HashSet<EnCategoria>();
        }

        public ICollection<EnCategoria> Consultar()
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
            EnCategoria FiltroEntidad = _estructuraEntidad.Categoria;
            IEnumerable<EnCategoria> Query = _rpCategoria._dbSet;
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.CategoriaId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (_estructuraEntidad.B_ConsultarCategoriaCero && FiltroEntidad.CategoriaId >= 0)
                    Query = Query.Where(q => q.CategoriaId == FiltroEntidad.CategoriaId);
                if (!_estructuraEntidad.B_ConsultarCategoriaCero && FiltroEntidad.CategoriaId > 0)
                    Query = Query.Where(q => q.CategoriaId == FiltroEntidad.CategoriaId);
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.DepartamentoPrefijo))
                    Query = Query.Where(q => q.DepartamentoPrefijo.ToUpper().Contains(FiltroEntidad.DepartamentoPrefijo.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.CategoriaId);
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
    }
}
