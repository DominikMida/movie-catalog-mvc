using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class GatunekController : BaseController
    {
        private readonly MovieContext _context;

        public GatunekController(MovieContext context)
        {
            _context = context;
        }

        // GET: Gatunek
        public async Task<IActionResult> Index()
        {
            return View(await _context.Gatunek.ToListAsync());
        }

        // GET: Gatunek/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gatunek = await _context.Gatunek
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gatunek == null)
            {
                return NotFound();
            }

            return View(gatunek);
        }

        // GET: Gatunek/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Gatunek/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nazwa")] Gatunek gatunek)
        {
            if (ModelState.IsValid)
            {
                _context.Add(gatunek);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gatunek);
        }

        // GET: Gatunek/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gatunek = await _context.Gatunek.FindAsync(id);
            if (gatunek == null)
            {
                return NotFound();
            }
            return View(gatunek);
        }

        // POST: Gatunek/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nazwa")] Gatunek gatunek)
        {
            if (id != gatunek.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gatunek);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GatunekExists(gatunek.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gatunek);
        }

        // GET: Gatunek/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gatunek = await _context.Gatunek
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gatunek == null)
            {
                return NotFound();
            }

            return View(gatunek);
        }

        // POST: Gatunek/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gatunek = await _context.Gatunek.FindAsync(id);
            if (gatunek != null)
            {
                _context.Gatunek.Remove(gatunek);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GatunekExists(int id)
        {
            return _context.Gatunek.Any(e => e.Id == id);
        }
    }
}
