using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;   // ⬅️  Necesario para [NotMapped]
using System.Web;

namespace hoka.Entity.Data
{
    public class Reembolso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El folio es requerido")]
        [StringLength(50, ErrorMessage = "El folio no puede exceder 50 caracteres")]
        public string Folio { get; set; }

        [Required(ErrorMessage = "La fecha de solicitud es requerida")]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "La forma de pago es requerida")]
        [StringLength(50, ErrorMessage = "La forma de pago no puede exceder 50 caracteres")]
        public string FormaPago { get; set; }

        [Required(ErrorMessage = "El tipo de moneda es requerido")]
        [StringLength(20, ErrorMessage = "El tipo de moneda no puede exceder 20 caracteres")]
        public string TipoMoneda { get; set; }

        [StringLength(100, ErrorMessage = "El banco no puede exceder 100 caracteres")]
        public string Banco { get; set; }

        [StringLength(50, ErrorMessage = "La cuenta bancaria no puede exceder 50 caracteres")]
        public string CuentaBancaria { get; set; }

        [StringLength(100, ErrorMessage = "La sucursal no puede exceder 100 caracteres")]
        public string Sucursal { get; set; }

        [Required(ErrorMessage = "El nombre del proveedor es requerido")]
        [StringLength(200, ErrorMessage = "El nombre del proveedor no puede exceder 200 caracteres")]
        public string NombreProveedor { get; set; }

        [Required(ErrorMessage = "El nombre del solicitante es requerido")]
        [StringLength(200, ErrorMessage = "El nombre del solicitante no puede exceder 200 caracteres")]
        public string NombreSolicitante { get; set; }

        [Required(ErrorMessage = "El concepto es requerido")]
        [StringLength(200, ErrorMessage = "El concepto no puede exceder 200 caracteres")]
        public string Concepto { get; set; }

        [StringLength(200, ErrorMessage = "El subconcepto no puede exceder 200 caracteres")]
        public string Subconcepto { get; set; }

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        [StringLength(20, ErrorMessage = "El tipo de documento no puede exceder 20 caracteres")]
        public string TipoDocumento { get; set; }

        [StringLength(50, ErrorMessage = "El número de documento no puede exceder 50 caracteres")]
        public string NumeroDocumento { get; set; }

        [Required(ErrorMessage = "El tipo de operación es requerido")]
        [StringLength(20, ErrorMessage = "El tipo de operación no puede exceder 20 caracteres")]
        public string TipoOperacion { get; set; }

        public string Observaciones { get; set; }

        [StringLength(50, ErrorMessage = "El UUID no puede exceder 50 caracteres")]
        public string Uuid { get; set; }

        [Required(ErrorMessage = "El importe sin IVA es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El importe debe ser positivo")]
        public decimal ImporteSinIva { get; set; }

        [Required(ErrorMessage = "El importe de IVA es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El importe debe ser positivo")]
        public decimal ImporteIva { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La retención debe ser positiva")]
        public decimal RetencionIsr { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "La retención debe ser positiva")]
        public decimal RetencionIva { get; set; }

        [Required(ErrorMessage = "El total es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El total debe ser positivo")]
        public decimal Total { get; set; }

        [StringLength(255, ErrorMessage = "La ruta del XML no puede exceder 255 caracteres")]
        public string XmlPath { get; set; }

        [StringLength(255, ErrorMessage = "La ruta del PDF no puede exceder 255 caracteres")]
        public string PdfPath { get; set; }

        [StringLength(255, ErrorMessage = "La ruta de la cotización no puede exceder 255 caracteres")]
        public string CotizacionPath { get; set; }

        [Required(ErrorMessage = "La opción de pago es requerida")]
        [StringLength(50, ErrorMessage = "La opción de pago no puede exceder 50 caracteres")]
        public string OpcionPago { get; set; }

        /* ------------------------------------------------------------
         *  Propiedades para los archivos: solo las usa la vista,
         *  EF no debe mapearlas → [NotMapped]
         * ------------------------------------------------------------ */
        [NotMapped]
        [Display(Name = "Archivo XML")]
        public HttpPostedFileBase XmlFile { get; set; }

        [NotMapped]
        [Display(Name = "Archivo PDF")]
        public HttpPostedFileBase PdfFile { get; set; }

        [NotMapped]
        [Display(Name = "Cotización")]
        public HttpPostedFileBase CotizacionFile { get; set; }
    }
}
