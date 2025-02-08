using hoka_cli.Context.EntityFramework.Entities;
using hoka_cli.Struct;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace hoka_cli.Models.Compuadmo.Catalogo.Almacen
{
    public class SvCatalogoAlmacenConsultarPorUsuario
    {
        private RpCatalogoAlmacen _rpCatalogoAlmacen;
        private EsCatalogoAlmacen _estructuraEntidad;
        private ICollection<EnCatalogoAlmacen> _entidades;

        public SvCatalogoAlmacenConsultarPorUsuario(RpCatalogoAlmacen repositorio, EsCatalogoAlmacen esCatalogoAlmacen)
        {
            _rpCatalogoAlmacen = repositorio;
            _estructuraEntidad = esCatalogoAlmacen;
            _entidades = new HashSet<EnCatalogoAlmacen>();
        }

        public ICollection<EnCatalogoAlmacen> Consultar()
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
            EnCatalogoAlmacen FiltroEntidad = _estructuraEntidad.CatalogoAlmacen;
            string query = $"EXECUTE SP_CatalogoAlmacenObtenerPorUsuario {FiltroConsulta.UsuarioId}";
            IEnumerable<EnCatalogoAlmacen> Query = _rpCatalogoAlmacen.FindByQuery(query);
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.TablaRegistroEstatusId > 0)
                    switch (FiltroConsulta.TablaRegistroEstatusId)
                    {
                        case 1:
                            Query = Query.Where(q => q.B_TieneSistema == true);
                            break;
                        case 2:
                            Query = Query.Where(q => q.B_TieneSistema == false);
                            break;
                        default:
                            break;
                    };
            }
            if (FiltroPaginacion != null && FiltroPaginacion.B_Paginacion)
            {
                if (FiltroPaginacion.UltimoId > 0)
                    Query = Query.Where(x => x.AlmacenId > FiltroPaginacion.UltimoId);
                if (FiltroPaginacion.NumeroRegistros > 0)
                    Query = Query.Take(FiltroPaginacion.NumeroRegistros);
            }
            if (FiltroEntidad != null)
            {
                if (FiltroEntidad.AlmacenId > 0)
                    Query = Query.Where(q => q.AlmacenId == FiltroEntidad.AlmacenId);
                if (!string.IsNullOrEmpty(FiltroEntidad.SucursalPrefijo))
                    Query = Query.Where(q => q.SucursalPrefijo.ToUpper().Contains(FiltroEntidad.SucursalPrefijo.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Nombre))
                    Query = Query.Where(q => q.Nombre.ToUpper().Contains(FiltroEntidad.Nombre.ToUpper()));
                if (!string.IsNullOrEmpty(FiltroEntidad.Prefijo))
                    Query = Query.Where(q => q.Prefijo.ToUpper().Contains(FiltroEntidad.Prefijo.ToUpper()));
            }
            if (FiltroConsulta != null)
            {
                if (FiltroConsulta.NumeroRegistros > 0)
                    Query = Query.Take(FiltroConsulta.NumeroRegistros);
                if (FiltroConsulta.B_TablaRegistroDescendente)
                    Query = Query.OrderByDescending(q => q.AlmacenId);
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
