namespace CopyChar.Core;

public sealed class CharacterCopier(BackupStore backups)
{
    // Returns the backup made for each target, in the order of plan.Targets.
    public IReadOnlyList<Backup> Execute(CopyPlan plan)
    {
        var made = new List<Backup>();
        foreach (var target in plan.Targets)
        {
            made.Add(backups.Create(target));

            foreach (var file in plan.Files)
            {
                var destination = Path.Combine(target.FolderPath, file);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                File.Copy(Path.Combine(plan.Source.FolderPath, file), destination, overwrite: true);
            }
        }

        return made;
    }
}
