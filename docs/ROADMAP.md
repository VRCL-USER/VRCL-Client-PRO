# Roadmap

## Phase 1 — Foundation (current)
- [x] Discord bot slash-command starter: `/status`, `/assets`
- [x] FastAPI health endpoint and token-protected routes
- [x] SQLite metadata tables
- [x] Windows agent outbound heartbeat
- [x] Minimal dashboard health check
- [x] Security and setup documentation

## Phase 2 — Reliable local agent
- [ ] Agent registration and last-seen status in the dashboard
- [ ] Windows-friendly launcher and clear logs
- [ ] Local file picker for user-selected files only
- [ ] File validation and safe storage for explicitly authorized assets

## Phase 3 — Asset catalog
- [ ] Add an explicit authorization record for every imported asset
- [ ] Metadata view: display name, avatar ID when provided, file type, size, import date
- [ ] Search, filters, and audit history
- [ ] Per-user access checks before any file can be downloaded

## Phase 4 — Dashboard and Discord
- [ ] Authenticated dashboard session (no API secrets embedded in browser JavaScript)
- [ ] Player/asset cards using only permitted and authorized metadata
- [ ] Discord admin controls and useful status embeds
- [ ] Optional notifications for agent disconnects

## Phase 5 — Permitted integrations
- [ ] Review current VRChat terms and supported interfaces before integrating
- [ ] Use only documented/approved metadata access
- [ ] Keep private avatar files inaccessible unless the rights holder explicitly authorizes their use

## Release checklist
- [ ] Unit tests and API tests
- [ ] Dependency and secret scanning
- [ ] HTTPS and deployment configuration
- [ ] Backup, retention, and deletion procedures
- [ ] Permission review for the Discord bot
