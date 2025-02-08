using hoka_cli.Struct;
using System.Collections.Generic;

namespace hoka_cli.Models.Compuadmo.Categoria
{
    public class EsCategoria : EsEstructura
    {
        public EsCategoria()
        {
            Categoria = new EnCategoria();
            Categorias = new HashSet<EnCategoria>();
        }

        public EnCategoria Categoria { get; set; }
        public ICollection<EnCategoria> Categorias { get; set; }

        #region
        public bool B_ConsultarCategoriaCero { get; set; }
        #endregion
    }
}
