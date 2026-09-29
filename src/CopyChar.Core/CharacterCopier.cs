namespace CopyChar.Core;

public sealed class CharacterCopier(BackupStore backups)
{
    // Returns one backup per folder written to: target accounts first, then target characters.
    public IReadOnlyList<Backup> Execute(CopyPlan plan)
    {
        var made = new List<Backup>();

        foreach (var account in plan.TargetAccounts)
        {
            made.Add(backups.CreateForAccount(account));
            CopyFiles(plan.Source.AccountFolder, account, plan.AccountFiles);
        }

        foreach (var target in plan.Targets)
        {
            var bindingsFromAccount = plan.BindingsFromSourceAccount.Contains(target);
            if (plan.Files.Count == 0 && !bindingsFromAccount)
                continue;

            made.Add(backups.Create(target));
            CopyFiles(plan.Source.FolderPath, target.FolderPath, plan.Files);
            if (bindingsFromAccount)
                CopyFiles(plan.Source.AccountFolder, target.FolderPath, [CopyPlan.BindingsFile]);
        }

        return made;
    }

    private static void CopyFiles(string sourceFolder, string targetFolder, IEnumerable<string> files)
    {
        foreach (var file in files)
        {
            var destination = Path.Combine(targetFolder, file);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(Path.Combine(sourceFolder, file), destination, overwrite: true);
        }
    }
}
