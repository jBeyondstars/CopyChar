namespace CopyChar.Core;

public sealed record Character(string Account, string Realm, string Name, string FolderPath)
{
    public override string ToString() => $"{Name} ({Realm})";
}
