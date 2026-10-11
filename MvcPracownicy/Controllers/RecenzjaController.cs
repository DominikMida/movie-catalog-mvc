using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Models;

namespace MvcPracownicy.Controllers
{
    public class RecenzjaController : BaseController
    {
        private readonly MovieContext _context;

        public RecenzjaController(MovieContext context)
        {
            _context = context;
        }

        // GET: Recenzja
        public async Task<IActionResult> Index()
        {
            var recenzje = _context.Recenzja.Include(r => r.Film).AsNoTracking();
            return View(await recenzje.ToListAsync());
        }

        // GET: Recenzja/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzja = await _context.Recenzja
                .Include(r => r.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recenzja == null)
            {
                return NotFound();
            }

            return View(recenzja);
        }

        // GET: Recenzja/Create
        public IActionResult Create()
        {
            PopulateFilmDropDownList();
            return View();
        }

                // POST: Recenzja/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Autor,Ocena,Komentarz,FilmId")] Recenzja recenzja)
        {
            ModelState.Remove("Film");

            Film? film = null;
            if (recenzja.FilmId > 0)
            {
                film = await _context.Film.FindAsync(recenzja.FilmId);
            }

            if (film == null)
            {
                ModelState.AddModelError("FilmId", "Musisz wybrać film.");
                PopulateFilmDropDownList(recenzja.FilmId);
                return View(recenzja);
            }

            if (ModelState.IsValid)
            {
                recenzja.Film = film;
                _context.Add(recenzja);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateFilmDropDownList(recenzja.FilmId);
            return View(recenzja);
        }


        // GET: Recenzja/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzja = await _context.Recenzja
                .Include(r => r.Film)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recenzja == null)
            {
                return NotFound();
            }

            PopulateFilmDropDownList(recenzja.FilmId);
            return View(recenzja);
        }

                // POST: Recenzja/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> Edit(int id, [Bind("Id,Autor,Ocena,Komentarz,FilmId")] Recenzja recenzja)
            {
                if (id != recenzja.Id)
                {
                    return NotFound();
                }

                ModelState.Remove("Film");

                Film? film = null;
                if (recenzja.FilmId > 0)
                {
                    film = await _context.Film.FindAsync(recenzja.FilmId);
                }

                if (film == null)
                {
                    ModelState.AddModelError("FilmId", "Musisz wybrać film.");
                    PopulateFilmDropDownList(recenzja.FilmId);
                    return View(recenzja);
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingRecenzja = await _context.Recenzja
                            .Where(r => r.Id == id)
                            .Include(r => r.Film)
                            .FirstOrDefaultAsync();

                        if (existingRecenzja == null)
                        {
                            return NotFound();
                        }

                        existingRecenzja.Autor = recenzja.Autor;
                        existingRecenzja.Ocena = recenzja.Ocena;
                        existingRecenzja.Komentarz = recenzja.Komentarz;
                        existingRecenzja.Film = film;
                        existingRecenzja.FilmId = film.Id;

                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RecenzjaExists(recenzja.Id))
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

                PopulateFilmDropDownList(recenzja.FilmId);
                return View(recenzja);
            }
        // GET: Recenzja/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recenzja = await _context.Recenzja
                .Include(r => r.Film)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recenzja == null)
            {
                return NotFound();
            }

            return View(recenzja);
        }

        // POST: Recenzja/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recenzja = await _context.Recenzja.FindAsync(id);
            if (recenzja != null)
            {
                _context.Recenzja.Remove(recenzja);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateFilmDropDownList(object? selectedFilm = null)
        {
            var filmyQuery = from f in _context.Film
                             orderby f.Tytul
                             select f;
            ViewBag.FilmId = new SelectList(filmyQuery.AsNoTracking(), "Id", "Tytul", selectedFilm);
        }

        private bool RecenzjaExists(int id)
        {
            return _context.Recenzja.Any(e => e.Id == id);
        }
    }
}