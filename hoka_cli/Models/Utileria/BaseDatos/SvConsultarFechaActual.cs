using hoka_cli.Context.EntityFramework;
using hoka_cli.Context.EntityFramework.Servicios;
using System;
using System.Linq;

namespace hoka_cli.Models.Utileria.BaseDatos
{
    public static class SvConsultarFechaActual
    {
        //public async static Task<DateTime> Consultar()
        //{
        //    DBCHokaCompuadmo dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
        //    DateTime fechaActual = await dbContext.Database.SqlQuery<DateTime>("SELECT DATEADD(DAY, 0, GETDATE())").SingleAsync();
        //    return fechaActual;
        //}

        public static DateTime Consultar(int cantidadDias)
        {
            DBCHokaCompuadmo dbContext = SvEstablecerConexionDBCHokaCompuadmo.EstablecerConexion();
            DateTime fechaActual = dbContext.Database.SqlQuery<DateTime>($"SELECT DATEADD(DAY, {cantidadDias}, GETDATE())").Single();
            return fechaActual;
        }
    }
}
