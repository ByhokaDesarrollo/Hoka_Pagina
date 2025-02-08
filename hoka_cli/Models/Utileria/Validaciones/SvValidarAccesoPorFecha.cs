using System;

namespace hoka_cli.Models.Utileria.Validaciones
{
    public static class SvValidarAccesoPorFecha
    {
        public static bool Validar(DateTime fechaActual)
        {
            DateTime fechaVigencia = new DateTime(2024, 11, 22);
            bool accesoPorFecha = DateTime.Compare(fechaVigencia, fechaActual) > 0;
            return accesoPorFecha;
        }
    }
}
