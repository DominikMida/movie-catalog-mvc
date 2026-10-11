using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using MvcPracownicy.Data;
namespace MvcPracownicy.Controllers;
using MvcPracownicy.Helpers;
using MvcPracownicy.Models;

public class AccountController : Controller
{
    private bool IsLogged()
    {
        return HttpContext.Session.GetString("user") != null;
    }

    private bool IsAdmin()
    {
        return HttpContext.Session.GetString("isAdmin") == "1";
    }

    [HttpGet]
    public IActionResult Logowanie()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Logowanie(string login, string haslo)
    {
        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText =
        @"
        SELECT IsAdmin FROM loginy
        WHERE Login = $login AND Haslo = $haslo
        ";
        var hasloHash = HashHelper.GetMd5Hash(haslo);
        cmd.Parameters.AddWithValue("$login", login);
        cmd.Parameters.AddWithValue("$haslo", hasloHash);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            int isAdmin = reader.GetInt32(0);

            HttpContext.Session.SetString("user", login);
            HttpContext.Session.SetString("isAdmin", isAdmin.ToString());
            return RedirectToAction("Panel");
        }

        ViewBag.Error = "Błędne dane";
        return View();
    }


    public IActionResult Panel()
    {
        if (!IsLogged())
            return RedirectToAction("Logowanie");

        return View();
    }

    public IActionResult Wyloguj()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Logowanie");
    }

    public IActionResult Dane()
    {
        if (HttpContext.Session.GetString("user") == null)
            return RedirectToAction("Logowanie");

        List<string> lista = new();

        using var con = Data.Db.GetConnection();

        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Tekst FROM dane";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(reader.GetString(0));
        }

        return View(lista);
    }

    [HttpPost]
    public IActionResult DodajDana(string tekst)
    {
        if (HttpContext.Session.GetString("user") == null)
            return RedirectToAction("Logowanie");

        using var con = Data.Db.GetConnection();

        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO dane (Tekst) VALUES ($t)";
        cmd.Parameters.AddWithValue("$t", tekst);

        cmd.ExecuteNonQuery();

        return RedirectToAction("Dane");
    }

    public IActionResult Uzytkownicy()
    {
        if (!IsLogged() || !IsAdmin())
            return RedirectToAction("Logowanie");

        List<UzytkownikViewModel> lista = new();

        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Login, IsAdmin, Token FROM loginy ORDER BY IsAdmin DESC, Login";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            lista.Add(new UzytkownikViewModel
            {
                Id = reader.GetInt32(0),
                Login = reader.GetString(1),
                IsAdmin = reader.GetInt32(2) == 1,
                Token = reader.GetString(3)
            });
        }

        return View(lista);
    }

    [HttpGet]
    public IActionResult DodajUzytkownika()
    {
        if (!IsLogged() || !IsAdmin())
            return RedirectToAction("Logowanie");

        return View();
    }

    [HttpPost]
    public IActionResult DodajUzytkownika(string login, string haslo, string hasloPotwierdzenie)
    {
        if (!IsLogged() || !IsAdmin())
            return RedirectToAction("Logowanie");

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(haslo))
        {
            ViewBag.Error = "Login i hasło są wymagane";
            return View();
        }

        if (haslo != hasloPotwierdzenie)
        {
            ViewBag.Error = "Hasła nie zgadzają się";
            return View();
        }

        if (haslo.Length < 6)
        {
            ViewBag.Error = "Hasło musi mieć co najmniej 6 znaków";
            return View();
        }

        using var con = Db.GetConnection();

        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM loginy WHERE Login = $login";
        cmd.Parameters.AddWithValue("$login", login);
        long count = (long)cmd.ExecuteScalar();

        if (count > 0)
        {
            ViewBag.Error = "Login już istnieje w systemie";
            return View();
        }

        var token = "token-" + Guid.NewGuid().ToString("N").Substring(0, 8);
        cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO loginy (Login, Haslo, IsAdmin, Token) VALUES ($login, $haslo, 0, $token)";
        cmd.Parameters.AddWithValue("$login", login);
        cmd.Parameters.AddWithValue("$haslo", HashHelper.GetMd5Hash(haslo));
        cmd.Parameters.AddWithValue("$token", token);

        try
        {
            cmd.ExecuteNonQuery();
            ViewBag.Success = "Użytkownik został pomyślnie dodany";
            return RedirectToAction("Uzytkownicy");
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Błąd przy dodawaniu użytkownika: {ex.Message}";
            return View();
        }
    }

    [HttpPost]
    public IActionResult UsunUzytkownika(int id)
    {
        if (!IsLogged() || !IsAdmin())
            return RedirectToAction("Logowanie");

        if (id == 1)
        {
            ViewBag.Error = "Nie można usunąć głównego administratora";
            return RedirectToAction("Uzytkownicy");
        }

        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM loginy WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        cmd.ExecuteNonQuery();

        return RedirectToAction("Uzytkownicy");
    }
}
