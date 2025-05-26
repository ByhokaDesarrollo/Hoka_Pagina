using hoka_cli.Context.EntityFramework.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka_cli.Struct
{
    public class EsConsulta
    {
        public EsConsulta()
        {
            Paginacion = new EnPaginacion();
        }

        public int UsuarioId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// 0 Todos
        /// 1 Activos
        /// 2 Inactivos
        /// </summary>
        public byte TablaRegistroEstatusId { get; set; }
        public bool B_TablaRegistroDescendente { get; set; }
        public int NumeroRegistros { get; set; }
        public EnPaginacion Paginacion { get; set; }
        public List<int> ListaIds { get; set; }

        [NotMapped]
        public string FechaInicioFormatoFecha => FechaInicio?.ToString("yyyyMMdd") ?? "";

        [NotMapped]
        public string FechaFinFormatoFecha => FechaFin?.ToString("yyyyMMdd") ?? "";
    }
}
