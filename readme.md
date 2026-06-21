# M23 — Sistem de încărcare cu benzi

Simulator și sistem de monitorizare/control pentru procesul cu evenimente discrete M23 (sistem de încărcare cu benzi transportoare).

## Arhitectură

```
┌─────────────────────────┐
│      M23.Simulator      │  console app — rulează FSM-urile, server TCP, perturbații
└────────────┬────────────┘
             │ TCP (JSON)
┌────────────▼─────────────┐
│      M23.Controller      │  ASP.NET Core — client TCP, WebSocket hub, logger, rapoarte
└────────────┬─────────────┘
             │ WebSocket + HTTP
┌────────────▼─────────────┐
│       M23.Monitor        │  SvelteKit — aplicație web de monitorizare și control
└──────────────────────────┘
             │
┌────────────▼─────────────┐
│          MSSQL           │  evenimente, perioade de funcționare/oprire
└──────────────────────────┘
```

- **M23.Shared** — bibliotecă .NET cu tipurile mesajelor de protocol, partajată între Simulator și Controller.
- **M23.Tests** — teste unitare (xUnit) pentru FSM-uri, orchestrator, server TCP și servicii din Controller.

## Cerințe de sistem

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) și npm
- Docker (recomandat pentru MSSQL pe Linux/macOS) sau o instanță MSSQL Server accesibilă
- `dotnet-ef` tool: `dotnet tool install --global dotnet-ef`

## 1. Configurare bază de date (MSSQL)

### Pe Linux/macOS (Docker)

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStr0ng!Passw0rd" \
  -p 1433:1433 --name m23-mssql \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

Pentru a porni/opri containerul ulterior:

```bash
docker start m23-mssql
docker stop m23-mssql
```

### Pe Windows

Se poate folosi o instanță locală SQL Server (Express/Developer) sau aceeași abordare cu Docker Desktop.

### Connection string

Actualizați `M23.Controller/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost,1433;Database=M23;User Id=sa;Password=YourStr0ng!Passw0rd;TrustServerCertificate=True"
  }
}
```

> Pe Windows cu autentificare Windows, connection string-ul poate folosi `Trusted_Connection=True` în loc de `User Id`/`Password`.

### Aplicare migrații

```bash
cd M23.Controller
dotnet ef database update
```

> Migrațiile se aplică automat și la pornirea aplicației (`db.Database.MigrateAsync()` în `Program.cs`), dar este utilă rularea manuală pentru verificare.

## 2. Pornire Simulator

```bash
cd M23.Simulator
dotnet run
```

Simulatorul pornește un server TCP pe portul **5000** și un task de perturbații care introduce aleatoriu erori de senzori.

## 3. Pornire Controller

```bash
cd M23.Controller
dotnet run
```

Controller-ul se conectează la Simulator prin TCP, expune un WebSocket pe `/ws` și endpoint-uri HTTP pentru rapoarte. Verificați în consolă portul pe care ascultă (implicit configurat în `Properties/launchSettings.json`, de exemplu `http://localhost:5265`).

### Configurare adresă Simulator

Dacă Simulatorul rulează pe altă mașină/port, actualizați `M23.Controller/appsettings.json`:

```json
{
  "Simulator": {
    "Host": "127.0.0.1",
    "Port": "5000"
  }
}
```

## 4. Pornire aplicație de monitorizare (M23.Monitor)

```bash
cd M23.Monitor
npm install
npm run dev
```

Deschideți adresa afișată (implicit `http://localhost:5173`).

În interfață, în bara de conectare, introduceți adresa WebSocket a Controller-ului (de exemplu `ws://localhost:5265/ws`) și apăsați **Connect**.

> Dacă Controller-ul rulează pe HTTPS, folosiți `wss://` în loc de `ws://`.

## Ordinea de pornire recomandată

1. MSSQL (Docker container)
2. M23.Simulator
3. M23.Controller
4. M23.Monitor (`npm run dev`)
5. Conectare din interfața web la Controller

## Rulare teste

```bash
cd M23.Tests
dotnet test
```

Acoperă: FSM-urile (Flap, InputBelt, OutputBelt, System), evaluatorul de condiții, orchestratorul procesului, serverul TCP și serviciile Controller-ului (stare proces, rapoarte).

## Structura soluției

```
M23/
├── M23.sln
├── M23.Simulator/        # simulator proces, FSM-uri, server TCP, perturbații
├── M23.Controller/       # ASP.NET Core — WebSocket, logger, rapoarte, EF Core
├── M23.Shared/           # protocol de comunicare partajat
├── M23.Tests/            # teste unitare xUnit
└── M23.Monitor/          # aplicație web SvelteKit (separată de soluția .NET)
```

## Funcționalități principale

- Simulare proces cu evenimente discrete (4 benzi, clapetă cu 3 poziții, detectare defecte senzori)
- Comunicare în timp real proces ↔ aplicație de monitorizare prin WebSocket
- Pornire/oprire individuală a benzilor, oprire de urgență (S0), oprire benzi de intrare (S5)
- Întrerupere și reluare proces (stare FAULT cu alarmă și restart manual)
- Jurnalizare evenimente în bază de date MSSQL
- Vizualizare istoric evenimente și rapoarte privind perioadele de întrerupere a procesului
- Interfață responsivă (desktop, tabletă, mobil)