namespace CopyChar.Core;

// Files the client keeps in a character folder, grouped the way players think about them.
// cache.md5 is deliberately absent: the client compares it with the files to detect
// local edits, so the target must keep its own.
public sealed record SettingCategory(string Label, IReadOnlyList<string> Files)
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

    public static readonly SettingCategory AddOnList =
        new("Enabled addons", ["AddOns.txt"]);

    public static IReadOnlyList<SettingCategory> All { get; } =
        [GameOptions, KeyBindings, Macros, Interface, Chat, AddOnList];

    public override string ToString() => Label;
}
