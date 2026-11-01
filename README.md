# VRCL PRO

A standalone, local-first Windows desktop companion for VRChat-related workflows. It is a WPF/C# application with local storage and PowerShell development/build scripts.

**There is no Discord bot or cloud API.** The application does not ask for a VRChat password, browser cookie, or session token.

## v1.1.00 features

- Purple wolf-style VRCL PRO application icon and rebranded desktop window.
- Official VRChat instance/invite link handoff to the user's normal browser and installed client.
- Local import of files already on the user's PC: `.vrca`, `.vrcw`, `.unitypackage`, and `.zip`.
- Optional user ID, avatar ID, world name, world ID, location/instance ID, and notes.
- Optional local preview image for an asset.
- Local library with metadata, file paths, import dates, and SHA-256 file hashes.
- In-app console with visible command output.

## Console commands

- `help` — list available local commands.
- `version` — display the application version.
- `library list` — list assets and the metadata saved in the local library.
- `library open` — open the local VRCL storage directory.
- `ids <text or link>` — find `avtr_`, `usr_`, and `wrld_` identifiers in text that you provide.
- `join <official VRChat link>` — open an HTTPS link on `vrchat.com` through the standard browser/client handoff.
- `clear` — clear console output.

These are local application commands. They do not query or scrape live VRChat instances.

## Important VRChat integration boundary

The current VRChat Terms of Service prohibit automated bots, crawlers, scrapers, or other automated means from accessing/searching the Platform or extracting data, and prohibit bypassing platform security or access-control features. Therefore, VRCL PRO does not create a bot that joins arbitrary instances, unlocks all avatars, harvests live avatar/world data, or extracts protected `.vrca`/`.vrcw` files from the Platform.

Creator permission is important, but it does not by itself provide an official technical API or override VRChat platform controls. The local library imports files already supplied to or present on the user's computer. Future integrations will use only documented and permitted interfaces.

See [VRChat Terms of Service](https://hello.vrchat.com/legal), [VRChat Public SDK API](https://creators.vrchat.com/sdk/public-sdk-api/), and [docs/SECURITY.md](docs/SECURITY.md).

## Requirements

- Windows 10/11 x64
- .NET 10 SDK
- PowerShell 5.1 or newer

## Run from source

1. Download or clone this repository.
2. Install the .NET 10 SDK.
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

The self-contained x64 executable is written to:

```text
dist\win-x64\VRCL PRO.exe
```

## Local data

User data is stored under:

```text
%LOCALAPPDATA%\VRCL\ClientPRO
```

- `Assets\` — local asset files imported into the library.
- `Thumbnails\` — optional local preview images.
- `Library.json` — asset metadata and SHA-256 hashes.

Do not commit private assets, personal metadata, or local library files to GitHub.

## Project layout

```text
VRCL-Client-PRO/
├── src/VRCL.ClientPRO/
│   ├── Assets/vrcl-pro.ico
│   ├── Models/
│   ├── Services/
│   ├── App.xaml
│   ├── MainWindow.xaml
│   └── VRCL.ClientPRO.csproj
├── scripts/
│   ├── Build-VRCLClientPRO.ps1
│   └── Run-VRCLClientPRO.ps1
├── docs/
└── VERSION, CHANGELOG.md, VERSIONING.md
```
