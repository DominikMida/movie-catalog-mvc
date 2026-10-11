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
    public class FilmController : BaseController
    {
        private readonly MovieContext _context;

        public FilmController(MovieContext context)
        {
            _context = context;
        }

        // GET: Film
        public async Task<IActionResult> Index()
        {
            var filmy = _context.Film
                .Include(f => f.Gatunek)
                .Include(f => f.Studio)
                .AsNoTracking();
            return View(await filmy.ToListAsync());
        }

        // GET: Film/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Gatunek)
                .Include(f => f.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // GET: Film/Create
        public IActionResult Create()
        {
            PopulateStudioDropDownList();
            PopulateGatunekDropDownList();
            return View();
        }

        // POST: Film/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Tytul,Rok,StudioId,GatunekId")] Film film,
            IFormCollection form)
        {
            string gatunekValue = form["GatunekId"].ToString();
            string studioValue = form["StudioId"].ToString();

            Gatunek gatunek = null;
            if (gatunekValue != "-1")
            {
                var gg = _context.Gatunek.Where(g => g.Id == int.Parse(gatunekValue));
                if (gg.Any())
                    gatunek = gg.First();
            }

            Studio studio = null;
            if (studioValue != "-1")
            {
                var ss = _context.Studio.Where(s => s.Id == int.Parse(studioValue));
                if (ss.Any())
                    studio = ss.First();
            }

            if (ModelState.IsValid)
            {
                film.Gatunek = gatunek;
                film.Studio = studio;
                film.GatunekId = gatunek?.Id;
                film.StudioId = studio?.Id;

                _context.Add(film);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateGatunekDropDownList(film.GatunekId);
            PopulateStudioDropDownList(film.StudioId);
            return View(film);
        }

        // GET: Film/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Include(f => f.Gatunek)
                .Include(f => f.Studio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (film == null)
            {
                return NotFound();
            }

            PopulateStudioDropDownList(film.StudioId);
            PopulateGatunekDropDownList(film.GatunekId);

            return View(film);
        }

        // POST: Film/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Tytul,Rok,StudioId,GatunekId")] Film film,
            IFormCollection form)
        {
            if (id != film.Id)
            {
                return NotFound();
            }

            string gatunekValue = form["GatunekId"];
            string studioValue = form["StudioId"];

            Gatunek gatunek = null;
            if (gatunekValue != "-1")
            {
                var gg = _context.Gatunek.Where(g => g.Id == int.Parse(gatunekValue));
                if (gg.Any())
                    gatunek = gg.First();
            }

            Studio studio = null;
            if (studioValue != "-1")
            {
                var ss = _context.Studio.Where(s => s.Id == int.Parse(studioValue));
                if (ss.Any())
                    studio = ss.First();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingFilm = await _context.Film
                        .Where(f => f.Id == id)
                        .Include(f => f.Gatunek)
                        .Include(f => f.Studio)
                        .FirstAsync();

                    existingFilm.Tytul = film.Tytul;
                    existingFilm.Rok = film.Rok;
                    existingFilm.Gatunek = gatunek;
                    existingFilm.Studio = studio;
                    existingFilm.GatunekId = gatunek?.Id;
                    existingFilm.StudioId = studio?.Id;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FilmExists(film.Id))
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

            PopulateGatunekDropDownList(film.GatunekId);
            PopulateStudioDropDownList(film.StudioId);
            return View(film);
        }

        // GET: Film/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var film = await _context.Film
                .Where(f => f.Id == id)
                .Include(f => f.Gatunek)
                .Include(f => f.Studio)
                .FirstOrDefaultAsync();

            if (film == null)
            {
                return NotFound();
            }

            return View(film);
        }

        // POST: Film/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var film = await _context.Film.FindAsync(id);
            if (film != null)
            {
                _context.Film.Remove(film);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private void PopulateStudioDropDownList(object selectedStudio = null)
        {
            var studiosQuery = from s in _context.Studio
                               orderby s.Nazwa
                               select s;
            ViewBag.StudioId = new SelectList(studiosQuery.AsNoTracking(), "Id", "Nazwa", selectedStudio);
        }

        private void PopulateGatunekDropDownList(object selectedGatunek = null)
        {
            var gatunkiQuery = from g in _context.Gatunek
                               orderby g.Nazwa
                               select g;
            ViewBag.GatunekId = new SelectList(gatunkiQuery.AsNoTracking(), "Id", "Nazwa", selectedGatunek);
        }


        // GET: Film/Ranking
        public async Task<IActionResult> Ranking()
        {
            var filmy = await _context.Film
                .Include(f => f.Gatunek)
                .Include(f => f.Studio)
                .Include(f => f.Recenzje)
                .AsNoTracking()
                .ToListAsync();

            var ranking = filmy
                .Select(f => new FilmRankingViewModel
                {
                    Id = f.Id,
                    Tytul = f.Tytul,
                    Rok = f.Rok,
                    GatunekNazwa = f.Gatunek != null ? f.Gatunek.Nazwa : "Brak",
                    StudioNazwa = f.Studio != null ? f.Studio.Nazwa : "Brak",
                    SredniaOcena = f.Recenzje != null && f.Recenzje.Count > 0 
                        ? Math.Round((double)f.Recenzje.Average(r => r.Ocena), 2)
                        : 0,
                    LiczbaRecenzji = f.Recenzje != null ? f.Recenzje.Count : 0
                })
                .OrderByDescending(x => x.SredniaOcena)
                .ThenBy(x => x.Tytul)
                .ToList();

            return View(ranking);
        }

        private bool FilmExists(int id)
        {
            return _context.Film.Any(e => e.Id == id);
        }
    }
}