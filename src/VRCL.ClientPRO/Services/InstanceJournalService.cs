using System.IO;
using System.Text.Json;
using VRCL.ClientPRO.Models;

namespace VRCL.ClientPRO.Services;

public sealed class InstanceJournalService
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

    private string JournalPath => Path.Combine(RootDirectory, "Instances.json");

    public async Task<List<InstanceObservation>> LoadAsync(CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(RootDirectory);
        if (!File.Exists(JournalPath))
            return [];

        await using var stream = File.OpenRead(JournalPath);
        return await JsonSerializer.DeserializeAsync<List<InstanceObservation>>(
            stream, JsonOptions, cancellationToken) ?? [];
    }

    public async Task<InstanceObservation> SaveAsync(
        string? instanceLink,
        string? worldName,
        string? worldId,
        string? locationId,
        string? userIds,
        string? avatarIds,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(RootDirectory);
        var entries = await LoadAsync(cancellationToken);
        var entry = new InstanceObservation
        {
            InstanceLink = instanceLink?.Trim() ?? string.Empty,
            WorldName = worldName?.Trim() ?? string.Empty,
            WorldId = worldId?.Trim() ?? string.Empty,
            LocationId = locationId?.Trim() ?? string.Empty,
            UserIds = userIds?.Trim() ?? string.Empty,
            AvatarIds = avatarIds?.Trim() ?? string.Empty,
            Notes = notes?.Trim() ?? string.Empty,
            SavedUtc = DateTime.UtcNow
        };

        entries.Insert(0, entry);
        var tempPath = JournalPath + "." + entry.RecordId + ".tmp";
        await File.WriteAllTextAsync(tempPath, JsonSerializer.Serialize(entries, JsonOptions), cancellationToken);
        File.Move(tempPath, JournalPath, overwrite: true);
        return entry;
    }
}
