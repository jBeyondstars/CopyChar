namespace CopyChar.Core;

// Files the client keeps in a character folder, or at the root of the account folder when
// AccountWide, grouped the way players think about them.
// cache.md5 is deliberately absent: the client compares it with the files to detect
// local edits, so the target must keep its own.
public sealed record SettingCategory(string Label, IReadOnlyList<string> Files, bool AccountWide = false)
{
    // The client uses copied files for the session but only sends a setting to the server once it
    // changes in game; at a later login the server copy wins. Null when the file is not synced.
    public string? InGameSaveStep { get; init; }

    private const string OptionsStep = "Options: change any option and set it back.";
    private const string BindingsStep = "Key bindings: change a binding, set it back and click Okay.";
    private const string MacrosStep = "Macros: in /macro, add and remove a space in a macro, then close the window.";
    private const string EditModeStep = "Edit Mode: move a frame, move it back and save the layout.";
    private const string ChatStep = "Chat: change a chat window setting and set it back.";

    public static readonly SettingCategory GameOptions =
        new("Game options", ["config-cache.wtf"]) { InGameSaveStep = OptionsStep };

    public static readonly SettingCategory KeyBindings =
        new("Character-specific key bindings", ["bindings-cache.wtf"]) { InGameSaveStep = BindingsStep };

    public static readonly SettingCategory Macros =
        new("Character macros", ["macros-cache.txt"]) { InGameSaveStep = MacrosStep };

    public static readonly SettingCategory Interface =
        new("Interface layout and Edit Mode", ["layout-local.txt", "edit-mode-cache-character.txt"]) { InGameSaveStep = EditModeStep };

    public static readonly SettingCategory Chat =
        new("Chat windows and text-to-speech", ["chat-cache.txt", "tts-cache-character.txt"]) { InGameSaveStep = ChatStep };

    public static readonly SettingCategory ClickBindings =
        new("Click bindings", ["click-bindings-cache.txt"]) { InGameSaveStep = "Click bindings: change one and set it back." };

    public static readonly SettingCategory AddOnList =
        new("Enabled addons", ["AddOns.txt"]);

    public static readonly SettingCategory AccountKeyBindings =
        new("Key bindings", ["bindings-cache.wtf"], AccountWide: true) { InGameSaveStep = BindingsStep };

    public static readonly SettingCategory AccountMacros =
        new("General macros", ["macros-cache.txt"], AccountWide: true) { InGameSaveStep = MacrosStep };

    public static readonly SettingCategory AccountOptions =
        new("Account options", ["config-cache.wtf"], AccountWide: true) { InGameSaveStep = OptionsStep };

    // The character file only stores which layout is active; the layouts themselves live here.
    public static readonly SettingCategory EditModeLayouts =
        new("Edit Mode layouts", ["edit-mode-cache-account.txt"], AccountWide: true) { InGameSaveStep = EditModeStep };

    public static readonly SettingCategory AccountTextToSpeech =
        new("Text-to-speech", ["tts-cache-account.txt"], AccountWide: true) { InGameSaveStep = ChatStep };

    public static IReadOnlyList<SettingCategory> PerCharacter { get; } =
        [GameOptions, KeyBindings, Macros, Interface, Chat, ClickBindings, AddOnList];

    public static IReadOnlyList<SettingCategory> PerAccount { get; } =
        [AccountKeyBindings, AccountMacros, AccountOptions, EditModeLayouts, AccountTextToSpeech];

    public override string ToString() => Label;
}
