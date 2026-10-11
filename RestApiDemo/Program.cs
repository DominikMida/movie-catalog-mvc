using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

const string baseUrl = "https://localhost:5001/api/Dane";
const string apiUser = "dominik";
const string apiToken = "dominik-token-2026";

var client = new HttpClient();
client.DefaultRequestHeaders.Add("X-Api-User", apiUser);
client.DefaultRequestHeaders.Add("X-Api-Token", apiToken);

Console.WriteLine("REST API demo: GET /api/Dane");
try
{
    await GetAll();
}
catch (Exception ex)
{
    Console.WriteLine($"Błąd: {ex.Message}");
}

Console.WriteLine("\nREST API demo: POST /api/Dane");
int createdId = 0;
try
{
    createdId = await Create("Nowy wpis z demo");
    Console.WriteLine($"Utworzony wpis o ID: {createdId}");
}
catch (Exception ex)
{
    Console.WriteLine($"Błąd: {ex.Message}");
}

if (createdId > 0)
{
    Console.WriteLine($"\nREST API demo: PUT /api/Dane/{createdId}");
    try
    {
        await Update(createdId, "Zmieniony tekst demo");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd: {ex.Message}");
    }

    Console.WriteLine($"\nREST API demo: GET /api/Dane/{createdId} (pobranie zmienionego)");
    try
    {
        await GetById(createdId);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd: {ex.Message}");
    }

    Console.WriteLine($"\nREST API demo: DELETE /api/Dane/{createdId}");
    try
    {
        await Delete(createdId);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Błąd: {ex.Message}");
    }
}

async Task GetAll()
{
    var results = await client.GetFromJsonAsync<List<DaneDto>>(baseUrl);
    if (results is null)
    {
        Console.WriteLine("Brak odpowiedzi");
        return;
    }

    foreach (var item in results)
    {
        Console.WriteLine($"{item.Id}: {item.Tekst}");
    }
}

async Task<int> Create(string text)
{
    var response = await client.PostAsJsonAsync(baseUrl, new { Tekst = text });
    Console.WriteLine($"Status: {response.StatusCode}");
    
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
        return 0;
    }

    try
    {
        var body = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);
        var root = doc.RootElement;
        if (root.TryGetProperty("id", out var idElement) && idElement.TryGetInt32(out var id))
        {
            Console.WriteLine($"Odpowiedź: {body}");
            return id;
        }
    }
    catch { }

    return 0;
}

async Task GetById(int id)
{
    var response = await client.GetAsync($"{baseUrl}/{id}");
    Console.WriteLine($"Status: {response.StatusCode}");
    if (response.IsSuccessStatusCode)
    {
        var item = await response.Content.ReadFromJsonAsync<DaneDto>();
        if (item != null)
            Console.WriteLine($"{item.Id}: {item.Tekst}");
    }
    else
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}

async Task Update(int id, string text)
{
    var response = await client.PutAsJsonAsync($"{baseUrl}/{id}", new { Tekst = text });
    Console.WriteLine($"Status: {response.StatusCode}");
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}

async Task Delete(int id)
{
    var response = await client.DeleteAsync($"{baseUrl}/{id}");
    Console.WriteLine($"Status: {response.StatusCode}");
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine(await response.Content.ReadAsStringAsync());
    }
}

internal class DaneDto
{
    public int Id { get; set; }
    public string? Tekst { get; set; }
}
