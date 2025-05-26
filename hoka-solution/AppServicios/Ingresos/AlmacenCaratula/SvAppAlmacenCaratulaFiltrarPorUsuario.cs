using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using System.Collections.Generic;
using System.Linq;

namespace hoka.AppServicios.Ingresos.AlmacenCaratula
{
    public static class SvAppAlmacenCaratulaFiltrarPorUsuario
    {
        public static ICollection<EnAlmacenCaratula> Filtrar(
            ICollection<EnUsuarioPermisoAlmacenCaratula> almacenes,
            ICollection<EnAlmacenCaratula> caratulas)
        {
            caratulas = caratulas
                .Where(
                    x => 
                    almacenes.Any(
                        c =>
                        c.AlmacenId == x.AlmacenId &&
                        c.B_Activo
                    )
                )
                .ToHashSet();
            return caratulas;
        }
    }
}