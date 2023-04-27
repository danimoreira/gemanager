using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using GEPV.Domain.DTO;
using GEPV.Domain.Entities;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using GEPV.Domain.SQL;
using GEPV.Domain.Util;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GEPosVendas.Controllers
{
    public class ClienteController : Controller
    {
        private ClienteService Service = new ClienteService(new ClienteRepository());
        private EstadoService EstadoService = new EstadoService(new EstadoRepository());
        private RegiaoService RegiaoService = new RegiaoService(new RegiaoRepository());
        private VendedorService VendedorService = new VendedorService(new VendedorRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = new HttpContextAccessor();

        // GET: Cliente
        public IActionResult Index()
        {            
            this.UpdateBag();
            
            var clientes = new Consultas().GetAllClientesList();

            if (!ViewBag.EhAdmin)            
                clientes = clientes.Where(x => x.IdVendedor == ViewBag.IdVendedorLogado).ToList();
        
            ViewBag.TotalClientes = clientes.Count;

            ViewBag.Clientes = clientes;
            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];

            ViewBag.ClienteSelecionado = HttpContext.Request.Query["idClienteSelecionado"];

            return View(clientes);
        }

        public IActionResult Pesquisar(string termo)
        {
            this.UpdateBag();
            if (string.IsNullOrEmpty(termo))
                return View(new List<Cliente>());            

            List<Cliente> cliente = Service.List().Where(x => Utilitario.RemoveDiacritics(x.RazaoSocial.ToUpper()).Contains(Utilitario.RemoveDiacritics(termo.ToUpper())) || x.Cnpj.Contains(termo) || x.Regiao.Descricao.Contains(termo)).ToList();

            return View(cliente);
        }

        public IActionResult PesquisarClientesSemVendedor(bool clienteSemVendedor)
        {
            this.UpdateBag();                    
            
            var clientes = Service.List();

            if (clienteSemVendedor)
                clientes = clientes.Where(x => x.IdVendedor is null).ToList();
          
            return View(clientes);
        }

        private List<SelectListItem> getListPotenciais(string defValue) {
            
            var listPotenciais = new List<SelectListItem> {
                            new SelectListItem { Text = "Indefinido", Value="0", Selected = false },
                            new SelectListItem { Text = "Pequeno", Value="1", Selected = false },
                            new SelectListItem { Text = "Médio", Value="2", Selected = false },
                            new SelectListItem { Text = "Grande", Value="3", Selected = false }
            };

            foreach (SelectListItem sl in listPotenciais) {
                if (sl.Value == defValue) {
                    sl.Selected = true;
                }
            }

            return listPotenciais;

        }

        // GET: Cliente/Details/5
        public IActionResult Details(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Cliente cliente = Service.GetById(id.Value);

            var potenciais = getListPotenciais(cliente.Potencial.ToString());

            ViewBag.PotencialNome = potenciais.Find(x => x.Value == cliente.Potencial.ToString()).Text;

            if (cliente == null)
            {
                return new StatusCodeResult(404);
            }
            return View(cliente);
        }

        // GET: Cliente/Create
        public IActionResult Create()
        {
            this.UpdateBag();
            
            return View();
        }

        // POST: Cliente/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,RazaoSocial,NomeFantasia,Cnpj,InscricaoEstadual,TelefonePrincipal,TelefoneContato,EmailPrincipal,EmailNFe,Observacao,Logradouro,Numero,Bairro,Cep,Cidade,IdEstado,IdRegiao,IdVendedor,NomeComprador,Potencial,Latilong,Situacao,IdMatriz")] Cliente cliente)
        {
            this.UpdateBag();
            if (ModelState.IsValid)
            {
                cliente.Situacao = "A";
                Service.Add(cliente);
                return RedirectToAction("Index");
            }

            FillLists();
           
            return View(cliente);
        }

        // GET: Cliente/Edit/5
        public IActionResult Edit(int? id)
        {
            this.UpdateBag();
            
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Cliente cliente = Service.GetById(id.Value);
            if (cliente == null)
            {
                return new StatusCodeResult(404);
            }  

            FillLists(cliente.IdEstado, cliente.IdRegiao, cliente.IdVendedor, cliente.Id, cliente.IdMatriz);

            ViewBag.Potencial = getListPotenciais(cliente.Potencial.ToString());

            return View(cliente);
        }

        // POST: Cliente/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,RazaoSocial,NomeFantasia,Cnpj,InscricaoEstadual,TelefonePrincipal,TelefoneContato,EmailPrincipal,EmailNFe,Observacao,Logradouro,Numero,Bairro,Cep,Cidade,IdEstado,IdRegiao,IdVendedor,NomeComprador,Potencial,Latilong,Situacao,IdMatriz")] Cliente cliente)
        {
            this.UpdateBag();

            if (ModelState.IsValid)
            {
                Service.Update(cliente);
                return RedirectToAction("Index");
            }      

            FillLists(cliente.IdEstado, cliente.IdRegiao, cliente.IdVendedor, cliente.Id, cliente.IdMatriz);

            ViewBag.ListPotencial = new SelectList(ViewBag.Potencial, "ListPotencial", "ListPotencial", cliente.Potencial.ToString());

            return View(cliente);
        }

        // GET: Cliente/Delete/5
        public IActionResult Delete(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Cliente cliente = Service.GetById(id.Value);
            if (cliente == null)
            {
                return new StatusCodeResult(404);
            }
            return View(cliente);
        }

        // POST: Cliente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            this.UpdateBag();
            Cliente cliente = Service.GetById(id);
            Service.Delete(cliente);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upload(IFormFile upload)
        {
            this.UpdateBag();
            FillLists();
            ViewBag.ListaInvalida = null;

            if (ModelState.IsValid)
            {
                if (upload != null && upload.Length > 0)
                {
                    if (upload.FileName.EndsWith(".csv"))
                    {
                        var lineCount = 0;
                        Stream stream = Request.Body;
                        using (var reader = new StreamReader(stream))
                        {

                            List<Cliente> clientList = new List<Cliente>();
                            while (!reader.EndOfStream)
                            {
                                lineCount++;

                                var line = reader.ReadLine();
                                var values = line.Split(';');

                                if (values.Length < 19)
                                {
                                    ViewBag.ListaInvalida = "A linha " + lineCount + " tem " + values.Length + " colunas. Corrija para 19 colunas antes de importar.";
                                }


                                Cliente cliente = new Cliente();
                                cliente.RazaoSocial = values[0].ToString();
                                cliente.NomeFantasia = values[1].ToString();
                                cliente.Cnpj = values[2].ToString();
                                cliente.InscricaoEstadual = values[3].ToString();
                                cliente.TelefonePrincipal = values[4].ToString();
                                cliente.TelefoneContato = values[5].ToString();
                                cliente.EmailPrincipal = values[6].ToString();
                                cliente.EmailNFe = values[7].ToString();
                                cliente.Observacao = values[8].ToString();
                                cliente.Logradouro = values[9].ToString();
                                cliente.Numero = values[10].ToString();
                                cliente.Bairro = values[11].ToString();
                                cliente.Cep = values[12].ToString();
                                cliente.Cidade = values[13].ToString();
                                Estado estado = EstadoService.List().Where(x => x.Sigla == values[14]).FirstOrDefault();
                                if (estado != null)
                                {
                                    cliente.IdEstado = estado.Id;
                                }
                                else
                                {
                                    cliente.IdEstado = 1;
                                }
                                Regiao regiao = RegiaoService.List().Where(x => x.Descricao == values[15]).FirstOrDefault();
                                if (regiao != null)
                                {
                                    cliente.IdRegiao = regiao.Id;
                                }
                                else
                                {
                                    cliente.IdRegiao = 1;
                                }
                                cliente.NomeComprador = values[16].ToString();
                                Vendedor vendedor = VendedorService.List().Where(x => x.Nome == values[17]).FirstOrDefault();
                                if (vendedor != null)
                                {
                                    cliente.IdVendedor = vendedor.Id;
                                }
                                else
                                {
                                    cliente.IdVendedor = 99;
                                }
                                var potenciais = new List<SelectListItem>
                                {
                                                new SelectListItem { Text = "Indefinido", Value="0" },
                                                new SelectListItem { Text = "Pequeno", Value="1" },
                                                new SelectListItem { Text = "Médio", Value="2" },
                                                new SelectListItem { Text = "Grande", Value="3" }
                                };
                                int potencial = Int32.Parse(potenciais.Find(x => x.Text == values[18]).Value);
                                if (vendedor != null)
                                {
                                    cliente.Potencial = potencial;
                                }
                                else
                                {
                                    cliente.Potencial = 0;
                                }

                                clientList.Add(cliente);

                            }

                            ViewBag.ImportCount = lineCount - 1;

                            if (ViewBag.ImportCount <= 0)
                            {
                                ViewBag.ListaInvalida = "Não há clientes nessa lista. Importe um csv delimitado por ; com um cliente por linha, até 100 linhas.";
                            }

                            foreach (Cliente c in clientList)
                            {
                                if (ModelState.IsValid)
                                {
                                    Service.Add(c);
                                }
                            }
                        }

                        return View("Import");
                    }
                    else
                    {
                        ModelState.AddModelError("File", "This file format is not supported");

                        ViewBag.ListaInvalida = "Importe um .csv delimitado por ; com um cliente por linha, até 100 linhas.";

                        return View("Import");
                    }
                }
                else
                {
                    ModelState.AddModelError("File", "Please Upload Your file");
                }
            }
            return View("Import");
        }

        // GET: Cliente/Import
        public IActionResult Import()
        {
            this.UpdateBag();
            return View();
        }

        //id = vendedorId
        public IActionResult Export(int id)
        {
            Vendedor vendedor = VendedorService.GetById(id);

            var header = "RAZAO_SOCIAL;NOME_FANTASIA;CNPJ;INSCRICAO_ESTADUAL;TELEFONE_PRINCIPAL;TELEFONE_CONTATO;EMAIL_PRINCIPAL;EMAIL_NFE;LOGRADOURO;NUMERO;BAIRRO;CEP;CIDADE;UF_ESTADO;REGIAO;NOME_COMPRADOR;NOME_VENDEDOR;POTENCIAL\n";

            //fetch mysql, concat by semicolon
            List<ExportClientes> gc = new Consultas().GetExportClientes(id);
            string stringClientes = header;
            foreach (ExportClientes tc in gc)
            {

                stringClientes = stringClientes + tc.Linha.Replace("\n", "").Replace("\r", "") + "\n";
            }
            var byteArray = Encoding.UTF8.GetBytes(stringClientes);
            //Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(byteArray, "text/csv", "Clientes-" + vendedor.Nome + ".csv");
        }


        // GET: Regiao/Todas
        public IActionResult MapaTodos()
        {
            ViewBag.Features = new Consultas().GetRegioesFeatures();
            ViewBag.Locais = new Consultas().GetLatiLongClientes();
            return View("MapaTodos", Service.List());
        }

        public void FillLists(int? idEstado = 0, int? idRegiao = 0, int? idVendedor = 0, int? idCliente = 0, int? idMatriz = 0)
        {
            if (idEstado > 0)
                ViewBag.IdEstado = new SelectList(EstadoService.List().OrderBy(m => m.Sigla), "Id", "Sigla", idEstado);
            else 
                ViewBag.IdEstado = new SelectList(EstadoService.List().OrderBy(m => m.Sigla), "Id", "Sigla");

            if (idRegiao > 0)                
                ViewBag.IdRegiao = new SelectList(RegiaoService.List().OrderBy(m => m.Descricao), "Id", "Descricao", idRegiao);
            else 
                ViewBag.IdRegiao = new SelectList(RegiaoService.List().OrderBy(m => m.Descricao), "Id", "Descricao");

            if (idVendedor > 0)
                if (ViewBag.EhAdmin)
                    ViewBag.IdVendedor = new SelectList(VendedorService.List().OrderBy(m => m.Nome), "Id", "Nome", idVendedor);
                else
                    ViewBag.IdVendedor = new SelectList(VendedorService.List()
                                                        .Where(x => x.Id == idVendedor)
                                                        .OrderBy(m => m.Nome), "Id", "Nome", idVendedor);
            else 
                ViewBag.IdVendedor = new SelectList(VendedorService.List().OrderBy(m => m.Nome), "Id", "Nome");

            if (idCliente > 0 && idMatriz > 0)
                ViewBag.IdMatriz = new SelectList(Service.List()
                                    .Where(x => x.Id != idCliente)
                                    .OrderBy(m => m.RazaoSocial), "Id", "RazaoSocial", idMatriz);
            else 
                ViewBag.IdMatriz = new SelectList(Service.List()
                                    .OrderBy(m => m.RazaoSocial), "Id", "RazaoSocial");
        }

        public void UpdateBag()
        {
            FillLists();
            
            ViewBag.Situacao = new List<SelectListItem>{
                            new SelectListItem { Text = "Ativo", Value="A" },
                            new SelectListItem { Text = "Inativo", Value="I" }
            };

            ViewBag.Potencial = new List<SelectListItem>
            {
                            new SelectListItem { Text = "Indefinido", Value="0" },
                            new SelectListItem { Text = "Pequeno", Value="1" },
                            new SelectListItem { Text = "Médio", Value="2" },
                            new SelectListItem { Text = "Grande", Value="3" }
            };

            if (_httpContextAccessor?.HttpContext?.Request.Cookies["displayName"] == null)
            {
                return;
            }
           
            var idVendedorLogado = Convert.ToInt32(_httpContextAccessor?.HttpContext?.Request.Cookies["idVendedorLogado"]);
            var vendedor = VendedorService.GetById(idVendedorLogado);

            ViewBag.EhAdmin = vendedor.Admin == 1;
            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = idVendedorLogado;
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
