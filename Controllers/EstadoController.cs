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

namespace GEPosVendas.Controllers
{
    public class EstadoController : Controller
    {
        private EstadoService Service = new EstadoService(new EstadoRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Estado
        public IActionResult Index()
        {
            this.UpdateBag();
            return View(Service.List());
        }

        // GET: Estado/Details/5
        public IActionResult Details(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Estado estado = Service.GetById(id.Value);
            if (estado == null)
            {
                return new StatusCodeResult(404);
            }
            return View(estado);
        }

        // GET: Estado/Create
        public IActionResult Create()
        {
            this.UpdateBag();
            return View();
        }

        // POST: Estado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descricao,Sigla")] Estado estado)
        {
            this.UpdateBag();
            if (ModelState.IsValid)
            {
                Service.Add(estado);
                return RedirectToAction("Index");
            }

            return View(estado);
        }

        // GET: Estado/Edit/5
        public IActionResult Edit(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Estado estado = Service.GetById(id.Value);
            if (estado == null)
            {
                return new StatusCodeResult(404);
            }
            return View(estado);
        }

        // POST: Estado/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Descricao,Sigla")] Estado estado)
        {
            this.UpdateBag();
            if (ModelState.IsValid)
            {
                Service.Update(estado);
                return RedirectToAction("Index");
            }
            return View(estado);
        }

        // GET: Estado/Delete/5
        public IActionResult Delete(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Estado estado = Service.GetById(id.Value);
            if (estado == null)
            {
                return new StatusCodeResult(404);
            }
            return View(estado);
        }

        // POST: Estado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            this.UpdateBag();
            Estado estado = Service.GetById(id);
            Service.Delete(estado);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Service.Dispose();
            }
            base.Dispose(disposing);
        }

        public void UpdateBag()
        {

            if (_httpContextAccessor?.HttpContext?.Request.Cookies["displayName"] == null)
            {
                return;
            }

            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdVendedorLogado = Convert.ToInt32(_httpContextAccessor?.HttpContext?.Request.Cookies["idVendedorLogado"]);
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
