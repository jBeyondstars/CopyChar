namespace CopyChar.Core;

public sealed record Character(string Account, string Realm, string Name, string FolderPath)
{
    // folderPath is <...>\Account\<account>\<realm>\<name>.
    public static Character FromFolder(string folderPath)
    {
        var realmFolder = Path.GetDirectoryName(folderPath)!;
        var accountFolder = Path.GetDirectoryName(realmFolder)!;
        return new Character(
            Path.GetFileName(accountFolder),
            Path.GetFileName(realmFolder),
            Path.GetFileName(folderPath),
            folderPath);
    }

    public override string ToString() => $"{Name} ({Realm})";
}
