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

    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".vrca",
        ".vrcw",
        ".unitypackage",
        ".zip"
    };

    private static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".webp",
        ".bmp"
    };

    public string RootDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VRCL",
        "ClientPRO");

    private string AssetsDirectory => Path.Combine(RootDirectory, "Assets");
    private string ThumbnailsDirectory => Path.Combine(RootDirectory, "Thumbnails");
    private string CatalogPath => Path.Combine(RootDirectory, "Library.json");

    public LocalLibraryService()
    {
        Directory.CreateDirectory(AssetsDirectory);
        Directory.CreateDirectory(ThumbnailsDirectory);
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
        string assetType,
        string? assetName,
        string? userId,
        string? avatarId,
        string? worldName,
        string? worldId,
        string? locationId,
        string? notes,
        string sourceAssetPath,
        string? sourceThumbnailPath,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(sourceAssetPath))
            throw new InvalidOperationException("Select an existing local asset file.");

        var extension = Path.GetExtension(sourceAssetPath);
        if (!SupportedExtensions.Contains(extension))
            throw new InvalidOperationException("Supported local asset formats are .vrca, .vrcw, .unitypackage, and .zip.");

        if (!string.IsNullOrWhiteSpace(sourceThumbnailPath))
        {
            if (!File.Exists(sourceThumbnailPath))
                throw new InvalidOperationException("The selected preview image could not be found.");

            var imageExtension = Path.GetExtension(sourceThumbnailPath);
            if (!SupportedImageExtensions.Contains(imageExtension))
                throw new InvalidOperationException("Preview images must be PNG, JPG, JPEG, WEBP, or BMP.");
        }

        var recordId = Guid.NewGuid().ToString("N");
        var storedAssetPath = Path.Combine(AssetsDirectory, recordId + extension);
        var storedThumbnailPath = string.Empty;

        try
        {
            File.Copy(sourceAssetPath, storedAssetPath, overwrite: false);

            if (!string.IsNullOrWhiteSpace(sourceThumbnailPath))
            {
                storedThumbnailPath = Path.Combine(
                    ThumbnailsDirectory,
                    recordId + Path.GetExtension(sourceThumbnailPath));
                File.Copy(sourceThumbnailPath, storedThumbnailPath, overwrite: false);
            }

            await using var fileStream = File.OpenRead(storedAssetPath);
            var hashBytes = await SHA256.HashDataAsync(fileStream, cancellationToken);
            var hash = Convert.ToHexString(hashBytes);

            var record = new AuthorizedAsset
            {
                RecordId = recordId,
                AssetType = string.IsNullOrWhiteSpace(assetType) ? "Other" : assetType.Trim(),
                AssetName = string.IsNullOrWhiteSpace(assetName)
                    ? Path.GetFileNameWithoutExtension(sourceAssetPath)
                    : assetName.Trim(),
                SourceFileName = Path.GetFileName(sourceAssetPath),
                UserId = userId?.Trim() ?? string.Empty,
                AvatarId = avatarId?.Trim() ?? string.Empty,
                WorldName = worldName?.Trim() ?? string.Empty,
                WorldId = worldId?.Trim() ?? string.Empty,
                LocationId = locationId?.Trim() ?? string.Empty,
                Notes = notes?.Trim() ?? string.Empty,
                StoredAssetPath = storedAssetPath,
                ThumbnailPath = storedThumbnailPath,
                Sha256 = hash,
                ImportedUtc = DateTime.UtcNow
            };

            var records = await LoadAsync(cancellationToken);
            records.Insert(0, record);

            var temporaryPath = CatalogPath + "." + recordId + ".tmp";
            await File.WriteAllTextAsync(
                temporaryPath,
                JsonSerializer.Serialize(records, JsonOptions),
                cancellationToken);
            File.Move(temporaryPath, CatalogPath, overwrite: true);

            return record;
        }
        catch
        {
            TryDelete(storedAssetPath);
            TryDelete(storedThumbnailPath);
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

    private static void TryDelete(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return;

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
