using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Web;
using GEPV.Domain.Entities;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;
using GEPV.Domain.SQL;
using Microsoft.AspNetCore.Mvc.Rendering;
using GEPV.Domain.DTO;

namespace GEPosVendas.Controllers
{
    public class VendedorController : Controller
    {
        private VendedorService Service = new VendedorService(new VendedorRepository());
        private ClienteService ClienteService = new ClienteService(new ClienteRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;


        // GET: vendedor
        public IActionResult Index()
        {
            
            return View(Service.List());
        }

        // GET: vendedor/Details/5
        public IActionResult Details(int? id)
        {            
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Vendedor vendedor = Service.GetById(id.Value);
            if (vendedor == null)
            {
                return new StatusCodeResult(404);
            }
            return View(vendedor);
        }

        // GET: vendedor/Create
        public IActionResult Create()
        {
            ViewBag.TiposDeUsuario = this.getTiposDeUsuario();
            return View();
        }

        // POST: vendedor/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Nome,DataNascimento,Email,Usuario,Senha,Admin,ConfirmarSenha")] Vendedor vendedor)
        {
            if (ModelState.IsValid)
            {
                Service.Add(vendedor);
                return RedirectToAction("Index");
            }

            return View(vendedor);
        }

        // GET: vendedor/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Vendedor vendedor = Service.GetById(id.Value);
            if (vendedor == null)
            {
                return new StatusCodeResult(404);
            }

            ViewBag.TiposDeUsuario = this.getTiposDeUsuario(vendedor.Admin.ToString());

            return View(vendedor);
        }

        // POST: vendedor/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Nome,DataNascimento,Email,Usuario,Senha,Admin,ConfirmarSenha")] Vendedor vendedor)
        {
            if (ModelState.IsValid)
            {
                Service.Update(vendedor);
                return RedirectToAction("Index");
            }
            return View(vendedor);
        }

        // GET: vendedor/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }

            Vendedor vendedor = Service.GetById(id.Value);
            if (vendedor == null)
            {
                return new StatusCodeResult(404);
            }
            return View(vendedor);
        }

        private List<SelectListItem> getTiposDeUsuario(string defValue = "0")
        {

            var list = new List<SelectListItem> {
                            new SelectListItem { Text = "Vendedor", Value="0", Selected = false },
                            new SelectListItem { Text = "Administrador", Value="1", Selected = false }
            };

            foreach (SelectListItem sl in list)
            {
                if (sl.Value == defValue)
                {
                    sl.Selected = true;
                }
            }

            return list;

        }

        // POST: vendedor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Vendedor vendedor = Service.GetById(id);

            var clientes = ClienteService.List().Where(c => c.IdVendedor == id).ToList();

            Service.Delete(vendedor);

            foreach(Cliente cliente in clientes) {
                cliente.IdVendedor = null;
                ClienteService.Update(cliente);
            }
            return RedirectToAction("Index");
        }
    }
}
