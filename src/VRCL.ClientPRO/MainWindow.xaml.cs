using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using VRCL.ClientPRO.Models;
using VRCL.ClientPRO.Services;

namespace VRCL.ClientPRO;

public partial class MainWindow : Window
{
    private const string AppVersion = "v1.1.01";
    private static readonly Regex VrchatIdRegex = new(
        @"\b(?:avtr|usr|wrld)_[A-Za-z0-9-]+\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly LocalLibraryService _library = new();
    private readonly InstanceJournalService _instanceJournal = new();
    private readonly ObservableCollection<InstanceObservation> _instanceEntries = [];
    private readonly ObservableCollection<AuthorizedAsset> _assets = [];

    public MainWindow()
    {
        InitializeComponent();
        AssetsDataGrid.ItemsSource = _assets;
        InstanceJournalDataGrid.ItemsSource = _instanceEntries;
        Loaded += async (_, _) =>
        {
            await RefreshLibraryAsync();
            await RefreshInstanceJournalAsync();
            AppendConsole($"VRCL PRO {AppVersion} ready.");
            AppendConsole("Type 'help' to see local commands.");
        };
        FooterStatusTextBlock.Text = $"Ready · Local data: {_library.RootDirectory}";
    }

    private void JoinInstance_Click(object sender, RoutedEventArgs e)
    {
        if (TryOpenInstanceLink(InstanceLinkTextBox.Text, out var message))
        {
            FooterStatusTextBlock.Text = message;
            AppendConsole(message);
        }
        else
        {
            MessageBox.Show(message, "Could not open VRChat link", MessageBoxButton.OK, MessageBoxImage.Warning);
            AppendConsole("ERROR: " + message);
        }
    }

    private bool TryOpenInstanceLink(string input, out string message)
    {
        var text = input.Trim();
        if (!Uri.TryCreate(text, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            (!uri.Host.Equals("vrchat.com", StringComparison.OrdinalIgnoreCase) &&
             !uri.Host.Equals("www.vrchat.com", StringComparison.OrdinalIgnoreCase)))
        {
            message = "Paste a valid HTTPS link hosted on vrchat.com.";
            return false;
        }

        try
        {
            Process.Start(new ProcessStartInfo(uri.AbsoluteUri) { UseShellExecute = true });
            message = "Opened the official VRChat link using your Windows browser/client handoff.";
            return true;
        }
        catch (Exception ex)
        {
            message = "Windows could not open the link: " + ex.Message;
            return false;
        }
    }

    private async Task RefreshInstanceJournalAsync()
    {
        try
        {
            var entries = await _instanceJournal.LoadAsync();
            _instanceEntries.Clear();
            foreach (var entry in entries.OrderByDescending(x => x.SavedUtc))
                _instanceEntries.Add(entry);
            AppendConsole($"Loaded {_instanceEntries.Count} saved instance journal entr(y/ies).");
        }
        catch (Exception ex)
        {
            AppendConsole("INSTANCE JOURNAL ERROR: " + ex.Message);
        }
    }

    private void ParseJournalLink_Click(object sender, RoutedEventArgs e)
    {
        var input = JournalInstanceLinkTextBox.Text.Trim();
        if (!Uri.TryCreate(input, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps ||
            (!uri.Host.Equals("vrchat.com", StringComparison.OrdinalIgnoreCase) &&
             !uri.Host.Equals("www.vrchat.com", StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show("Paste a valid HTTPS link hosted on vrchat.com.", "Invalid VRChat link",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var worldMatch = Regex.Match(input, @"wrld_[A-Za-z0-9-]+", RegexOptions.IgnoreCase);
        if (worldMatch.Success)
            JournalWorldIdTextBox.Text = worldMatch.Value;

        var query = uri.Query.TrimStart('?').Split('&', StringSplitOptions.RemoveEmptyEntries);
        foreach (var item in query)
        {
            var parts = item.Split('=', 2);
            if (parts.Length != 2)
                continue;

            var key = Uri.UnescapeDataString(parts[0]);
            var value = Uri.UnescapeDataString(parts[1]);
            if (key.Equals("instanceId", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("instance_id", StringComparison.OrdinalIgnoreCase))
                JournalLocationIdTextBox.Text = value;
            else if (key.Equals("worldId", StringComparison.OrdinalIgnoreCase) &&
                     string.IsNullOrWhiteSpace(JournalWorldIdTextBox.Text))
                JournalWorldIdTextBox.Text = value;
        }

        if (string.IsNullOrWhiteSpace(JournalLocationIdTextBox.Text))
        {
            var separator = input.IndexOf(':');
            if (separator >= 0 && worldMatch.Success && separator > input.IndexOf(worldMatch.Value, StringComparison.Ordinal))
                JournalLocationIdTextBox.Text = input[(separator + 1)..].Split('?', '&')[0];
        }

        AppendConsole($"Parsed supplied VRChat link. World ID: {EmptyAsDash(JournalWorldIdTextBox.Text)}; Location ID: {EmptyAsDash(JournalLocationIdTextBox.Text)}");
        FooterStatusTextBlock.Text = "Parsed the link you supplied. No live instance scan was performed.";
    }

    private async void SaveInstanceEntry_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(JournalWorldNameTextBox.Text) &&
                string.IsNullOrWhiteSpace(JournalWorldIdTextBox.Text) &&
                string.IsNullOrWhiteSpace(JournalLocationIdTextBox.Text) &&
                string.IsNullOrWhiteSpace(JournalInstanceLinkTextBox.Text) &&
                string.IsNullOrWhiteSpace(JournalNotesTextBox.Text))
            {
                MessageBox.Show("Enter at least a world name, ID, instance link, or note before saving.",
                    "Nothing to save", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var entry = await _instanceJournal.SaveAsync(
                JournalInstanceLinkTextBox.Text,
                JournalWorldNameTextBox.Text,
                JournalWorldIdTextBox.Text,
                JournalLocationIdTextBox.Text,
                JournalUserIdsTextBox.Text,
                JournalAvatarIdsTextBox.Text,
                JournalNotesTextBox.Text);

            _instanceEntries.Insert(0, entry);
            AppendConsole($"Saved instance journal entry: {EmptyAsDash(entry.WorldName)} / {EmptyAsDash(entry.WorldId)}");
            FooterStatusTextBlock.Text = "Instance details saved locally.";
            JournalWorldNameTextBox.Clear();
            JournalWorldIdTextBox.Clear();
            JournalLocationIdTextBox.Clear();
            JournalUserIdsTextBox.Clear();
            JournalAvatarIdsTextBox.Clear();
            JournalNotesTextBox.Clear();
            JournalInstanceLinkTextBox.Clear();
        }
        catch (Exception ex)
        {
            AppendConsole("Could not save instance entry: " + ex.Message);
            MessageBox.Show(ex.Message, "Instance journal error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BrowseAsset_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select a local VRChat asset file",
            Filter = "VRChat and Unity asset files (*.vrca;*.vrcw;*.unitypackage;*.zip)|*.vrca;*.vrcw;*.unitypackage;*.zip|All files (*.*)|*.*",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
        {
            AssetFileTextBox.Text = dialog.FileName;
            if (string.IsNullOrWhiteSpace(AssetNameTextBox.Text))
                AssetNameTextBox.Text = Path.GetFileNameWithoutExtension(dialog.FileName);
        }
    }

    private void BrowseThumbnail_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select an optional avatar or world preview image",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.webp;*.bmp)|*.png;*.jpg;*.jpeg;*.webp;*.bmp",
            CheckFileExists = true
        };

        if (dialog.ShowDialog(this) == true)
            ThumbnailFileTextBox.Text = dialog.FileName;
    }

    private async void ImportAsset_Click(object sender, RoutedEventArgs e)
    {
        if (!PermissionConfirmedCheckBox.IsChecked.GetValueOrDefault())
        {
            ShowImportError("Confirm that you own this local file or have permission to inspect and use it.");
            return;
        }

        if (string.IsNullOrWhiteSpace(AssetFileTextBox.Text))
        {
            ShowImportError("Select a local asset file first.");
            return;
        }

        ImportButton.IsEnabled = false;
        ImportStatusTextBlock.Text = "Importing local file and calculating SHA-256…";

        try
        {
            var selectedType = AssetTypeComboBox.SelectedItem as ComboBoxItem;
            var assetType = selectedType?.Content?.ToString() ?? "Other";

            var record = await _library.ImportAsync(
                assetType,
                AssetNameTextBox.Text,
                UserIdTextBox.Text,
                AvatarIdTextBox.Text,
                WorldNameTextBox.Text,
                WorldIdTextBox.Text,
                LocationIdTextBox.Text,
                AssetNotesTextBox.Text,
                AssetFileTextBox.Text,
                ThumbnailFileTextBox.Text);

            _assets.Insert(0, record);
            UpdateLibraryCount();

            ImportStatusTextBlock.Text = $"Imported successfully. SHA-256: {record.Sha256}";
            FooterStatusTextBlock.Text = $"Imported {record.AssetName} into the local library.";
            AppendConsole($"Imported {record.AssetType}: {record.AssetName}");
            AppendConsole($"Stored file: {record.StoredAssetPath}");
            AppendConsole($"SHA-256: {record.Sha256}");

            AssetNameTextBox.Clear();
            UserIdTextBox.Clear();
            AvatarIdTextBox.Clear();
            WorldNameTextBox.Clear();
            WorldIdTextBox.Clear();
            LocationIdTextBox.Clear();
            AssetNotesTextBox.Clear();
            AssetFileTextBox.Clear();
            ThumbnailFileTextBox.Clear();
            PermissionConfirmedCheckBox.IsChecked = false;
            AssetTypeComboBox.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ShowImportError(ex.Message);
            AppendConsole("IMPORT ERROR: " + ex.Message);
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
            AppendConsole("LIBRARY ERROR: " + ex.Message);
        }
    }

    private void UpdateLibraryCount()
    {
        LibraryCountTextBlock.Text = $"{_assets.Count} local asset record(s).";
    }

    private void OpenLibraryFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _library.OpenFolder();
            AppendConsole("Opened local library folder: " + _library.RootDirectory);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Could not open storage folder", MessageBoxButton.OK, MessageBoxImage.Error);
            AppendConsole("ERROR: " + ex.Message);
        }
    }

    private void OpenSelectedAsset_Click(object sender, RoutedEventArgs e)
    {
        if (AssetsDataGrid.SelectedItem is not AuthorizedAsset asset)
        {
            AppendConsole("Select an asset in the library first.");
            return;
        }

        try
        {
            if (!File.Exists(asset.StoredAssetPath))
            {
                AppendConsole("Asset file is missing: " + asset.StoredAssetPath);
                return;
            }

            Process.Start(new ProcessStartInfo(asset.StoredAssetPath) { UseShellExecute = true });
            AppendConsole("Opened local asset: " + asset.AssetName);
        }
        catch (Exception ex)
        {
            AppendConsole("Could not open asset: " + ex.Message);
        }
    }

    private async void RunConsoleCommand_Click(object sender, RoutedEventArgs e)
    {
        await ExecuteConsoleCommandAsync(ConsoleCommandTextBox.Text);
    }

    private async void ConsoleCommand_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
            return;

        e.Handled = true;
        await ExecuteConsoleCommandAsync(ConsoleCommandTextBox.Text);
    }

    private async Task ExecuteConsoleCommandAsync(string rawCommand)
    {
        var command = rawCommand.Trim();
        if (string.IsNullOrWhiteSpace(command))
            return;

        ConsoleCommandTextBox.Clear();
        AppendConsole("> " + command);

        if (command.Equals("clear", StringComparison.OrdinalIgnoreCase))
        {
            ConsoleOutputTextBox.Clear();
            return;
        }

        if (command.Equals("help", StringComparison.OrdinalIgnoreCase))
        {
            AppendConsole("Available local commands:");
            AppendConsole("  help                 Show this help.");
            AppendConsole("  version              Show the application version.");
            AppendConsole("  library list         List local imported asset metadata.");
            AppendConsole("  library open         Open the local storage folder.");
            AppendConsole("  instances list       List saved instance journal entries.");
            AppendConsole("  instances open       Open the local data folder.");
            AppendConsole("  ids <text or link>   Parse avatar/user/world IDs from text you provide.");
            AppendConsole("  join <VRChat link>   Open an official VRChat link through the normal client handoff.");
            AppendConsole("  clear                Clear the console output.");
            AppendConsole("No command scrapes instances, unlocks avatars, or downloads platform content.");
            return;
        }

        if (command.Equals("version", StringComparison.OrdinalIgnoreCase))
        {
            AppendConsole("VRCL PRO " + AppVersion);
            return;
        }

        if (command.Equals("library list", StringComparison.OrdinalIgnoreCase))
        {
            if (_assets.Count == 0)
            {
                AppendConsole("The local asset library is empty.");
                return;
            }

            foreach (var asset in _assets)
            {
                AppendConsole($"{asset.AssetType}: {asset.AssetName} | User: {EmptyAsDash(asset.UserId)} | Avatar: {EmptyAsDash(asset.AvatarId)} | World: {EmptyAsDash(asset.WorldId)} | Location: {EmptyAsDash(asset.LocationId)}");
            }
            return;
        }

        if (command.Equals("library open", StringComparison.OrdinalIgnoreCase))
        {
            _library.OpenFolder();
            AppendConsole("Opened: " + _library.RootDirectory);
            return;
        }

        if (command.Equals("instances list", StringComparison.OrdinalIgnoreCase))
        {
            if (_instanceEntries.Count == 0)
                AppendConsole("No instance journal entries have been saved.");
            foreach (var entry in _instanceEntries)
                AppendConsole($"{EmptyAsDash(entry.WorldName)} | {EmptyAsDash(entry.WorldId)} | {EmptyAsDash(entry.LocationId)} | {entry.SavedUtc:u}");
            return;
        }

        if (command.Equals("instances open", StringComparison.OrdinalIgnoreCase))
        {
            Process.Start(new ProcessStartInfo(_instanceJournal.RootDirectory) { UseShellExecute = true });
            AppendConsole("Opened local instance journal folder: " + _instanceJournal.RootDirectory);
            return;
        }

        if (command.StartsWith("ids ", StringComparison.OrdinalIgnoreCase))
        {
            var value = command[4..];
            var matches = VrchatIdRegex.Matches(value)
                .Select(match => match.Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (matches.Length == 0)
            {
                AppendConsole("No avatar (avtr_), user (usr_), or world (wrld_) IDs were found in the text you supplied.");
            }
            else
            {
                AppendConsole("IDs found in supplied text:");
                foreach (var id in matches)
                    AppendConsole("  " + id);
            }
            return;
        }

        if (command.StartsWith("join ", StringComparison.OrdinalIgnoreCase))
        {
            if (TryOpenInstanceLink(command[5..], out var message))
                AppendConsole(message);
            else
                AppendConsole("ERROR: " + message);
            return;
        }

        AppendConsole("Unknown command. Type 'help' for available commands.");
        await Task.CompletedTask;
    }

    private void AppendConsole(string message)
    {
        if (ConsoleOutputTextBox is null)
            return;

        ConsoleOutputTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        ConsoleOutputTextBox.ScrollToEnd();
    }

    private static string EmptyAsDash(string? value) =>
        string.IsNullOrWhiteSpace(value) ? "-" : value;

    private void ShowImportError(string message)
    {
        ImportStatusTextBlock.Text = message;
        MessageBox.Show(message, "Import not completed", MessageBoxButton.OK, MessageBoxImage.Warning);
    }
}
