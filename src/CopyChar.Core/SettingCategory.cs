namespace CopyChar.Core;

// Files the client keeps in a character folder, or at the root of the account folder when
// AccountWide, grouped the way players think about them.
// cache.md5 is deliberately absent: the client compares it with the files to detect
// local edits, so the target must keep its own.
public sealed record SettingCategory(string Label, IReadOnlyList<string> Files, bool AccountWide = false)
{
    public static readonly SettingCategory GameOptions =
        new("Game options", ["config-cache.wtf"]);

    public static readonly SettingCategory KeyBindings =
        new("Character-specific key bindings", ["bindings-cache.wtf"]);

    public static readonly SettingCategory Macros =
        new("Character macros", ["macros-cache.txt"]);

    public static readonly SettingCategory Interface =
        new("Interface layout and Edit Mode", ["layout-local.txt", "edit-mode-cache-character.txt"]);

    public static readonly SettingCategory Chat =
        new("Chat windows and text-to-speech", ["chat-cache.txt", "tts-cache-character.txt"]);

    public static readonly SettingCategory ClickBindings =
        new("Click bindings", ["click-bindings-cache.txt"]);

    public static readonly SettingCategory AddOnList =
        new("Enabled addons", ["AddOns.txt"]);

    public static readonly SettingCategory AccountKeyBindings =
        new("Key bindings", ["bindings-cache.wtf"], AccountWide: true);

    public static readonly SettingCategory AccountMacros =
        new("General macros", ["macros-cache.txt"], AccountWide: true);

    public static readonly SettingCategory AccountOptions =
        new("Account options", ["config-cache.wtf"], AccountWide: true);

    // The character file only stores which layout is active; the layouts themselves live here.
    public static readonly SettingCategory EditModeLayouts =
        new("Edit Mode layouts", ["edit-mode-cache-account.txt"], AccountWide: true);

    public static readonly SettingCategory AccountTextToSpeech =
        new("Text-to-speech", ["tts-cache-account.txt"], AccountWide: true);

    public static IReadOnlyList<SettingCategory> PerCharacter { get; } =
        [GameOptions, KeyBindings, Macros, Interface, Chat, ClickBindings, AddOnList];

    public static IReadOnlyList<SettingCategory> PerAccount { get; } =
        [AccountKeyBindings, AccountMacros, AccountOptions, EditModeLayouts, AccountTextToSpeech];

    public override string ToString() => Label;
}
