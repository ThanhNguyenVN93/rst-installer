# rst-installer

Companion data/mirror repo for the Intel RST Auto Installer tool.

- `driver-mapping.json` — remote-updatable CPU generation -> driver download mapping (the app's embedded copy can be overridden by this file without a new app release).
- Release assets — mirrored copies of the Intel RST installer packages, used as a fallback download when Intel's own mirror link is unavailable. Checksums for both primary and fallback URLs are tracked in `driver-mapping.json`.
