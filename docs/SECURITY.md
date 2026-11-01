# Security and authorized use

VRCL Client PRO is a local Windows desktop application. It is designed to manage files the operator owns or has explicit permission to use.

## Contributor authorization

- Record the contributor's name and relevant avatar ID where supplied.
- Keep the permission document beside the imported file.
- Describe the scope of permission in the import record.
- Preserve a SHA-256 hash to identify the exact imported file.
- Add revocation and deletion controls before distributing the app broadly.
- A consent form records permission; it does not itself provide technical access to VRChat-hosted files.

## Acquisition

The current import flow requires the user to select a local .vrca file and its accompanying permission document. The contributor must supply the file or provide it through a supported transfer mechanism.

Do not add functionality that bypasses VRChat privacy settings, scrapes live instances, circumvents platform access controls, or extracts protected files. Use documented and permitted interfaces for any future VRChat integration.

## Credentials

- Do not collect or store VRChat passwords.
- Do not collect browser cookies, session tokens, or authentication secrets.
- Use the user's existing official VRChat client for account sign-in and instance handoff.

## Local storage

The application stores its catalog and imported files under:

`%LOCALAPPDATA%\VRCL\ClientPRO`

Do not commit creator files, permission documents, personal data, or local library metadata to GitHub.
