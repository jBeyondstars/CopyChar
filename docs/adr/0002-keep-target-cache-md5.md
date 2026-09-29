# 0002. Do not copy cache.md5, keep the target's own

## Context

Each character folder contains a 240-byte binary `cache.md5`: 10 entries of 24 bytes, each made of the MD5 of a settings file (16 bytes), a Unix timestamp (4 bytes) and 4 zero bytes. On the Classic Anniversary client the entries are: 0 `config-cache.wtf`, 1 `bindings-cache.wtf`, 2 `macros-cache.txt`, 3 `layout-local.txt`, 4 `chat-cache.txt`, 5 `tts-cache-character.txt`, 8 `edit-mode-cache-character.txt`; entries 6, 7 and 9 are all zero. The MD5 values match the content of the files exactly.

The WoW Forever client uses the same layout and also fills entry 6 with `flagged-cache-character.txt` (tutorials already seen, not worth copying) and entry 7 with `click-bindings-cache.txt`. The account folder has its own `cache.md5`: 0 `config-cache.wtf`, 1 `bindings-cache.wtf`, 2 `macros-cache.txt`, 3 `tts-cache-account.txt`, 4 `flagged-cache-account.txt`, 5 `edit-mode-cache-account.txt`.

The client syncs these settings with the server, and the timestamps are the server's sync times. Three copies between two accounts on the WoW Forever beta showed how the client uses this file:

1. Target `cache.md5` kept: the MD5 values no longer match, the client treats the files as local edits and uses them. Settings that changed in game during the session (options, camera) were sent to the server and stayed. Key bindings and macros, untouched in game, were never sent: at a later login the server copy came back.
2. Target `cache.md5` rewritten with the MD5 of the copied files and the current time: the client treats the files as synced, takes the server copy at login and rewrites the timestamps with the server's. The copy is lost straight away.
3. Target `cache.md5` kept, then one key binding and one macro changed and set back in game: everything stayed after several logins.

## Decision

`cache.md5` belongs to no category and is never copied or edited. The target keeps its own. After a copy, the application lists the settings to change and set back in game, one per kind of synced setting copied.

## Consequences

- The copy is used at the next login, and kept for good once each kind of setting has been touched in game.
- A player who skips the step in game gets the server copy of the untouched settings back later, without warning from the game.
- Copying the source's `cache.md5` would tie the copied files to another character's sync timestamps, with the same effect as case 2.
- None of this is documented by Blizzard. It is one of the things to check after a client update.
