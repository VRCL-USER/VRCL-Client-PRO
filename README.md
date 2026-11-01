# VRCL Client PRO

A standalone, local-first Windows desktop companion for VRChat workflows. The application is built with WPF/C#, stores its library locally, and uses PowerShell scripts for development and publishing.

**This project is not a Discord bot.** The desktop application does not require a Discord application, bot token, cloud API, or remote service.

## Current desktop foundation

- Dark purple WPF desktop interface.
- Official VRChat instance/invite link handoff to the user's browser and installed client.
- Local import of creator-supplied `.vrca` files.
- Required signed-permission document attached to each imported asset.
- SHA-256 file hash and local audit metadata.
- Searchable/displayable local asset catalog (initial version).
- No VRChat passwords, cookies, or session tokens are collected or stored.

The import workflow expects the contributor to provide the actual file, or to make it available through a supported transfer method. A permission document records consent; it does not create a technical API or override platform access controls. The app does not scrape live instances or bypass VRChat privacy restrictions.

## Project layout

```text
VRCL-Client-PRO/
├── src/
│   └── VRCL.ClientPRO/
│       ├── Models/
│       ├── Services/
│       ├── App.xaml
│       ├── MainWindow.xaml
│       └── VRCL.ClientPRO.csproj
├── scripts/
│   ├── Build-VRCLClientPRO.ps1
│   └── Run-VRCLClientPRO.ps1
└── docs/
    ├── ROADMAP.md
    └── SECURITY.md
```

## Requirements

- Windows 10/11 x64
- .NET 10 SDK (Windows desktop workload/components)
- PowerShell 5.1 or newer

## Run from source

1. Download or clone this repository.
2. Install the .NET 10 SDK from the official Microsoft .NET download page.
3. Open PowerShell in the repository root.
4. Run:

   ```powershell
   .\scripts\Run-VRCLClientPRO.ps1
   ```

## Build a standalone Windows executable

From the repository root, run:

```powershell
.\scripts\Build-VRCLClientPRO.ps1
```

The self-contained x64 publish output will be written to:

```text
dist\win-x64\VRCL Client PRO.exe
```

The self-contained build includes the .NET runtime, so the target PC does not need to install .NET separately.

## Local data

User data is stored under:

```text
%LOCALAPPDATA%\VRCL\ClientPRO
```

- `Assets\` — creator-supplied avatar files.
- `Consents\` — accompanying permission documents.
- `Library.json` — local asset metadata and SHA-256 hashes.

## Planned work

See [docs/ROADMAP.md](docs/ROADMAP.md). Planned areas include improved asset metadata inspection, revocation/deletion workflows, SARs integration for files the user is authorized to process, permitted VRChat metadata, and instance-management improvements.

## Security and permitted use

See [docs/SECURITY.md](docs/SECURITY.md). Do not commit private creator files, signed permission documents, credentials, or personal data to this repository.
