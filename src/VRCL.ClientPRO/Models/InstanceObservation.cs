namespace VRCL.ClientPRO.Models;

public sealed class InstanceObservation
{
    public string RecordId { get; set; } = Guid.NewGuid().ToString("N");
    public string InstanceLink { get; set; } = string.Empty;
    public string WorldName { get; set; } = string.Empty;
    public string WorldId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string UserIds { get; set; } = string.Empty;
    public string AvatarIds { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public DateTime SavedUtc { get; set; } = DateTime.UtcNow;
}
