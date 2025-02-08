using hoka_cli.Context.EntityFramework.Entities;

namespace hoka_cli.Struct
{
    public class EsEstructura
    {
        public EsEstructura()
        {
            Consulta = new EsConsulta();
            Propiedad = new EsPropiedad();
            Error = new EsError();

        }

        public EsConsulta Consulta { get; set; }
        public EsPropiedad Propiedad { get; set; }
        public EsError Error { get; set; }

        public void EstablecerObjetoConsultaNulo() => Consulta = null;
        public void EstablecerObjetoPropiedadNulo() => Propiedad = null;
        public void EstablecerObjetoErrorNulo() => Error = null;
    }
}
