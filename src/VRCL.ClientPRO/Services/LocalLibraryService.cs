using System.IO;
using System.Security.Cryptography;
using System.Text.Json;
using VRCL.ClientPRO.Models;

namespace VRCL.ClientPRO.Services;

public sealed class LocalLibraryService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public string RootDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VRCL",
        "ClientPRO");

    private string AssetsDirectory => Path.Combine(RootDirectory, "Assets");
    private string ConsentsDirectory => Path.Combine(RootDirectory, "Consents");
    private string CatalogPath => Path.Combine(RootDirectory, "Library.json");

    public LocalLibraryService()
    {
        Directory.CreateDirectory(AssetsDirectory);
        Directory.CreateDirectory(ConsentsDirectory);
    }

    public async Task<List<AuthorizedAsset>> LoadAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(CatalogPath))
            return [];

        await using var stream = File.OpenRead(CatalogPath);
        return await JsonSerializer.DeserializeAsync<List<AuthorizedAsset>>(
            stream, JsonOptions, cancellationToken) ?? [];
    }

    public async Task<AuthorizedAsset> ImportAsync(
        string assetName,
        string contributorName,
        string contributorUserId,
        string avatarId,
        string permissionScope,
        string sourceAssetPath,
        string sourceConsentPath,
        CancellationToken cancellationToken = default)
    {
        ValidateInput(assetName, nameof(assetName));
        ValidateInput(contributorName, nameof(contributorName));
        ValidateInput(permissionScope, nameof(permissionScope));

        if (!File.Exists(sourceAssetPath) ||
            !string.Equals(Path.GetExtension(sourceAssetPath), ".vrca", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Select an existing .vrca file supplied by the authorized contributor.");

        if (!File.Exists(sourceConsentPath))
            throw new InvalidOperationException("Select the contributor's signed permission document.");

        var recordId = Guid.NewGuid().ToString("N");
        var storedAssetPath = Path.Combine(AssetsDirectory, recordId + Path.GetExtension(sourceAssetPath));
        var storedConsentPath = Path.Combine(ConsentsDirectory, recordId + Path.GetExtension(sourceConsentPath));

        try
        {
            File.Copy(sourceAssetPath, storedAssetPath, overwrite: false);
            File.Copy(sourceConsentPath, storedConsentPath, overwrite: false);

            await using var fileStream = File.OpenRead(storedAssetPath);
            var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);
            var hash = Convert.ToHexString(hashBytes);

            var record = new AuthorizedAsset
            {
                RecordId = recordId,
                AssetName = assetName.Trim(),
                ContributorName = contributorName.Trim(),
                ContributorUserId = contributorUserId.Trim(),
                AvatarId = avatarId.Trim(),
                PermissionScope = permissionScope.Trim(),
                StoredAssetPath = storedAssetPath,
                StoredConsentPath = storedConsentPath,
                Sha256 = hash,
                ImportedUtc = DateTime.UtcNow
            };

            var records = await LoadAsync(cancellationToken);
            records.Insert(0, record);
            var temporaryPath = CatalogPath + "." + recordId + ".tmp";
            await File.WriteAllTextAsync(temporaryPath, JsonSerializer.Serialize(records, JsonOptions), cancellationToken);
            File.Move(temporaryPath, CatalogPath, overwrite: true);
            return record;
        }
        catch
        {
            TryDelete(storedAssetPath);
            TryDelete(storedConsentPath);
            throw;
        }
    }

    public void OpenFolder()
    {
        Directory.CreateDirectory(RootDirectory);
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = RootDirectory,
            UseShellExecute = true
        });
    }

    private static void ValidateInput(string value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException($"The {fieldName} field is required.", fieldName);
    }

    private static void TryDelete(string path)
    {
        try
        {
            if (File.Exists(path))
                File.Delete(path);
        }
        catch
        {
            // Preserve the original exception. The leftover file can be removed manually.
        }
    }
}
