using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using VRCL.ClientPRO.Models;
using VRCL.ClientPRO.Services;

namespace VRCL.ClientPRO;

public partial class MainWindow : Window
{
    private readonly LocalLibraryService _library = new();
    private readonly ObservableCollection<AuthorizedAsset> _assets = [];

    public MainWindow()
    {
        InitializeComponent();
        AssetsDataGrid.ItemsSource = _assets;
        Loaded += async (_, _) => await RefreshLibraryAsync();
        FooterStatusTextBlock.Text = $"Ready · Local data: {_library.RootDirectory}";
    }

    private void JoinInstance_Click(object sender, RoutedEventArgs e)
    {
        var text = InstanceLinkTextBox.Text.Trim();
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            (!uri.Host.Equals("vrchat.com", StringComparison.OrdinalIgnoreCase) &&
             !uri.Host.Equals("www.vrchat.com", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Paste a valid HTTPS link hosted on vrchat.com. Use an instance or invite link copied from VRChat.", "Invalid VRChat link", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            FooterStatusTextBlock.Text = "Opened official VRChat link using the Windows browser/client handoff.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Could not open link", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BrowseAsset_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select a creator-supplied VRChat avatar file",
            Filter = "VRChat avatar files (*.vrca)|*.vrca|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == true)
            AssetFileTextBox.Text = dialog.FileName;
    }

    private void BrowseConsent_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select the creator's signed permission document",
            Filter = "Documents and images|*.pdf;*.png;*.jpg;*.jpeg;*.webp;*.doc;*.docx|All files (*.*)|*.*",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) == true)
            ConsentFileTextBox.Text = dialog.FileName;
    }

    private async void ImportAsset_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionConfirmedCheckBox.IsChecked.GetValueOrDefault())
        {
            ShowImportError("Confirm that the contributor authorized the file transfer and that you have attached the permission document.");
            return;
        }
        if (string.IsNullOrWhiteSpace(AssetFileTextBox.Text) || string.IsNullOrWhiteSpace(ConsentFileTextBox.Text))
        {
            ShowImportError("Select both the .vrca file and the signed permission document.");
            return;
        }

        ImportButton.IsEnabled = false;
        ImportStatusTextBlock.Text = "Importing file and recording its hash…";
        try
        {
            var record = await _library.ImportAsync(
                AssetNameTextBox.Text,
                ContributorNameTextBox.Text,
                ContributorUserIdTextBox.Text,
                AvatarIdTextBox.Text,
                PermissionScopeTextBox.Text,
                AssetFileTextBox.Text,
                ConsentFileTextBox.Text);

            _assets.Insert(0, record);
            UpdateLibraryCount();
            ImportStatusTextBlock.Text = $"Imported successfully. SHA-256: {record.Sha256}";
            FooterStatusTextBlock.Text = $"Imported {record.AssetName} with its consent record.";
            AssetNameTextBox.Clear();
            ContributorNameTextBox.Clear();
            ContributorUserIdTextBox.Clear();
            AvatarIdTextBox.Clear();
            AssetFileTextBox.Clear();
            ConsentFileTextBox.Clear();
            PermissionConfirmedCheckBox.IsChecked = false;
        }
        catch (Exception ex)
        {
            ShowImportError(ex.Message);
        }
        finally
        {
            ImportButton.IsEnabled = true;
        }
    }

    private async Task RefreshLibraryAsync()
    {
        try
        {
            var records = await _library.LoadAsync();
            _assets.Clear();
            foreach (var record in records.OrderByDescending(x => x.ImportedUtc))
                _assets.Add(record);
            UpdateLibraryCount();
        }
        catch (Exception ex)
        {
            FooterStatusTextBlock.Text = "Could not read local library: " + ex.Message;
        }
    }

    private void UpdateLibraryCount()
    {
        LibraryCountTextBlock.Text = $"{_assets.Count} authorized asset record(s) stored locally.";
    }

    private void OpenLibraryFolder_Click(object sender, RoutedEventArgs e)
    {
        try { _library.OpenFolder(); }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Could not open storage folder", MessageBoxButton.OK, MessageBoxImage.Error); }
    }

    private void ShowImportError(string message)
    {
        ImportStatusTextBlock.Text = message;
        MessageBox.Show(message, "Import not completed", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
