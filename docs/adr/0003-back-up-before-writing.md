# 0003. Back up each target before writing

## Context

A copy overwrites files of the target with no way back in game. A selection mistake (wrong source, wrong target) would lose a character's configuration. Character folders are small, usually a few dozen KB.

## Decision

- Before writing to a target, the whole character folder is zipped into `%LOCALAPPDATA%\CopyChar\backups`.
- The zip contains a `.copychar-origin` entry with the original path, so a backup can be restored without asking for the folder again.
- A restore first backs up the current state, deletes the folder and extracts the zip. The folder goes back exactly to its backed up state, including the absence of files added since.

## Consequences

- Every copy and every restore can be undone.
- Backups pile up; there is no automatic purge.
- Backing up the whole folder rather than only the overwritten files takes a bit more space, but keeps the restore simple and exact.
