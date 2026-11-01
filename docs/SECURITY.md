# Security and permitted use

VRCL PRO is a local Windows desktop application. Its asset library is intended for files already present on the user's computer and for assets the user owns or has permission to inspect and use.

## VRChat integration

- Use the installed official VRChat client for account sign-in and instance-link handoff.
- Do not collect or store VRChat passwords, browser cookies, session tokens, or authentication secrets.
- Do not create an automated bot/crawler/scraper to access or search live VRChat instances or extract platform data.
- Do not bypass avatar/world privacy settings, security mechanisms, or access controls.
- Use only documented and permitted interfaces for future metadata integrations.

The current VRChat Terms of Service expressly restrict bypassing platform security/access controls and using automated bots, crawlers, scrapers, or other automated means to access/search the Platform or extract data. A private or educational purpose does not itself grant an exception.

## Local asset imports

The import screen lets the user select a local file and optional preview image. It supports `.vrca`, `.vrcw`, `.unitypackage`, and `.zip` files. The app calculates SHA-256 and stores metadata entered by the user, including optional user/avatar/world/location IDs.

The import checkbox is an operator attestation, not independent proof of ownership or permission. It does not provide access to platform-hosted files or override VRChat rules. Contributors must supply files or provide them through a supported transfer method.

## Local storage

The application stores local data under:

`%LOCALAPPDATA%\VRCL\ClientPRO`

- `Assets\` — imported local files.
- `Thumbnails\` — optional preview images.
- `Library.json` — metadata and SHA-256 hashes.

Do not commit local assets, private avatar files, preview images, personal metadata, or library contents to GitHub.
