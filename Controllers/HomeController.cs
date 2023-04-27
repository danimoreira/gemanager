using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GEPosVendas.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor? _httpContextAccessor = default;
        // GET: Home
        public IActionResult Index()
        {
            this.UpdateBag();
            if (string.IsNullOrEmpty(ViewBag.Usuario))
                return RedirectToAction("Index", "Login");

            return RedirectToAction("Index", "Tarefas");
        }

        public void UpdateBag()
        {
            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = Convert.ToInt32(_httpContextAccessor?.HttpContext?.Request.Cookies["idVendedorLogado"]);
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}