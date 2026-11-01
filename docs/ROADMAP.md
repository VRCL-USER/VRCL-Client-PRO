# VRCL Client PRO roadmap

## Phase 1 — Standalone desktop foundation
- [x] WPF desktop application project
- [x] Dark purple desktop interface
- [x] Official VRChat link handoff
- [x] Local creator-supplied .vrca import
- [x] Attach signed contributor permission document
- [x] SHA-256 hash and local JSON catalog
- [x] PowerShell run and self-contained publish scripts
- [ ] Build and test on the user's Windows PC

## Phase 2 — Asset library
- [ ] Search, sort, and filter imported assets
- [ ] Detail view for contributor, avatar ID, permission scope, hash, and file paths
- [ ] Open/export a selected authorized file
- [ ] Remove asset and associated consent record
- [ ] Permission expiry and revocation status
- [ ] Backup and restore local library

## Phase 3 — Authorized asset processing
- [ ] Integrate SARs as an optional local tool for files the user is authorized to process
- [ ] Validate file type, size, and integrity before processing
- [ ] Show conversion progress and detailed logs
- [ ] Preserve original file and provenance record
- [ ] Never acquire protected files from live instances or bypass platform access controls

## Phase 4 — VRChat companion tools
- [ ] Launch VRChat through the installed official client
- [ ] Improve official instance/invite link handling
- [ ] Add permitted instance information and local launch history
- [ ] Add documented/approved avatar metadata lookup where available
- [ ] Do not collect passwords, cookies, or session tokens

## Phase 5 — Quality and release
- [ ] Unit tests for import, hashing, catalog, and deletion
- [ ] App icon, installer, and update workflow
- [ ] Error reporting and rotating local logs
- [ ] Code signing and release packaging
- [ ] Accessibility and UI polish

## Scope note

Creator consent is important and is recorded with every imported asset. Consent alone does not provide a platform API or technical access to a private avatar file. The app's acquisition workflow uses files supplied by the contributor or another supported transfer method. No live-instance scraping, private-avatar extraction, or access-control bypass is included.
