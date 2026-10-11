using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MvcPracownicy.Helpers;
using MvcPracownicy.Models;
using System.Linq;

namespace MvcPracownicy.Data;

public static class Db
{
    private static string connectionString = "Data Source=app.db";

    public static SqliteConnection GetConnection()
    {
        var con = new SqliteConnection(connectionString);
        con.Open();
        return con;
    }

    public static void Init()
    {
        using var con = GetConnection();

        var cmd = con.CreateCommand();

        cmd.CommandText =
        @"
        CREATE TABLE IF NOT EXISTS loginy (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Login TEXT NOT NULL UNIQUE,
            Haslo TEXT NOT NULL,
            IsAdmin INTEGER DEFAULT 0,
            Token TEXT NOT NULL DEFAULT ''
        );

        CREATE TABLE IF NOT EXISTS dane (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Tekst TEXT NOT NULL
        );
        ";
        cmd.ExecuteNonQuery();


        cmd = con.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM loginy";
        long count = (long)cmd.ExecuteScalar();

        if (count == 0)
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO loginy (Login, Haslo, IsAdmin, Token) VALUES (@login, @haslo, 1, @token)";
            cmd.Parameters.AddWithValue("@login", "dominik");
            cmd.Parameters.AddWithValue("@haslo", HashHelper.GetMd5Hash("mida"));
            cmd.Parameters.AddWithValue("@token", "dominik-token-2026");
            cmd.ExecuteNonQuery();
            cmd = con.CreateCommand();
            cmd.CommandText = "INSERT INTO loginy (Login, Haslo, Token) VALUES (@login, @haslo, @token)";
            cmd.Parameters.AddWithValue("@login", "user");
            cmd.Parameters.AddWithValue("@haslo", HashHelper.GetMd5Hash("user"));
            cmd.Parameters.AddWithValue("@token", "user-token-2026");
            cmd.ExecuteNonQuery();
        }

        cmd = con.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM dane";
        count = (long)cmd.ExecuteScalar();

        if (count == 0)
        {
            cmd = con.CreateCommand();
            cmd.CommandText =
            @"
            INSERT INTO dane (Tekst)
            VALUES ('Pierwszy rekord'), ('Drugi rekord')
            ";
            cmd.ExecuteNonQuery();
        }
    }

    public static bool ValidateApiToken(string login, string token)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(token))
            return false;

        using var con = GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM loginy WHERE Login = $login AND Token = $token";
        cmd.Parameters.AddWithValue("$login", login);
        cmd.Parameters.AddWithValue("$token", token);

        var count = (long)cmd.ExecuteScalar();
        return count > 0;
    }


    private static string GenerateApiToken(string login)
    {
        return login + "-token-" + Guid.NewGuid().ToString("N").Substring(0, 8);
    }

    public static void Init(MovieContext context)
    {
          try
        {
            Init();
            context.Database.Migrate();
        }
        catch
        {
            context.Database.EnsureCreated();
        }
        SeedMovieData(context);
    }

    private static void SeedMovieData(MovieContext context)
    {
        if (!context.Studio.Any())
        {
            context.Studio.AddRange(
                new Studio { Nazwa = "Warner Bros." },
                new Studio { Nazwa = "Universal Pictures" },
                new Studio { Nazwa = "Pixar" },
                new Studio { Nazwa = "Paramount" },
                new Studio { Nazwa = "Netflix" }
            );
            context.SaveChanges();
        }

        if (!context.Gatunek.Any())
        {
            context.Gatunek.AddRange(
                new Gatunek { Nazwa = "Akcja" },
                new Gatunek { Nazwa = "Komedia" },
                new Gatunek { Nazwa = "Dramat" },
                new Gatunek { Nazwa = "Sci-Fi" },
                new Gatunek { Nazwa = "Horror" }
            );
            context.SaveChanges();
        }

        if (!context.Film.Any())
        {
            var studios = context.Studio.ToList();
            var gatunki = context.Gatunek.ToList();

            context.Film.AddRange(
                new Film { Tytul = "Matrix", Rok = 1999, StudioId = studios[0].Id, GatunekId = gatunki.First(g => g.Nazwa == "Sci-Fi").Id },
                new Film { Tytul = "Incepcja", Rok = 2010, StudioId = studios[1].Id, GatunekId = gatunki.First(g => g.Nazwa == "Sci-Fi").Id },
                new Film { Tytul = "Django", Rok = 2012, StudioId = studios[2].Id, GatunekId = gatunki.First(g => g.Nazwa == "Dramat").Id },
                new Film { Tytul = "Kac Vegas", Rok = 2009, StudioId = studios[3].Id, GatunekId = gatunki.First(g => g.Nazwa == "Komedia").Id },
                new Film { Tytul = "Obecność", Rok = 2013, StudioId = studios[4].Id, GatunekId = gatunki.First(g => g.Nazwa == "Horror").Id }
            );
            context.SaveChanges();
        }

        if (!context.Recenzja.Any())
        {
            var films = context.Film.ToList();

            context.Recenzja.AddRange(
                new Recenzja { Autor = "Anna", Ocena = 9, Komentarz = "Świetny film z efektami.", FilmId = films[0].Id },
                new Recenzja { Autor = "Piotr", Ocena = 8, Komentarz = "Bardzo dobra fabuła.", FilmId = films[1].Id },
                new Recenzja { Autor = "Kasia", Ocena = 7, Komentarz = "Silne postaci i dramat.", FilmId = films[2].Id },
                new Recenzja { Autor = "Marek", Ocena = 8, Komentarz = "Śmieszny i dobrze zagrany.", FilmId = films[3].Id },
                new Recenzja { Autor = "Ewa", Ocena = 6, Komentarz = "Straszny i klimatyczny.", FilmId = films[4].Id }
            );
            context.SaveChanges();
        }
    }
}