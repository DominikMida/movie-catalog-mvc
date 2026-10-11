# Movie Catalog – ASP.NET Core MVC + REST API

A web application for managing movies, genres, studios and user reviews, built with **ASP.NET Core 8 MVC**, **Entity Framework Core** and **SQLite**. Besides the server-rendered web UI, the project exposes a **token-authenticated REST API** and includes a small console clien t for testing it.

Built as a university lab project (*Advanced Programming 2*, AGH University of Science and Technology).

<!-- Add 1–2 screenshots here, e.g. the movie list and the ranking page:
![Movie list](docs/movies.png)
-->

## Features

- **CRUD for movies, genres, studios and reviews** through a Razor-based web interface
- **Movie ranking** – movies sorted by average review score, with the number of reviews
- **Session-based login** with two roles: regular user and administrator
- **Admin panel** – administrators can view and add users
- **REST API** with CRUD endpoints, protected by a custom authorization filter (`X-Api-User` / `X-Api-Token` headers)
- **Automatic first-run setup** – database schema, sample data and initial accounts are created on startup
- **Console client** (`RestApiDemo`) that exercises the API end to end

## Tech stack

| Area | Technology |
|---|---|
| Language / framework | C#, ASP.NET Core 8 (MVC) |
| Data access | Entity Framework Core (code-first migrations), ADO.NET (`Microsoft.Data.Sqlite`) |
| Database | SQLite |
| Views | Razor, Bootstrap, HTML/CSS |
| Authentication | Server-side sessions (web UI), API token (REST API) |

## Architecture

The application follows the **Model–View–Controller** pattern:

- **Models** – entity classes and view models (`Film`, `Gatunek`, `Studio`, `Recenzja`, `FilmRankingViewModel`, …)
- **Views** – Razor views, one folder per controller
- **Controllers** – handle HTTP requests and application logic; web controllers inherit from `BaseController`, which restricts access to logged-in users
- **Helpers** – `ApiKeyAuthorizeAttribute` (action filter validating API credentials), `HashHelper`
- **Data** – `MovieContext` (EF Core `DbContext`) and `Db` (database initialization, seeding and credential lookups)

### Data model

```
Gatunek (Genre) 1 ──< Film >── 1 Studio
                       │
                       1
                       │
                       ^
                   Recenzja (Review)
```

| Table | Columns |
|---|---|
| `Film` | Id, Tytul (title), Rok (year), StudioId, GatunekId |
| `Gatunek` | Id, Nazwa (name) |
| `Studio` | Id, Nazwa (name) |
| `Recenzja` | Id, Autor (author), Ocena (rating), Komentarz (comment), FilmId |
| `loginy` | Id, Login, Haslo (password hash), IsAdmin, Token |
| `dane` | Id, Tekst – generic table exposed by the REST API |

Relationships: one genre has many movies, one studio has many movies, one movie has many reviews.

## Authentication and authorization

**Web UI.** After a successful login, the user's name and role are stored in the server-side session (`user`, `isAdmin`). Controllers verify the session before serving any page. Administrators can additionally view users and create new ones.

**REST API.** Each request to a protected endpoint must carry two headers:

```
X-Api-User:  <login>
X-Api-Token: <token>
```

The `[ApiKeyAuthorize]` filter validates the pair against the `loginy` table and returns `401 Unauthorized` otherwise.

## REST API

Base path: `/api/Dane`

| Method | Endpoint | Description | Auth |
|---|---|---|---|
| GET | `/api/Dane/test` | Health check | No |
| GET | `/api/Dane` | List all records | Yes |
| GET | `/api/Dane/{id}` | Get a record by ID | Yes |
| POST | `/api/Dane` | Create a record | Yes |
| PUT | `/api/Dane/{id}` | Update a record | Yes |
| DELETE | `/api/Dane/{id}` | Delete a record | Yes |

Example:

```bash
curl -k https://localhost:5001/api/Dane \
  -H "X-Api-User: <login>" \
  -H "X-Api-Token: <token>"

curl -k -X POST https://localhost:5001/api/Dane \
  -H "X-Api-User: <login>" \
  -H "X-Api-Token: <token>" \
  -H "Content-Type: application/json" \
  -d '{"tekst": "My first record"}'
```

The `RestApiDemo` project runs through the full cycle (GET → POST → PUT → GET → DELETE) and prints the results.

## Getting started

**Requirements:** [.NET 8 SDK](https://dotnet.microsoft.com/download)

```bash
git clone https://github.com/DominikMida/<repository-name>.git
cd <repository-name>/MvcPracownicy
dotnet run
```

Open the HTTPS address printed in the console (e.g. `https://localhost:5001`).

On first launch the application automatically:

1. creates the SQLite database (`app.db`) and applies EF Core migrations,
2. seeds sample studios, genres and movies,
3. creates two demo accounts (see below).

### Demo accounts

| Role | Login | Password | API token |
|---|---|---|---|
| Administrator | `dominik` | `mida` | `dominik-token-2026` |
| Regular user | `user` | `user` | `user-token-2026` |

These are sample credentials for local testing only (defined in `Data/Db.cs`). Do not reuse them in any real deployment.

To try the API client, start the web app first, then in another terminal:

```bash
cd RestApiDemo
dotnet run
```

## Project structure

```
MvcPracownicy/          ASP.NET Core MVC application
├── Controllers/        Film, Gatunek, Studio, Recenzja, Account, DaneApi
├── Data/               MovieContext (EF Core), Db (init + seeding)
├── Helpers/            ApiKeyAuthorizeAttribute, HashHelper
├── Migrations/         EF Core migrations
├── Models/             Entities and view models
├── Views/              Razor views
└── Program.cs          Application configuration and middleware
RestApiDemo/            Console client for the REST API
```

## Known limitations and possible improvements

This is a coursework project, so some decisions favour simplicity over production readiness:

- Passwords are hashed with **MD5**, which is not suitable for real systems. A proper solution would use PBKDF2/bcrypt or ASP.NET Core Identity.
- API tokens are stored in plain text and the initial ones are fixed. They should be generated randomly and stored hashed.
- The REST API currently exposes the generic `dane` table rather than the movie entities; extending it to films, genres, studios and reviews would be a natural next step.
- Data access mixes EF Core (movies) with raw ADO.NET (users and API data); unifying it would simplify the code.
- No automated tests yet.

## Author

**Dominik Mida** – Computer Science student (Intelligent Systems), AGH University of Science and Technology
[LinkedIn](https://linkedin.com/in/dominikmida) · [GitHub](https://github.com/DominikMida)
