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
    public class RegiaoController : Controller
    {
        private RegiaoService Service = new RegiaoService(new RegiaoRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Regiao
        public IActionResult Index()
        {
            this.UpdateBag();
            return View(Service.List());
        }

        // GET: Regiao/Details/5
        public IActionResult Details(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Regiao regiao = Service.GetById(id.Value);
            if (regiao == null)
            {
                return new StatusCodeResult(404);
            }
            return View(regiao);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Service.Dispose();
            }
            base.Dispose(disposing);
        }


        // GET: Regiao/Create
        public IActionResult Create()
        {
            this.UpdateBag();
            return View();
        }

        // POST: Regiao/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Descricao,Feature")] Regiao regiao)
        {
            this.UpdateBag();
            if (ModelState.IsValid)
            {
                Service.Add(regiao);                
                return RedirectToAction("Index");
            }

            return View(regiao);
        }

        // GET: Regiao/Edit/5
        public IActionResult Edit(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Regiao regiao = Service.GetById(id.Value);
            if (regiao == null)
            {
                return new StatusCodeResult(404);
            }
            return View(regiao);
        }

        // POST: Regiao/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Descricao,Feature")] Regiao regiao)
        {
            this.UpdateBag();
            if (ModelState.IsValid)
            {
                Service.Update(regiao);
                return RedirectToAction("Index");
            }
            return View(regiao);
        }

        // GET: Regiao/Delete/5
        public IActionResult Delete(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Regiao regiao = Service.GetById(id.Value);
            if (regiao == null)
            {
                return new StatusCodeResult(404);
            }
            return View(regiao);
        }

        // POST: Regiao/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            this.UpdateBag();
            Regiao regiao = Service.GetById(id);
            Service.Delete(id);
            return RedirectToAction("Index");
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
