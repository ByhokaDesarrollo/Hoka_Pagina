using System;

namespace hoka_cli.Models.Utileria.Validaciones
{
    public static class SvValidarAccesoPorFecha
    {
        public static bool Validar(DateTime fechaActual)
        {
            DateTime fechaVigencia = new DateTime(2025, 5, 9);
            bool accesoPorFecha = DateTime.Compare(fechaVigencia, fechaActual) > 0;
            return accesoPorFecha;
        }
    }
}
