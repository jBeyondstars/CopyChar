# 0002. Do not copy cache.md5, keep the target's own

## Context

Each character folder contains a 240-byte binary `cache.md5`: 10 entries of 24 bytes, each made of the MD5 of a settings file (16 bytes), a Unix timestamp (4 bytes) and 4 zero bytes. On the Classic Anniversary client the entries are: 0 `config-cache.wtf`, 1 `bindings-cache.wtf`, 2 `macros-cache.txt`, 3 `layout-local.txt`, 4 `chat-cache.txt`, 5 `tts-cache-character.txt`, 8 `edit-mode-cache-character.txt`; entries 6, 7 and 9 are all zero. The MD5 values match the content of the files exactly.

The client syncs these settings with the server. This file tells it whether a file changed locally since the last sync.

## Decision

`cache.md5` belongs to no category and is never copied. The target keeps its own.

## Consequences

- After a copy, the target's MD5 values no longer match its files: the client should see them as locally modified and keep them, instead of taking the server version.
- Copying the source's `cache.md5` would tie the copied files to another character's sync timestamps, with unpredictable results.
- This behavior is inferred from the format, not documented by Blizzard. It is one of the things to check after a client update.
