using System.Web.Mvc;

namespace hoka.Controllers.Almacen.Caratula
{
    public class AlmacenCaratulaGastoController : Controller
    {
        [HttpGet]
        public ActionResult Index(int UsuarioId, string FechaRegistro, string AlmacenId)
        {
            string response = $"UsuarioId: {UsuarioId} FechaRegistro: {FechaRegistro} AlmacenId: {AlmacenId}";
            return View();
        }
    }
}