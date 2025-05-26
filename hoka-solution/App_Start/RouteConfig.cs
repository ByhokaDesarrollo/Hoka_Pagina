using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace hoka
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "super-caja-cuadre-efectivo",
            //    url: "super/caja-cuadre-efectivo",
            //    defaults: new { controller = "SuperPages", action = "VCuadreEfectivo" }
            //);

            //routes.MapRoute(
            //    name: "super-caja-vouchers",
            //    url: "super/caja-vouchers",
            //    defaults: new { controller = "SuperPages", action = "VCuadreVoucher" }
            //);

            //routes.MapRoute(
            //    name: "super-caja-obtener-cuadre-efectivo",
            //    url: "super/caja-obtener-cuadre-efectivo",
            //    defaults: new { controller = "SuperPages", action = "ObtenerCuadreEfectivo" }
            //);

            //routes.MapRoute(
            //    name: "super-caja-obtener-cuadre-vouchers",
            //    url: "super/caja-obtener-cuadre-vouchers",
            //    defaults: new { controller = "SuperPages", action = "ObtenerCuadreVouchers" }
            //);

            // routes.MapRoute(
            //     name: "super-caja-actualizar-cuadre-vouchers",
            //     url: "super/caja-actualizar-cuadre-vouchers",
            //     defaults: new { controller = "SuperPages", action = "ActualizarCuadreVouchers" }
            // );

            // routes.MapRoute(
            //     name: "super-caja-actualizar-cuadre-monedas",
            //     url: "super/caja-actualizar-cuadre-monedas",
            //     defaults: new { controller = "SuperPages", action = "ActualizarCuadreMonedas" }
            // );

            // routes.MapRoute(
            //     name: "super-caja-almacenes",
            //     url: "super/caja-almacenes",
            //     defaults: new { controller = "SuperPages", action = "VCajaAlmacenes" }
            // );

            // routes.MapRoute(
            //     name: "super-caja-almacenes-fecha",
            //     url: "super/caja-almacenes-fecha/{IdAlmacen}",
            //     defaults: new { controller = "SuperPages", action = "VCajaAlmacenesFecha" }
            // );

            // routes.MapRoute(
            //     name: "super-caja-almacenes-fecha-moneda",
            //     url: "super/caja-almacenes-fecha-moneda/{IdAlmacen}/{Fecha}",
            //     defaults: new { controller = "SuperPages", action = "VCajaAlmacenesFechaMoneda" }
            // );

            // routes.MapRoute(
            //    name: "super-caja-almacenes-fecha-moneda-efectivo",
            //    url: "super/caja-almacenes-fecha-moneda/{IdAlmacen}/{Fecha}/{IdMoneda}",
            //    defaults: new { controller = "SuperPages", action = "VCajaAlmacenesFechaMonedaEfectivo" }
            //);

            // routes.MapRoute(
            //     name: "super-reporte-registro-ventas",
            //     url: "super/caja-reporte-ventas",
            //     defaults: new { controller = "SuperPages", action = "VReporteRegistroVentas" }
            // );

            // routes.MapRoute(
            //     name: "super-filter-getsalesbycategories",
            //     url: "super/filter-getsalesbycategories",
            //     defaults: new { controller = "SuperPages", action = "getSalesByCategories" }
            // );

            //admin

            routes.MapRoute(
              name: "admin-caja-obtener-cuadre-vouchers",
              url: "admin/caja-obtener-cuadre-vouchers",
              defaults: new { controller = "AdminPages", action = "ObtenerCuadreVouchers" }
          );

            routes.MapRoute(
               name: "admin-caja-cuadre-efectivo",
               url: "admin/caja-cuadre-efectivo",
               defaults: new { controller = "AdminPages", action = "VCuadreEfectivo" }
           );

            routes.MapRoute(
                name: "admin-caja-cuadre-voucher",
                url: "admin/caja-cuadre-voucher",
                defaults: new { controller = "AdminPages", action = "VCuadreVoucher" }
            );

            routes.MapRoute(
                name: "admin-caja-obtener-cuadre-efectivo",
                url: "admin/caja-obtener-cuadre-efectivo",
                defaults: new { controller = "AdminPages", action = "ObtenerCuadreEfectivo" }
            );

            routes.MapRoute(
                name: "admin-caja-obtener-cuadre-voucher",
                url: "admin/caja-obtener-cuadre-voucher",
                defaults: new { controller = "AdminPages", action = "ObtenerCuadreVoucher" }
            );

            routes.MapRoute(
                name: "admin-caja-actualizar-cuadre-vouchers",
                url: "admin/caja-actualizar-cuadre-vouchers",
                defaults: new { controller = "AdminPages", action = "ActualizarCuadreVouchers" }
            );

            routes.MapRoute(
                name: "admin-caja-actualizar-cuadre-monedas",
                url: "admin/caja-actualizar-cuadre-monedas",
                defaults: new { controller = "AdminPages", action = "ActualizarCuadreMonedas" }
            );

            routes.MapRoute(
                name: "admin-gestionar-efectivo",
                url: "admin/gestionar-efectivo",
                defaults: new { controller = "AdminPages", action = "VGestionarEfectivo" }
            );

            routes.MapRoute(
                name: "admin-gestionar-efectivo-almacenes-fecha",
                url: "admin/gestionar-efectivo-almacenes-fecha/{IdAlmacen}",
                defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFecha" }
            );

            routes.MapRoute(
                name: "admin-gestionar-vouchers-almacenes-fecha",
                url: "admin/gestionar-vouchers-almacenes-fecha/{IdAlmacen}",
                defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFechaVoucher" }
            );


            routes.MapRoute(
                name: "admin-caja-almacenes-fecha-moneda",
                url: "admin/caja-almacenes-fecha-moneda/{IdAlmacen}/{Fecha}",
                defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFechaMoneda" }
            );

            routes.MapRoute(
                name: "admin-caja-almacenes-fecha-moneda-voucher",
                url: "admin/caja-almacenes-fecha-moneda-voucher/{IdAlmacen}/{Fecha}",
                defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFechaMonedaVoucher" }
            );

            routes.MapRoute(
               name: "admin-caja-almacenes-fecha-moneda-efectivo",
               url: "admin/caja-almacenes-fecha-moneda-efectivo/{IdAlmacen}/{Fecha}/{IdMoneda}",
               defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFechaMonedaEfectivo" }
           );

            routes.MapRoute(
               name: "admin-caja-almacenes-fecha-moneda-efectivo-voucher",
               url: "admin/caja-almacenes-fecha-moneda-efectivo-voucher/{IdAlmacen}/{Fecha}/{IdVoucher}",
               defaults: new { controller = "AdminPages", action = "VCajaAlmacenesFechaMonedaEfectivoVoucher" }
           );

            //para el usuario edicion de voucher
            routes.MapRoute(
               name: "usuario-caja-almacenes-fecha-moneda-efectivo-voucher",
               url: "usuario/caja-almacenes-fecha-moneda-efectivo-voucher/{IdAlmacen}/{Fecha}/{IdVoucher}",
               defaults: new { controller = "UserPages", action = "VCajaAlmacenesFechaMonedaEfectivoVoucher" }
           );

            routes.MapRoute(
                name: "admin-gestionar-vouchers",
                url: "admin/gestionar-vouchers",
                defaults: new { controller = "AdminPages", action = "VGestionarVouchers" }
            );

            routes.MapRoute(
                name: "admin-reporte-registro-ventas",
                url: "admin/caja-reporte-ventas",
                defaults: new { controller = "AdminPages", action = "VReporteRegistroVentas" }
            );

            routes.MapRoute(
                name: "admin-filter-getsalesbycategories",
                url: "admin/filter-getsalesbycategories",
                defaults: new { controller = "AdminPages", action = "getSalesByCategories" }
            );
            //admin

            // Usuario
            routes.MapRoute(
                name: "usuario-vouchers",
                url: "usuario/vouchers",
                defaults: new { controller = "UserPages", action = "VVouchers" }
            );

            routes.MapRoute(
                name: "usuario-vouchers-captura",
                url: "usuario/vouchers-captura/{IdVoucher}",
                defaults: new { controller = "UserPages", action = "VVouchersCaptura" }
            );

            // BANCO:CAJA:CARATULA
            routes.MapRoute(
                name: "banco-caratula",
                url: "banco/caratula",
                defaults: new
                {
                    controller = "BancoCaratula",
                    action = "Index"
                }
            );

            // BANCO:CAJA:REPORTE
            routes.MapRoute(
                name: "banco-caratula-efectivo-reporte",
                url: "banco/caratula/reporte-efectivo",
                defaults: new
                {
                    controller = "BancoCaratulaEfectivoReporte",
                    action = "Index"
                }
            );

            // ALMACEN:CAJA:CARATULA
            routes.MapRoute(
                name: "almacen-caja-caratula",
                url: "almacen/caja/caratula",
                defaults: new
                {
                    controller = "AlmacenCaratula",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "almacen-caja-caratula-reporte",
                url: "almacen/caja/caratula/reporte",
                defaults: new
                {
                    controller = "AlmacenCaratulaReporte",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "almacen-caja-caratula-venta",
                url: "almacen/caja/caratula/venta",
                defaults: new
                {
                    controller = "AlmacenCaratulaVenta",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "almacen-caja-caratula-efectivo",
                url: "almacen/caja/caratula/efectivo",
                defaults: new
                {
                    controller = "AlmacenCaratulaEfectivo",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "almacen-caja-caratula-voucher",
                url: "almacen/caja/caratula/voucher",
                defaults: new
                {
                    controller = "AlmacenCaratulaVoucher",
                    action = "Index"
                }
            );

            // </GESTOR REPORTE VENTAS

            routes.MapRoute(
                name: "usuario-reporte-registro-ventas",
                url: "usuario/caja-reporte-ventas",
                defaults: new { controller = "UserPages", action = "VReporteRegistroVentas" }
            );

            routes.MapRoute(
                name: "usuario-reporte-registro-efectivo-monedas",
                url: "usuario/caja-reporte-efectivo-monedas",
                defaults: new { controller = "UserPages", action = "VCajaReporteEfectivoMoneda" }
            );

            routes.MapRoute(
                name: "usuario-reporte-registro-efectivo-monedas-valores",
                url: "usuario/caja-reporte-efectivo-monedas-valores/{IdMoneda}",
                defaults: new { controller = "UserPages", action = "VReporteRegistroEfectivo" }
            );

            routes.MapRoute(
                name: "usuario-filter-getsalesbycategories",
                url: "usuario/filter-getsalesbycategories",
                defaults: new { controller = "UserPages", action = "getSalesByCategories" }
            );

            //actualizar otros campos del resumen de ventas
            routes.MapRoute(
                name: "update-status-sales",
                url: "update-status-sales",
                defaults: new { controller = "Main", action = "updateStatusForSales" }
            );

            //actualizar otros campos del resumen de ventas
            routes.MapRoute(
                name: "update-data-sales",
                url: "update-data-sales",
                defaults: new { controller = "Main", action = "updateDataForSales" }
            );

            routes.MapRoute(
                name: "update-status-coin",
                url: "update-status-coin",
                defaults: new { controller = "Main", action = "updateStatusCoin" }
            );

            routes.MapRoute(
                name: "update-status-voucher",
                url: "update-status-voucher",
                defaults: new { controller = "Main", action = "updateStatusVoucher" }
            );


            //routes.MapRoute(
            //    name: "usuario-reporte-registro-efectivo",
            //    url: "usuario/caja-reporte-efectivo",
            //    defaults: new { controller = "UserPages", action = "VReporteRegistroEfectivo" }
            //);

            //

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        }
    }
}
