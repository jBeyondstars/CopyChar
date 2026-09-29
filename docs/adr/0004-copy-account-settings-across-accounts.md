# 0004. Copy account settings only to other accounts, on request

## Context

The client keeps part of the settings at the root of the account folder, `WTF\Account\<account>\`, shared by every character of that account: `bindings-cache.wtf` (key bindings, unless a character uses its own), `macros-cache.txt` (general macros), `config-cache.wtf` (account options), `edit-mode-cache-account.txt` (Edit Mode layouts) and `tts-cache-account.txt`.

Copying a character folder to a character of another account therefore leaves most of the setup behind. A manual test on WoW Forever confirmed it: the key bindings did not follow, and the character Edit Mode file, which only stores the index of the active layout, pointed to a layout of the target account. Copying these five files as well gave the expected result in game.

## Decision

- The five files are separate categories, grouped under "Account settings" and unchecked by default.
- They are only copied to the accounts of the targets that differ from the source account; characters of the source account already share them.
- Each target account gets its own backup, limited to the files at the root of the account folder.
- `cache.md5` and the account-wide `SavedVariables` stay out, for the reasons of ADR 0002 and because addon data there is keyed by character and shared with the other characters of the account.

## Consequences

- A copy to another account can reproduce the full setup of the source character, addon profiles aside.
- The copy changes every character of the target account, which the confirmation dialog states.
- Copying to several accounts at once sends the same account files to each of them.
