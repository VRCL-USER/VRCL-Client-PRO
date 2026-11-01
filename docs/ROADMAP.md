# VRCL PRO roadmap

## Phase 1 — Desktop foundation
- [x] Standalone WPF Windows application
- [x] Purple VRCL PRO branding and app icon
- [x] Official VRChat link handoff
- [x] Local asset import for files already on the PC
- [x] Optional avatar, user, world, and location ID metadata
- [x] Optional local preview images
- [x] SHA-256 hashing and local JSON catalog
- [x] In-app console and visible command output
- [x] PowerShell run and self-contained publish scripts
- [ ] Build and test v1.1.00 on the target Windows PC

## Phase 2 — Local asset workflows
- [ ] SARs integration for locally supplied files the user is authorized to process
- [ ] Asset details pane with hash, source file, metadata, and preview
- [ ] Search, sort, and filters
- [ ] Open/export selected files
- [ ] Delete records and associated local files
- [ ] Backup and restore library

## Phase 3 — Metadata and previews
- [ ] Read metadata from local asset files where supported by a documented file format/tool
- [ ] Preview extraction from local files where supported
- [ ] Import/export metadata manifests
- [ ] Add lookup through documented and permitted VRChat interfaces only, if available

## Phase 4 — VRChat companion
- [ ] Improve official instance/invite link handling
- [ ] Local launch history and user-selected favorites
- [ ] Display permitted metadata returned by supported interfaces
- [ ] Keep official VRChat client sign-in and privacy controls in charge

## Phase 5 — Release quality
- [ ] Unit tests for import, hashing, catalog, and deletion
- [ ] Installer and update workflow
- [ ] Error reporting and rotating logs
- [ ] Code signing and release packaging
- [ ] UI polish and accessibility

## Scope note

This project does not include an automated VRChat bot, live-instance scraping, avatar unlocking, access-control bypass, or extraction of protected platform assets. Contributor permission can be recorded as provenance, but local import requires a file already made available to the user through a supported transfer method.
