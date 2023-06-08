using GEPV.Domain.DTO;
using GEPV.Domain.Entities;
using GEPV.Domain.Interfaces.Services;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using GEPV.Domain.SQL;
using GEPV.Domain.Util;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GEPosVendas.Controllers
{
    public class TarefasController : Controller
    {
        private ContatosService Service = new ContatosService(new ContatosRepository());
        private ClienteService clienteService = new ClienteService(new ClienteRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;


        private List<FeriadoCliente> getFeriadosDoCliente(TarefasClientes item) {

            var ferMes = new Consultas().GetFeriadosMes();

            return ferMes.FindAll(x =>
                    //Estadual
                    (x.Tipo == 2 && x.Uf == item.UfEstado && x.Municipio == null)
                    //Municipal
                    || (x.Uf == item.UfEstado && x.Municipio == item.Cidade)
                    //Nacional
                    || (x.Uf == null && x.Tipo == 1));
        }
        // GET: Tarefas
        public IActionResult Index()
        {            
            var idVendedorLogado = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);
            var mensagens = new Consultas().GetMensagens();
            var vendedores = new Consultas().GetVendedores();
            var clientes = new Consultas().GetClientes(idVendedorLogado);
            ViewBag.TotalClientes = 0;
            foreach (var item in vendedores)
            {
                item.QtdeCliente = clientes.Where(x => x.IdVendedor == item.IdVendedor).Count();
                ViewBag.TotalClientes += item.QtdeCliente;
            }

            var listaPotencial = new List<SelectListItem>
            {
                            new SelectListItem { Text = "Indefinido", Value="0" },
                            new SelectListItem { Text = "P", Value="1" },
                            new SelectListItem { Text = "M", Value="2" },
                            new SelectListItem { Text = "G", Value="3" }
            };

            
            foreach (var item in clientes)
            {
                item.Feriados = this.getFeriadosDoCliente(item);

                item.PotencialNome = listaPotencial.Find(x => x.Value == item.Potencial.ToString())?.Text;
            }

            ViewBag.MensagemPrincipal = mensagens.FirstOrDefault();

            if (ViewBag.MensagemPrincipal != null) { 
                mensagens.RemoveAt(0);
            }

            ViewBag.Mensagens = mensagens;
            ViewBag.Vendedores = vendedores.Join(clientes, 
                            vendedores => vendedores.IdVendedor, 
                            clientes => clientes.IdVendedor, 
                            (vendedores, clientes) => new {vendedores.IdVendedor, vendedores.NomeVendedor, vendedores.QtdeCliente})
                            .DistinctBy(x => x.IdVendedor)
                            .Select(x => new TarefasVendedores() { 
                                IdVendedor = x.IdVendedor, 
                                NomeVendedor = x.NomeVendedor, 
                                QtdeCliente = x.QtdeCliente }).ToList();
            ViewBag.Clientes = clientes;

            ViewBag.TemFilial = false;

            ViewBag.Usuario = HttpContext.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = idVendedorLogado;

            ViewBag.ClienteSelecionado = HttpContext.Request.Query["idClienteSelecionado"];
            ViewBag.VendedorSelecionado = HttpContext.Request.Query["idVendedorSelecionado"];

            return View();
        }

        [HttpGet]
        public ViewResult ReloadTarefas(int? idClienteSelecionado, int? idVendedorSelecionado)
        {
            var idVendedorLogado = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);

            var vendedores = new Consultas().GetVendedores();
            var clientes = new Consultas().GetClientes(idVendedorLogado);
            ViewBag.TotalClientes = 0;

            foreach (var item in vendedores)
            {
                item.QtdeCliente = clientes.Where(x => x.IdVendedor == item.IdVendedor).Count();
                ViewBag.TotalClientes += item.QtdeCliente;
            }


            var listaPotencial = new List<SelectListItem>
            {
                            new SelectListItem { Text = "Indefinido", Value="0" },
                            new SelectListItem { Text = "P", Value="1" },
                            new SelectListItem { Text = "M", Value="2" },
                            new SelectListItem { Text = "G", Value="3" }
            };

            foreach (var item in clientes)
            {
                item.Feriados = this.getFeriadosDoCliente(item);

                if (item.Feriados == null) {
                    item.Feriados = new List<FeriadoCliente>();
                }
                item.PotencialNome = listaPotencial.Find(x => x.Value == item.Potencial.ToString())?.Text;
            }            
            
            ViewBag.Vendedores = vendedores.Where(x => x.QtdeCliente > 0).ToList();
            ViewBag.Clientes = clientes;

            if (HttpContext.Request.Cookies["displayName"] == null)
            {
                return View("Index");
            }

            UpdateBag(idClienteSelecionado);
            
            ViewBag.IdVendedorLogado = idVendedorLogado;

            ViewBag.ClienteSelecionado = idClienteSelecionado ?? 0;
            ViewBag.VendedorSelecionado = idVendedorSelecionado ?? 0;

            return View();
        }

        [HttpGet]
        public IActionResult PesquisarClientes(string termo)
        {
            var idVendedorLogado = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);

            var vendedores = new Consultas().GetVendedores();
            var clientes = new Consultas().GetClientes(idVendedorLogado);
            
            if (!string.IsNullOrWhiteSpace(termo))
                clientes = clientes
                            .Where(x => Utilitario.RemoveDiacritics(x.Nome.ToUpper()).Contains(Utilitario.RemoveDiacritics(termo.ToUpper())) 
                                    || Utilitario.RemoveDiacritics(x.RegiaoDescricao.ToUpper()).Contains(Utilitario.RemoveDiacritics(termo.ToUpper())))
                            .ToList();

            foreach (var item in vendedores)
            {
                item.QtdeCliente = clientes.Where(x => x.IdVendedor == item.IdVendedor).Count();
            }

            ViewBag.termo = termo;
            ViewBag.Vendedores = vendedores;
            ViewBag.Clientes = clientes;
            ViewBag.Usuario = HttpContext.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = idVendedorLogado;

            return View("Index");
        }

        public IActionResult RealizarTarefas(int? idCliente, int? idFornecedor)
        {
            if (!idCliente.HasValue)
                return RedirectToAction("Index");

            var idVendedor = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);
            if (idVendedor == 0)
                return RedirectToAction("Index", "Login");

            UpdateBag(idCliente, idFornecedor);

            return View();
        }

        [HttpPost]
        public ViewResult Salvar(Contatos contato, int? idClienteSelecionado, int? idVendedorSelecionado)
        {
            if (contato.DataContato.Equals(DateTime.MinValue))
                contato.DataContato = DateTime.Now;

            if (contato.Id > 0)
                Service.Update(contato);
            else
                Service.Add(contato);

            UpdateBag(contato.IdCliente, contato.IdFornecedor);

            ViewBag.ClienteSelecionado = idClienteSelecionado ?? 0;
            ViewBag.VendedorSelecionado = idVendedorSelecionado ?? 0;

            return View("RealizarTarefas");
        }

        [HttpGet]
        public ViewResult Detalhar(int? idcliente)
        {
            if (idcliente.HasValue)
                ViewBag.Fornecedores = new Consultas().GetFornecedoresPorCliente(idcliente);

            return View();
        }

        [HttpPost]
        public ViewResult ExcluirHistorico(int idHistorico, int? idCliente, int? idFornecedor, int? idClienteSelecionado, int? idVendedorSelecionado)
        {
            Service.Delete(idHistorico);

            UpdateBag(idCliente, idFornecedor, idHistorico);

            ViewBag.ClienteSelecionado = idClienteSelecionado ?? 0;
            ViewBag.VendedorSelecionado = idVendedorSelecionado ?? 0;

            return View("RealizarTarefas");
        }

        [HttpPost]
        public ViewResult EditarHistorico(int idHistorico, int? idCliente, int? idFornecedor)
        {
            UpdateBag(idCliente, idFornecedor, idHistorico);
            return View("RealizarTarefas");
        }

        [HttpGet]
        public ViewResult RecuperarDetalhes(int? idCliente, int? idFornecedor)
        {
            UpdateBag(idCliente, idFornecedor);
            return View("RealizarTarefas");
        }

        public void UpdateBag(int? idCliente = 0, int? idFornecedor =0, int? idHistorico = 0)
        {
            if (idHistorico.Value > 0)
                ViewBag.HistoricoAtual = Service.GetById(idHistorico.Value);

            ViewBag.Historico = new Consultas().GetHistoricoContatos(idCliente.Value, 0, idFornecedor.Value);
            ViewBag.IdCliente = idCliente;
            ViewBag.IdVendedor = Convert.ToInt32(HttpContext.Request.Cookies["idVendedorLogado"]);

            var listaFornecedores = new Consultas().GetFornecedoresPorCliente(idCliente);
            ViewBag.ListaFornecedores = new SelectList(listaFornecedores, "IdFornecedor", "Nome", idFornecedor);

            var cliente = clienteService.GetById(idCliente.Value);
            var idClienteMatriz = idCliente.Value;
            if (cliente.IdMatriz is not null)
                idClienteMatriz = cliente.IdMatriz.Value;

            var cnpjsCliente = new Consultas().GetCnpjCliente(idClienteMatriz);
            ViewBag.CnpjsCliente = new SelectList(cnpjsCliente, "Id", "RazaoSocial", idCliente);

            ViewBag.TemFilial = cnpjsCliente.Count() > 1;

            ViewBag.Usuario = HttpContext.Request.Cookies["displayName"];
        }




    }
}
