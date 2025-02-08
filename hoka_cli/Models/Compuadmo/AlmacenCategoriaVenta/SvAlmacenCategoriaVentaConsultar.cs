using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Models.Compuadmo.Catalogo.Almacen;
using hoka_cli.Models.Compuadmo.Categoria;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta
{
    public class SvAlmacenCategoriaVentaConsultar
    {
        private RpAlmacenCategoriaVenta _rpAlmacenCategoriaVenta;
        private EsAlmacenCategoriaVenta _estructuraEntidad;
        private ICollection<EnAlmacenCategoriaVenta> _entidades;

        public SvAlmacenCategoriaVentaConsultar(
            RpAlmacenCategoriaVenta repositorio,
            EsAlmacenCategoriaVenta esAlmacenCategoriaVenta)
        {
            _rpAlmacenCategoriaVenta = repositorio;
            _estructuraEntidad = esAlmacenCategoriaVenta;
            _entidades = new HashSet<EnAlmacenCategoriaVenta>();
        }

        public ICollection<EnAlmacenCategoriaVenta> Consultar()
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
            EnAlmacenCategoriaVenta FiltroEntidad = _estructuraEntidad.AlmacenCategoriaVenta;
            int In_AlmacenId = FiltroEntidad?.AlmacenId ?? 0;
            int In_CategoriaId = FiltroEntidad?.CategoriaId ?? 0;
            string In_FechaInicio = FiltroConsulta?.FechaInicio != null ? $"'{FiltroConsulta.FechaInicioFormatoFecha}'" : "NULL";
            string In_FechaFin = FiltroConsulta?.FechaFin != null ? $"'{FiltroConsulta.FechaFinFormatoFecha}'" : "NULL";
            if (In_FechaInicio.Length == 0)
            { 
                In_FechaInicio = FiltroEntidad?.Fecha != null ? $"'{FiltroEntidad.FechaSoloFecha}'" : "NULL";
                In_FechaFin = FiltroEntidad?.Fecha != null ? $"'{FiltroEntidad.FechaSoloFecha}'" : "NULL";
            }
            string query =
                $"EXECUTE SP_AlmacenCategoriaVenta_GenerarVenta " +
                $"{In_FechaInicio}," +
                $"{In_FechaFin}," +
                $"{In_AlmacenId}," +
                $"{In_CategoriaId}";
            IEnumerable<EnAlmacenCategoriaVenta> Query = _rpAlmacenCategoriaVenta.FindByQuery(query);
            if (FiltroConsulta != null)
            {
                // Insertar Codigo
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                // No Aplica
            }
            if (FiltroEntidad != null)
            {
                // No Aplica
            }
            if (FiltroConsulta != null)
            {
                // No Aplica
            }
            _entidades = Query.ToList();
        }

        private bool ConsultarPropiedades()
        {
            bool resultado = false;
            if (_estructuraEntidad.B_ConsultarAlmacen ||
                _estructuraEntidad.B_ConsultarCategoria)
                resultado = true;
            return resultado;
        }

        private void ObtnerPropiedades()
        {
            foreach (var entidad in _entidades)
            {
                entidad.Almacen = _estructuraEntidad.B_ConsultarAlmacen
                    ? ConsultarAlmacen(entidad.AlmacenId)
                    : null;
                entidad.Categoria = _estructuraEntidad.B_ConsultarCategoria
                    ? ConsultarCategoria(entidad.CategoriaId)
                    : null;
            }
        }

        private EnCatalogoAlmacen ConsultarAlmacen(int almacenId)
        {
            RpCatalogoAlmacen rpCatalogoAlmacen = SvCatalogoAlmacenIniciarRepositorio.IniciarRepositorio();
            EsCatalogoAlmacen esCatalogoAlmacen = new EsCatalogoAlmacen()
            {
                CatalogoAlmacen = new EnCatalogoAlmacen()
                {
                    AlmacenId = almacenId
                }
            };
            SvCatalogoAlmacenConsultar servicio = new SvCatalogoAlmacenConsultar(rpCatalogoAlmacen, esCatalogoAlmacen);
            EnCatalogoAlmacen CatalogoAlmacen = servicio.Consultar().Single();
            return CatalogoAlmacen;
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
