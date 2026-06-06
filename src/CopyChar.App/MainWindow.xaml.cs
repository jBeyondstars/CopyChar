using System.Windows;
using Microsoft.Win32;

namespace CopyChar.App;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel = new(AppStorage.GuessInstallPath());

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _viewModel;
    }

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
}
