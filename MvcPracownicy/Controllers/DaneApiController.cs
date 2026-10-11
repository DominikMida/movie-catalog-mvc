using Microsoft.AspNetCore.Mvc;
using MvcPracownicy.Data;
using MvcPracownicy.Helpers;
using MvcPracownicy.Models;
using Microsoft.Data.Sqlite;

namespace MvcPracownicy.Controllers;

[ApiController]
[Route("api/Dane")]
public class DaneApiController : ControllerBase
{
    [HttpGet("test")]
    public IActionResult TestConnection()
    {
        return Ok(new { message = "API jest dostępne!" });
    }

    [HttpGet]
    [ApiKeyAuthorize]
    public IActionResult GetAll()
    {
        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Tekst FROM dane ORDER BY Id";

        var list = new List<DaneDTO>();
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new DaneDTO
            {
                Id = reader.GetInt32(0),
                Tekst = reader.GetString(1)
            });
        }

        return Ok(list);
    }

    [HttpGet("{id}")]
    [ApiKeyAuthorize]
    public IActionResult Get(int id)
    {
        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Tekst FROM dane WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return NotFound();

        return Ok(new DaneDTO
        {
            Id = reader.GetInt32(0),
            Tekst = reader.GetString(1)
        });
    }

    [HttpPost]
    [ApiKeyAuthorize]
    public IActionResult Create([FromBody] DaneDTO dane)
    {
        if (dane is null || string.IsNullOrWhiteSpace(dane.Tekst))
            return BadRequest(new { error = "Pole Tekst jest wymagane." });

        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "INSERT INTO dane (Tekst) VALUES ($tekst)";
        cmd.Parameters.AddWithValue("$tekst", dane.Tekst);
        cmd.ExecuteNonQuery();

        var idCmd = con.CreateCommand();
        idCmd.CommandText = "SELECT last_insert_rowid();";
        var id = Convert.ToInt32(idCmd.ExecuteScalar());

        return CreatedAtAction(nameof(Get), new { id }, new DaneDTO { Id = id, Tekst = dane.Tekst });
    }

    [HttpPut("{id}")]
    [ApiKeyAuthorize]
    public IActionResult Update(int id, [FromBody] DaneDTO dane)
    {
        if (dane is null || string.IsNullOrWhiteSpace(dane.Tekst))
            return BadRequest(new { error = "Pole Tekst jest wymagane." });

        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "UPDATE dane SET Tekst = $tekst WHERE Id = $id";
        cmd.Parameters.AddWithValue("$tekst", dane.Tekst);
        cmd.Parameters.AddWithValue("$id", id);

        var affected = cmd.ExecuteNonQuery();
        if (affected == 0)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ApiKeyAuthorize]
    public IActionResult Delete(int id)
    {
        using var con = Db.GetConnection();
        var cmd = con.CreateCommand();
        cmd.CommandText = "DELETE FROM dane WHERE Id = $id";
        cmd.Parameters.AddWithValue("$id", id);

        var affected = cmd.ExecuteNonQuery();
        if (affected == 0)
            return NotFound();

        return NoContent();
    }
}
