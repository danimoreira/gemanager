using GEPV.Domain.Entities;
using GEPV.Domain.Interfaces.Services;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using GEPV.Domain.SQL;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GEPosVendas.Controllers
{
    public class FornecedorController : Controller
    {
        private FornecedorService Service = new FornecedorService(new FornecedorRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Fornecedor
        public IActionResult Index()
        {
            this.UpdateBag();
            List<Fornecedor> fornecedores = Service.List();
            return View(fornecedores);
        }

        // GET: Fornecedor/Details/5
        public IActionResult Details(int id)
        {
            this.UpdateBag();
            Fornecedor fornecedor = Service.List().Where(x => x.Id == id).FirstOrDefault();
            return View(fornecedor);
        }

        // GET: Fornecedor/Create
        public IActionResult Create()
        {
            this.UpdateBag();
            return View();
        }

        // POST: Fornecedor/Create
        [HttpPost]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                this.UpdateBag();
                // TODO: Add insert logic here
                Fornecedor fornecedor = new Fornecedor();

                fornecedor.NomeFantasia = collection["NomeFantasia"];
                fornecedor.Sigla = collection["Sigla"];
                fornecedor.Observacao = collection["Observacao"];

                Service.Add(fornecedor);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Fornecedor/Edit/5
        public IActionResult Edit(int id)
        {
            this.UpdateBag();
            Fornecedor fornecedor = Service.List().Where(x => x.Id == id).FirstOrDefault();
            return View(fornecedor);
        }

        // POST: Fornecedor/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                this.UpdateBag();
                // TODO: Add update logic here
                Fornecedor fornecedor = new Fornecedor();

                fornecedor.Id = id;
                fornecedor.NomeFantasia = collection["NomeFantasia"];
                fornecedor.Sigla = collection["Sigla"];
                fornecedor.Observacao = collection["Observacao"];

                Service.Update(fornecedor);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Fornecedor/Delete/5
        public IActionResult Delete(int id)
        {
            this.UpdateBag();
            Fornecedor fornecedor = Service.List().Where(x => x.Id == id).FirstOrDefault();
            return View(fornecedor);
        }

        // POST: Fornecedor/Delete/5
        [HttpPost]
        public IActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                this.UpdateBag();
                // TODO: Add delete logic here
                Fornecedor fornecedor = Service.List().Where(x => x.Id == id).FirstOrDefault();

                Service.Delete(fornecedor);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public void UpdateBag()
        {
            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = Convert.ToInt32(_httpContextAccessor?.HttpContext?.Request.Cookies["idVendedorLogado"]);
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
