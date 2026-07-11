using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace CopyChar.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainViewModel(AppStorage.GuessInstallPath(), Confirm);
        DataContext = _viewModel;
    }

    private bool Confirm(string question) =>
        MessageBox.Show(this, question, "CopyChar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;

    private void Browse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "World of Warcraft folder",
            InitialDirectory = _viewModel.InstallPath,
        };
        if (dialog.ShowDialog(this) != true)
            return;

        _viewModel.InstallPath = dialog.FolderName;
        AppStorage.SaveInstallPath(dialog.FolderName);
    }

    private void OpenBackupFolder_Click(object sender, RoutedEventArgs e)
    {
        Directory.CreateDirectory(AppStorage.BackupFolder);
        Process.Start("explorer.exe", AppStorage.BackupFolder);
    }
}
