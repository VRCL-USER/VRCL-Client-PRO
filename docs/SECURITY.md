# Security and authorized use

This repository is designed for assets the operator owns or has explicit permission to manage.

## Non-negotiable boundaries

- Do not bypass VRChat privacy settings, access controls, or avatar visibility restrictions.
- Do not extract, download, or redistribute another person's private avatar files without explicit authorization from the rights holder.
- Do not collect VRChat passwords, browser cookies, session tokens, or authentication secrets.
- Do not automate account logins or impersonate a user to obtain content.
- Use documented, permitted interfaces for any future VRChat metadata integration.
- Keep authorization evidence with each asset before enabling import, storage, or download features.

## Secrets

- Keep tokens in a local `.env` file. It is ignored by Git.
- Never paste bot tokens or API secrets into Discord messages, issues, or source files.
- Rotate a token immediately if it is exposed.
- Use a dedicated Discord bot account and grant only the permissions required by its commands.
- Use HTTPS and a proper secrets manager before exposing the API to the internet.

## Current implementation

The initial API stores agent heartbeats and an asset metadata table. The asset listing endpoint returns only rows explicitly marked `authorized = 1`. There is not yet an upload, file-serving, avatar-extraction, or VRChat-account integration endpoint.

The Windows agent only sends a heartbeat. It does not crawl folders, inspect running VRChat processes, collect credentials, or transfer files.

## Before enabling file features

1. Require an explicit user action to select a local file.
2. Record who owns the asset and why the operator is authorized to use it.
3. Validate file type, size, and content before processing.
4. Store files outside the web root with non-guessable identifiers.
5. Require authentication and per-user authorization for every download.
6. Log administrative actions without logging secrets.
7. Add retention and deletion controls.
