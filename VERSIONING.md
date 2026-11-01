# Application versioning

VRCL Client PRO uses the display format:

`vMAJOR.MINOR.PATCH`

The minor and patch fields use two digits in the displayed version.

Examples:

- `v1.0.00` — initial 1.0 release.
- `v1.0.01` — first patch.
- `v1.0.02` — second patch.
- `v1.0.10` — tenth patch.
- `v1.1.00` — next minor release after the 1.0 patch series.

## Rules

1. Every code change, bug fix, or feature change intended for the next app build must increment the version and add a changelog entry.
2. For a fix or small improvement, increment PATCH by one.
3. Keep PATCH zero-padded to two digits in the display version.
4. When the 1.0 patch series is complete, move to the next minor version and reset PATCH to `00` (for example, `v1.1.00`).
5. For a major incompatible redesign, increment MAJOR and reset MINOR and PATCH (for example, `v2.0.00`).
6. Update all version surfaces together:
   - Repository root `VERSION`
   - `CHANGELOG.md`
   - WPF project version metadata in `VRCL.ClientPRO.csproj`
   - The desktop window title when the user-visible release version changes

The root `VERSION` file is the human-readable source of truth. The .NET numeric package version uses the equivalent numeric value (for example, `1.0.2`) while `InformationalVersion` preserves the display label (`v1.0.02`).
