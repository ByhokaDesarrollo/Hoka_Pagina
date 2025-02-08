using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using hoka.Models;
using System.Web.Mvc;

namespace hoka.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        private readonly int idRol;
        private readonly List<int> requiredPermisos;

        public ValidarSesionAttribute(int idRol, params int[] permisos)
        {
            this.idRol = idRol;
            this.requiredPermisos = new List<int>(permisos);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var usuarioSesion = HttpContext.Current.Session["usuario"] as UsuarioModel;
            List<int?> permisosSesion = HttpContext.Current.Session["permisosId"] as List<int?>;

            if (usuarioSesion == null || usuarioSesion.Id_Estado != 1)
            {
                filterContext.Result = new RedirectResult("~/Home/ErrorView?");
                return;
            }

            if (usuarioSesion.Id_Rol != idRol)
            {
                filterContext.Result = new RedirectResult("~/Home/ErrorView?");
                return;
            }

            // Si hay permisos especificados en el atributo
            if (requiredPermisos.Count > 0)
            {
                // Si el usuario en la sesión no tiene una lista de permisos o no tiene todos los permisos requeridos
                if (permisosSesion == null || !requiredPermisos.All(p => permisosSesion.Contains(p)))
                {
                    filterContext.Result = new RedirectResult("~/Home/ErrorView?");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }


}