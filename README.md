# CopyChar

Windows tool that copies the settings of a World of Warcraft Classic character (Anniversary, Era, or the new WoW Forever) to other characters: game options, interface layout, chat windows, macros, key bindings, enabled addons and addon settings, including between two accounts.

![CopyChar main window](docs/images/main-window.png)

## Problem

Every new character starts with a blank interface. The game keeps its settings in files under `WTF\Account\`, some per character and some per account. Copying them by hand works, but you need to know which files to copy, the game must be closed, the server can bring the old settings back, and there is no way back if you pick the wrong folder.

CopyChar lists your characters, copies the settings you pick to one or more characters, and backs up every character or account it changes.

## Installation

Download `CopyChar.exe` from the [latest release](https://github.com/jBeyondstars/CopyChar/releases/latest) and run it. Nothing to install, .NET included.

The executable is not signed: on first launch, Windows SmartScreen may show "Windows protected your PC". Click **More info**, then **Run anyway**.

### Build from source

Requirements: .NET 10 SDK, Windows.

```
dotnet publish src/CopyChar.App -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -o publish
```

This produces `publish\CopyChar.exe` (about 60 MB), which runs without installing .NET.

To run the tests:

```
dotnet test
```

## Usage

### Copy settings

1. Quit the game completely.
2. Start `CopyChar.exe`. The `World of Warcraft` folder is found in `Program Files`; otherwise pick it with **Browse**. The version you played last (anniversary, classic_era, classic_beta for WoW Forever...) is selected; change it with **Version** if needed.
3. Pick the **source character**. Characters are grouped by account.
4. Check the **settings to copy**:
   - **Character settings**: game options, key bindings and macros specific to the character, interface layout and Edit Mode, chat windows, click bindings (WoW Forever), enabled addons.
   - **Account settings**: key bindings, general macros, account options, Edit Mode layouts and text-to-speech. They are shared by every character of an account, so they are only copied to target characters on another account than the source, and they change every character of that account. Unchecked by default.
   - **Addon settings**: the per-character settings of each addon. Unchecked by default, because some addons (guides, quest trackers) also keep the character's progress there.
5. Check the **target characters** and click **Copy**. The confirmation lists what will be changed.

When copying to another account, check the account settings as well: most key bindings and all Edit Mode layouts are stored per account, and the character settings alone would leave them behind.

### After the copy: one step in game

The game saves your settings on the Blizzard servers, but only the ones you change in game. A copied setting that you never touch in game is replaced by the server copy at a later login.

After a copy, CopyChar lists what to do on each target character, for example:

- Key bindings: change a binding, set it back and click **Okay**.
- Macros: in `/macro`, add and remove a space in a macro, then close the window.
- Options: change any option and set it back.

Log in with each target character, do these steps, then log out normally. The copied settings then stay.

### Restore a backup

Before changing anything, CopyChar backs up each target character and each target account in `%LOCALAPPDATA%\CopyChar\backups`. The **Backups** tab lists them: pick one and click **Restore** to put the character or account back as it was. The current state is backed up first, so a restore can be undone too.

## Design choices

- **The game must be closed.** It rewrites the settings files when you log out or quit, which would undo the copy. CopyChar refuses to copy or restore while the game runs from the selected folder.
- **`cache.md5` is never copied.** The game uses it to know which settings were changed on this computer; keeping the target's own file is what makes the game load the copied settings. Details in [docs/adr/0002](docs/adr/0002-keep-target-cache-md5.md).
- **Everything changed is backed up first.** Details in [docs/adr/0003](docs/adr/0003-back-up-before-writing.md).
- **Account settings are opt-in and only go to other accounts.** Details in [docs/adr/0004](docs/adr/0004-copy-account-settings-across-accounts.md).
- **Logic separated from the interface.** `CopyChar.Core` holds the file handling and the tests; `CopyChar.App` is the WPF window. Details in [docs/adr/0001](docs/adr/0001-wpf-with-separate-core-library.md).

## Known limitations

- **Account-wide addon profiles are not copied.** Many addons (TomTom, Questie, RXPGuides...) keep their settings per account, with a profile per character. Pick the profile in the addon itself on the target character.
- **Copied settings need the step in game** described above; skipping it brings the old settings back later.
- **WoW Forever is in beta.** A client update may change how settings are stored.
- **Backups are never deleted automatically.** Remove old ones from **Open folder** in the **Backups** tab.
- **Write access.** If the game is installed in a folder you cannot write to, the copy fails with an access denied message; run CopyChar as administrator.
- The Yes/No buttons of the confirmations follow the Windows display language.
- Windows only.

## Roadmap

- Copy account-wide addon profiles.
- Show the differences between source and target before copying.
- Delete backups older than a given age.
