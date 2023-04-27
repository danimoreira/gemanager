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

namespace GEPosVendas.Controllers
{
    public class FeriadoController : Controller
    {
        private FeriadoService Service = new FeriadoService(new FeriadoRepository());
        private EstadoService EstadoService = new EstadoService(new EstadoRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Feriado
        public IActionResult Index()
        {
            this.UpdateBag();
            return View(Service.List());
        }

        // GET: Feriado/Details/5
        public IActionResult Details(int? id)
        {
            this.UpdateBag();
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Feriado frd = Service.GetById(id.Value);
            if (frd == null)
            {
                return new StatusCodeResult(404);
            }
            return View(frd);
        }

        // GET: Feriado/Create
        public IActionResult Calendario()
        {
            this.UpdateBag();
            return View();
        }

        // GET: Feriado/Create
        public IActionResult Create()
        {
            List<Estado> estados = EstadoService.List();
            List<SelectListItem> selectEstados = new List<SelectListItem>();
            selectEstados.Add(new SelectListItem { Text = "Escolha o estado", Value = null, Selected = true });
            foreach (Estado e in estados)
            {
                selectEstados.Add(new SelectListItem { Text = e.Descricao, Value = e.Sigla });
            }
            ViewBag.Uf = selectEstados;

            this.UpdateBag();
            return View();
        }

        // POST: Feriado/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Data,Tipo,Nome,Descricao,Uf,Municipio")] Feriado frd)
        {
            this.UpdateBag();

            if (ModelState.IsValid)
            {
                Service.Add(frd);
                return RedirectToAction("Index");
            }

            return View(frd);
        }

        // GET: Feriado/Edit/5
        public IActionResult Edit(int? id)
        {
            this.UpdateBag();

            if (id == null)
            {
                return new StatusCodeResult(400);
            }

            Feriado frd = Service.GetById(id.Value);

            List<Estado> estados = EstadoService.List();
            List<SelectListItem> selectEstados = new List<SelectListItem>();
            selectEstados.Add(new SelectListItem { Text = "Escolha o estado", Value = null, Selected = true });
            foreach (Estado e in estados)
            {
                selectEstados.Add(new SelectListItem { Text = e.Descricao, Value = e.Sigla, Selected = e.Sigla == frd.Uf  });
            }
            ViewBag.Uf = selectEstados;
       

            if (frd == null)
            {
                return new StatusCodeResult(404);
            }
            return View(frd);
        }

        // POST: Feriado/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Data,Tipo,Nome,Descricao,Uf,Municipio")] Feriado frd)
        {
            this.UpdateBag();

            if (ModelState.IsValid)
            {
                Service.Update(frd);
                return RedirectToAction("Index");
            }
            return View(frd);
        }

        // GET: Feriado/Delete/5
        public IActionResult Delete(int? id)
        {
            this.UpdateBag();

            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Feriado frd = Service.GetById(id.Value);
            if (frd == null)
            {
                return new StatusCodeResult(404);
            }
            return View(frd);
        }

        // POST: Feriado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            this.UpdateBag();

            Feriado frd = Service.GetById(id);
            Service.Delete(frd);
           
            return RedirectToAction(
                "Index");
        }

        public void UpdateBag()
        {

            if (_httpContextAccessor?.HttpContext?.Request.Cookies["displayName"] == null)
            {
                return;
            }


            ViewBag.Usuario = _httpContextAccessor?.HttpContext?.Request.Cookies["displayName"];
            ViewBag.IdFeriadoLogado = Convert.ToInt32(_httpContextAccessor?.HttpContext?.Request.Cookies["idVendedorLogado"]);
            ViewBag.DataAtual = DateTime.Now.ToString("dd/MM/yyyy");
        }
    }
}
