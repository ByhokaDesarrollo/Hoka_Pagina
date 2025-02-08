using hoka_cli.Models.Compuadmo.AlmacenCategoriaVenta;
using hoka_cli.Models.Compuadmo.Catalogo.Almacen;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Linq;

namespace hoka_cli.Models.Joyeria.AlmacenCategoriaVenta
{
    public class SvJoyeriaAlmacenCategoriaVentaConsultar
    {
        private RpAlmacenCategoriaVenta _rpAlmacenCategoriaVenta;
        private EsAlmacenCategoriaVenta _estructuraEntidad;
        private ICollection<EnAlmacenCategoriaVenta> _entidades;

        public SvJoyeriaAlmacenCategoriaVentaConsultar(
            RpAlmacenCategoriaVenta repositorio,
            EsAlmacenCategoriaVenta esAlmacenCategoriaVenta)
        {
            _rpAlmacenCategoriaVenta = repositorio;
            _estructuraEntidad = esAlmacenCategoriaVenta;
            _entidades = new HashSet<EnAlmacenCategoriaVenta>();
        }

        public ICollection<EnAlmacenCategoriaVenta> Consultar()
        {
            ConsultarAlmacen();
            ObtenerEntidades();
            return _entidades;
        }

        private void ObtenerEntidades()
        {
            EsConsulta FiltroConsulta = _estructuraEntidad.Consulta;
            EnAlmacenCategoriaVenta FiltroEntidad = _estructuraEntidad.AlmacenCategoriaVenta;
            int In_AlmacenId = FiltroEntidad?.AlmacenId ?? 0;
            string In_AlmacenPrefijo = FiltroEntidad?.Almacen?.Prefijo ?? "";
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
                $"{In_AlmacenPrefijo}";
            IEnumerable<EnAlmacenCategoriaVenta> Query = _rpAlmacenCategoriaVenta.FindByQuery(query);
            _entidades = Query.ToList();
        }

        private void ConsultarAlmacen()
        {
            RpCatalogoAlmacen rpCatalogoAlmacen = SvCatalogoAlmacenIniciarRepositorio.IniciarRepositorio();
            EsCatalogoAlmacen esCatalogoAlmacen = new EsCatalogoAlmacen()
            {
                CatalogoAlmacen = new EnCatalogoAlmacen()
                {
                    AlmacenId = _estructuraEntidad.AlmacenCategoriaVenta.AlmacenId
                }
            };
            SvCatalogoAlmacenConsultar servicio = new SvCatalogoAlmacenConsultar(rpCatalogoAlmacen, esCatalogoAlmacen);
            EnCatalogoAlmacen CatalogoAlmacen = servicio.Consultar().Single();
            _estructuraEntidad.AlmacenCategoriaVenta.Almacen = CatalogoAlmacen;
        }
    }
}
