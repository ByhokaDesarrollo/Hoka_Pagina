using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hoka.HokaCli.Models.Compuadmo.Almacen
{
    [Table("almacenes")]
    public class EnAlmacen
    {
        [Key,Column(name: "Id_Almacen")]
        public int AlmacenId { get; set; }

        [MinLength(3), MaxLength(50)]
        public string Nombre { get; set; }

        [Column(name: "Almacen")]
        public string Prefijo { get; set; }

        public string RFC { get; set; }
        public string CP { get; set; }
        public string Ubicacion { get; set; }
        public string Ubicacion1 { get; set; }
        public string Responsable { get; set; }

        [Column(name: "ciudad")] public string Ciudad { get; set; }
        [Column(name: "estado")] public string Estado { get; set; }
        [Column(name: "telefono")] public string Telefono { get; set; }
        [Column(name: "fax")] public string Fax { get; set; }
        [Column(name: "internet")] public string Internet { get; set; }
        [Column(name: "email")] public string Email { get; set; }
        [Column(name: "sucursal")] public string SucursalPrefijo { get; set; }
        [Column(name: "letra")] public string Letra { get; set; }
        [Column(name: "folio_factura")] public Single? FolioFactura { get; set; }
        [Column(name: "folio_remision")] public Single? FolioRemision { get; set; }
        [Column(name: "folio_anticipos")] public Single? FolioAnticipos { get; set; }
        [Column(name: "folio_nc")] public Single? FolioNc { get; set; }
        [Column(name: "timbre")] public Single? Timbre { get; set; }
        [Column(name: "letratimbre")] public string LetraTimbre { get; set; }
        [Column(name: "folio_pedidos")] public Single? FolioPedidos { get; set; }
        [Column(name: "region")] public string Region { get; set; }
        [Column(name: "NOMBREFISCAL")] public string NombreFiscal { get; set; }
        [Column(name: "direccionfiscal")] public string DireccionFiscal { get; set; }
        [Column(name: "ciudadfiscal")] public string CiudadFiscal { get; set; }
        [Column(name: "regimen")] public string Regimen { get; set; }
        [Column(name: "nombreregimen")] public string NombreRegimen { get; set; }
        [Column(name: "dirdoctos")] public string Dirdoctos { get; set; }
        [Column(name: "nombreserver")] public string NombreServer { get; set; }
        [Column(name: "ipserver")] public string IPServer { get; set; }
        [Column(name: "carpeta")] public string Carpeta { get; set; }
        [Column(name: "foliocomplemento")] public Int64? FolioComplemento { get; set; }
        [Column(name: "folio_pedido")] public Int64? FolioPedido { get; set; }
        [Column(name: "folio_oc")] public Int64? FolioOc { get; set; }
        [Column(name: "folio_boleto")] public Int64? FolioBoleto { get; set; }
    }
}
