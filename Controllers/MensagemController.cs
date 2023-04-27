using Microsoft.AspNetCore.Mvc;
using GEPV.Domain.Entities;
using GEPV.Domain.Repository;
using GEPV.Domain.Services;

namespace GEPosVendas.Controllers
{
    public class MensagemController : Controller
    {
        private MensagemService Service = new MensagemService(new MensagemRepository());
        private readonly IHttpContextAccessor? _httpContextAccessor = default;

        // GET: Mensagem
        public IActionResult Index()
        {
            return View(Service.List());
        }

        // GET: Mensagem/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Mensagem msg = Service.GetById(id.Value);
            if (msg == null)
            {
                return new StatusCodeResult(404);
            }
            return View(msg);
        }

        // GET: Mensagem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mensagem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Texto,DataValidade,Tipo")] Mensagem msg)
        {

            if (ModelState.IsValid)
            {
                Service.Add(msg);
                return RedirectToAction("Index");
            }

            return View(msg);
        }

        // GET: Mensagem/Edit/5
        public IActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Mensagem msg = Service.GetById(id.Value);
            if (msg == null)
            {
                return new StatusCodeResult(404);
            }

            return View(msg);
        }

        // POST: Mensagem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("Id,Texto,DataValidade,Tipo")] Mensagem msg)
        {

            if (ModelState.IsValid)
            {
                Service.Update(msg);
                return RedirectToAction("Index");
            }
            return View(msg);
        }

        // GET: Mensagem/Delete/5
        public IActionResult Delete(int? id)
        {

            if (id == null)
            {
                return new StatusCodeResult(400);
            }
            Mensagem msg = Service.GetById(id.Value);
            if (msg == null)
            {
                return new StatusCodeResult(404);
            }
            return View(msg);
        }

        // POST: Mensagem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Mensagem msg = Service.GetById(id);
            Service.Delete(msg);
           
            return RedirectToAction("Index");
        }

    }
}
