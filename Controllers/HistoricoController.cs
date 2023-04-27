using GEPV.Domain.DTO;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using GEPV.Domain.SQL;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using GEPV.Domain.Entities;

namespace GEPosVendas.Controllers
{
    public class HistoricoController : Controller
    {
        private ClienteService ClienteService = new ClienteService(new ClienteRepository());
        private VendedorService VendedorService = new VendedorService(new VendedorRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Historico
        public IActionResult Index()
        {
            UpdateBag();

            return View();
        }

        public IActionResult Pesquisar(int? idCliente, int? idVendedor)
        {
            var historicoList = new List<HistoricoDTO>();

            if (idCliente is not null || idVendedor is not null)
                historicoList= new Consultas().GetHistoricoContatos(idCliente, null, null);

            ViewBag.Historico = historicoList;

            UpdateBag();

            return View("Index");
        }

        private void UpdateBag()
        {
            var idVendedorLogado = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);            
            var vendedor = VendedorService.GetById(idVendedorLogado);            

            var clientes = ClienteService.List();

            if (vendedor.Admin == 0)
                clientes = clientes.Where(x => x.IdVendedor == idVendedorLogado).ToList();

            ViewBag.clientes = clientes
            .Select(x => new SelectListItem()
            {
                Text = x.RazaoSocial,
                Value = x.Id.ToString()
            }).ToList().OrderBy(y => y.Text);

            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = idVendedorLogado;
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}