# 0001. WPF on .NET 10, with a separate core library

## Context

The tool fits in one window: pick a folder, a source character, categories and targets, then read a log. It only targets Windows, like the game client. The risky part is not the interface but the handling of the game files, which must be testable without starting the application.

## Decision

- WPF interface on .NET 10, with a single view model (`MainViewModel`) and a minimal implementation of `INotifyPropertyChanged` and `ICommand`, without an MVVM framework.
- All the logic (WTF folder scan, copy plan, copy, backups, game detection) lives in `CopyChar.Core`, which targets `net10.0` and does not know about WPF.
- xUnit tests on `CopyChar.Core` only, using real temporary folders rather than file system abstractions.

## Consequences

- Tests cover the real on-disk behavior (zip, delete, copy) at the cost of some I/O.
- The view model has no automated tests; it stays thin and delegates to the core.
- No NuGet dependency in the application. An MVVM framework would become useful if more windows are added.
