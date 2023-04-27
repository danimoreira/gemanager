using GEPV.Domain.DTO;
using GEPV.Domain.Interfaces.Services;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPosVendas.Controllers
{
    public class LoginController : Controller
    {

        private VendedorService VendedorService = new VendedorService(new VendedorRepository());
        private LoginService Service = new LoginService(new LoginRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = new HttpContextAccessor();

        // GET: Login
        public IActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/GEPV/Home";
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginDto loginDto, string returnUrl)
        {

            if (ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"] != null) { 
                return RedirectToAction("Index", "Tarefas");
            }
            ViewBag.ReturnUrl = returnUrl;


            ViewBag.LoginFailed = 0;
            var usuarioDados = Service.Logar(loginDto, VendedorService);

            if (usuarioDados == null) {
                HttpContext.Session.Clear();
                ViewBag.LoginFailed = 1;
                return View("Index", loginDto);
            }


            Response.Cookies.Delete(loginDto.Usuario);

            Response.Cookies.Delete("displayName");
            Response.Cookies.Append("displayName", usuarioDados.Nome);

            Response.Cookies.Delete("emailUsuario");
            Response.Cookies.Append("emailUsuario", usuarioDados.Email);

            Response.Cookies.Delete("idVendedorLogado");
            Response.Cookies.Append("idVendedorLogado", usuarioDados.Id.ToString());

            Response.Cookies.Delete("tokenUsuario");
            Response.Cookies.Append("tokenUsuario", usuarioDados.Admin == 1 ? "A" : "U");

            Response.Cookies.Delete("dataAtual");
            Response.Cookies.Append("dataAtual", DateTime.Now.ToString("dd/MM/yyyy"));


            return RedirectToAction("Index", "Tarefas");
        }

        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            foreach (var cookie in _httpContextAccessor?.HttpContext?.Request.Cookies) {
                Response.Cookies.Delete(cookie.Key);
            }
            return View("Index", new LoginDto());
        }
    }
}