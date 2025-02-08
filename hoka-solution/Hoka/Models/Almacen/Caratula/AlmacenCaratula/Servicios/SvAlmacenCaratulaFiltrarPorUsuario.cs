using hoka.HokaCli.Models.Compuadmo.Usuario.Permiso.AlmacenCaratula;
using hoka.HokaCli.Models.Ingresos.AlmacenCaratula;
using System.Collections.Generic;
using System.Linq;

namespace hoka.Hoka.Models.Almacen.Caratula.AlmacenCaratula.Servicios
{
    public static class SvAlmacenCaratulaFiltrarPorUsuario
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