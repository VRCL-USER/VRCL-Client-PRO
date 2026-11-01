# Changelog

All notable changes to VRCL Client PRO are recorded here. New application changes and fixes receive a version increment.

## v1.0.02 — 2026-11-01

- Fixed the missing `System.IO` namespace in the local asset library service.
- This resolves compiler errors for `Path`, `File`, and `Directory`.

## v1.0.01 — 2026-11-01

- Fixed WPF checkbox text wrapping markup so the desktop project can compile past the XAML markup validation step.

## v1.0.00 — 2026-11-01

- Established the standalone WPF desktop application foundation.
- Added the initial dark-themed overview, official VRChat link handoff, contributor-authorized asset import, consent-document storage, SHA-256 hashing, and local asset library.
- Added PowerShell development-run and self-contained Windows publish scripts.
