using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Models;

    public class MovieContext : DbContext
    {
        public MovieContext (DbContextOptions<MovieContext> options)
            : base(options)
        {
        }

        public DbSet<MvcPracownicy.Models.Film> Film { get; set; } = default!;

public DbSet<MvcPracownicy.Models.Studio> Studio { get; set; } = default!;

public DbSet<MvcPracownicy.Models.Gatunek> Gatunek { get; set; } = default!;

public DbSet<MvcPracownicy.Models.Recenzja> Recenzja { get; set; } = default!;
    }
