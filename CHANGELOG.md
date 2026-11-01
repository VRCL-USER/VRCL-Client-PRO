# Changelog

## v1.1.01 — 2026-11-01

- Added an Instance Journal for saving world names, world IDs, location/instance IDs, supplied instance links, user IDs, avatar IDs, and notes locally.
- Added parsing of world and instance identifiers from a VRChat link supplied by the user.
- Added `instances list` and `instances open` console commands.
- Instance entries are stored separately in `%LOCALAPPDATA%\\VRCL\\ClientPRO\\Instances.json`.
- Clarified that the journal records user-supplied observations and does not scan or monitor live instances.


All notable changes to VRCL PRO are recorded here. Every code change or fix intended for a new build receives a version increment.

## v1.1.00 — 2026-11-01

- Rebranded the desktop window and executable as **VRCL PRO**.
- Added a custom purple wolf-style VRCL PRO Windows application icon.
- Reworked local import so it no longer asks for contributor names or signed documents.
- Added local imports for .vrca, .vrcw, .unitypackage, and .zip files already on the computer.
- Added optional user ID, avatar ID, world name, world ID, and location/instance ID fields.
- Added optional local preview image support.
- Expanded the local library to show asset type, IDs, world metadata, preview images, and import time.
- Added an in-app command console with visible output: help, version, library list/open, ID parsing from user-supplied text, and official VRChat link handoff.
- Kept live platform scraping, automated VRChat bots, avatar unlocking, and protected asset extraction out of scope.

## v1.0.03 — 2026-11-01

- Corrected the desktop window XAML after the version-title change so the root Window attributes are valid.

## v1.0.02 — 2026-11-01

- Fixed the missing `System.IO` namespace in the local asset library service.
- This resolves compiler errors for `Path`, `File`, and `Directory`.

## v1.0.01 — 2026-11-01

- Fixed WPF checkbox text wrapping markup so the desktop project can compile past the XAML markup validation step.

## v1.0.00 — 2026-11-01

- Established the initial standalone WPF desktop application foundation.
- Added the initial dark-themed overview, official VRChat link handoff, local asset import, SHA-256 hashing, and local asset library.
- Added PowerShell development-run and self-contained Windows publish scripts.
