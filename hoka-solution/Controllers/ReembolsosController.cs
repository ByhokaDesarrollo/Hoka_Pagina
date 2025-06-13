using hoka.Entity;
using hoka.Entity.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hoka.Controllers
{
    public class ReembolsosController : Controller
    {
        // Método para llenar todos los combos y listas del ViewBag
        private void CargarCombos()
        {
            using (var db = new ApplicationDbContext())
            {
                ViewBag.Proveedores = db.Proveedores
                    .OrderBy(p => p.NombreRazonSocial)
                    .Select(p => p.NombreRazonSocial)
                    .Distinct()
                    .ToList();

                ViewBag.Sucursales = db.Almacenes
                    .OrderBy(a => a.Nombre)
                    .Select(a => a.Nombre)
                    .Distinct()
                    .ToList();

                ViewBag.Conceptos = db.Conceptos
                    .Select(c => c.TipoConcepto)
                    .Distinct()
                    .OrderBy(t => t)
                    .ToList();
            }

            ViewBag.FormasPago = new List<SelectListItem>
            {
                new SelectListItem { Text = "Efectivo", Value = "Efectivo" },
                new SelectListItem { Text = "Deposito", Value = "Deposito" }
            };
        }

        // GET: Reembolsos/Reembolso
        public ActionResult Reembolso()
        {
            try
            {
                CargarCombos();
            }
            catch (Exception ex)
            {
                ViewBag.Proveedores = new List<string>();
                ViewBag.Sucursales = new List<string>();
                ViewBag.Conceptos = new List<string>();
                ViewBag.FormasPago = new List<SelectListItem>();
                System.Diagnostics.Debug.WriteLine($"Error al cargar datos iniciales: {ex.Message}");
            }

            return View(new Reembolso { FechaSolicitud = DateTime.Now });
        }

        [HttpGet]
        public JsonResult ObtenerSubconceptos(string concepto)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var subconceptos = db.Conceptos
                        .Where(c => c.TipoConcepto == concepto)
                        .Select(c => c.Concepto)
                        .Distinct()
                        .OrderBy(s => s)
                        .ToList();

                    return Json(subconceptos, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener subconceptos: {ex.Message}");
                return Json(new List<string>(), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reembolso reembolso)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    CargarCombos();
                    return View("Reembolso", reembolso);
                }

                using (var db = new ApplicationDbContext())
                {
                    // Procesamiento de archivos
                    reembolso.XmlPath = GuardarArchivo(reembolso.XmlFile);
                    reembolso.PdfPath = GuardarArchivo(reembolso.PdfFile);
                    reembolso.CotizacionPath = GuardarArchivo(reembolso.CotizacionFile);

                    // Cálculos
                    reembolso.Total = reembolso.ImporteSinIva + reembolso.ImporteIva -
                                     reembolso.RetencionIsr - reembolso.RetencionIva;

                    db.Reembolsos.Add(reembolso);
                    int result = db.SaveChanges();

                    if (result > 0)
                    {
                        CargarCombos();
                        return View("Reembolso", new Reembolso { FechaSolicitud = DateTime.Now });
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo guardar el reembolso. Intente nuevamente.");
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");

                var fullErrorMessage = string.Join("; ", errorMessages);
                var exceptionMessage = string.Concat(ex.Message, " Errores de validación: ", fullErrorMessage);

                ModelState.AddModelError("", exceptionMessage);
            }
            catch (Exception ex)
            {
                var mensaje = $"Error al guardar: {ex.Message}";
                if (ex.InnerException != null)
                {
                    mensaje += $"\nError interno: {ex.InnerException.Message}";
                    if (ex.InnerException.InnerException != null)
                    {
                        mensaje += $"\nError SQL: {ex.InnerException.InnerException.Message}";
                    }
                }
                ModelState.AddModelError("", mensaje);
            }



            CargarCombos();
            return View("Reembolso", reembolso);
        }

        private string GuardarArchivo(HttpPostedFileBase archivo)
        {
            if (archivo == null || archivo.ContentLength == 0)
                return null;

            try
            {
                var directorio = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(directorio))
                    Directory.CreateDirectory(directorio);

                var extension = Path.GetExtension(archivo.FileName).ToLower();
                var nombreUnico = $"{Guid.NewGuid()}{extension}";
                var rutaCompleta = Path.Combine(directorio, nombreUnico);

                archivo.SaveAs(rutaCompleta);
                return $"/Uploads/{nombreUnico}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar archivo: {ex}");
                throw new Exception("No se pudo guardar el archivo adjunto. Por favor intente nuevamente.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
