# VRCL Client PRO

A starter monorepo for a Discord bot, FastAPI service, Windows companion agent, and lightweight dashboard for managing **user-owned or explicitly authorized VRChat assets**.

> **Important:** This project does not extract private avatars, bypass VRChat access controls, harvest session tokens, or download other users' assets without permission. The initial version provides service status and a safe foundation for authorized workflows. VRChat integration and asset importing are intentionally not implemented yet.

## Project layout

```text
VRCL-Client-PRO/
├── agent/
│   └── main.py            # Windows companion; sends outbound heartbeats
├── backend/
│   └── main.py            # FastAPI service and SQLite metadata catalog
├── bot/
│   └── main.py            # Discord slash commands
├── dashboard/
│   └── index.html         # Lightweight API status page
├── docs/
│   ├── ROADMAP.md
│   └── SECURITY.md
├── .env.example
├── .gitignore
└── requirements.txt
```

## Requirements

- Windows 10/11 or another OS for development
- Python 3.11 or newer
- A Discord application and bot token for the Discord bot
- A Discord server where you have permission to add the bot

The API and agent can be tested without a Discord bot token.

## Setup (Windows)

1. Clone this repository and open PowerShell in its folder.
2. Create and activate a virtual environment:

   ```powershell
   py -3.11 -m venv .venv
   .\.venv\Scripts\Activate.ps1
   ```

3. Install dependencies:

   ```powershell
   python -m pip install --upgrade pip
   pip install -r requirements.txt
   ```

4. Copy the example environment file:

   ```powershell
   Copy-Item .env.example .env
   ```

5. Edit `.env`. Set a long random `API_TOKEN`. Add `DISCORD_TOKEN` only when you are ready to run the bot. Never commit `.env` or share its values in chat.

## Run the API

From the repository root:

```powershell
python -m uvicorn backend.main:app --host 127.0.0.1 --port 8000 --reload
```

Open [http://127.0.0.1:8000/health](http://127.0.0.1:8000/health) to check the service.

## Run the Windows agent

In a second terminal, from the repository root:

```powershell
python agent/main.py
```

The agent makes outbound heartbeat requests to the API. It does not scan folders, collect credentials, or upload avatar files.

## Run the Discord bot

Set `DISCORD_TOKEN` in `.env`, invite the bot using only the permissions it needs (View Channels, Send Messages, Use Application Commands, Embed Links, and Read Message History), then run:

```powershell
python bot/main.py
```

Available starter slash commands:

- `/status` — checks whether the API is reachable.
- `/assets` — lists authorized assets in the metadata catalog.

Set `DISCORD_GUILD_ID` in `.env` to your test server ID for faster command registration during development. Leave it blank to sync globally.

## Dashboard

Run a local static server from the repository root:

```powershell
python -m http.server 5500 --directory dashboard
```

Then open [http://127.0.0.1:5500](http://127.0.0.1:5500). The starter dashboard displays API health only; it deliberately does not expose API secrets in browser code.

## Current limitations

- Asset catalog starts empty. There is no asset upload/download endpoint yet.
- The agent reports that it is online; it does not manipulate VRChat or Unity files.
- VRChat account integration is not configured. Do not add passwords, cookies, or session tokens.
- The API is intended for local development until authentication, HTTPS, deployment, and operational safeguards are reviewed.

See [docs/ROADMAP.md](docs/ROADMAP.md) and [docs/SECURITY.md](docs/SECURITY.md) before expanding the project.
