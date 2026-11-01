namespace VRCL.ClientPRO.Models;

public sealed class AuthorizedAsset
{
    public string RecordId { get; set; } = Guid.NewGuid().ToString("N");
    public string AssetType { get; set; } = "Avatar";
    public string AssetName { get; set; } = string.Empty;
    public string SourceFileName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string AvatarId { get; set; } = string.Empty;
    public string WorldName { get; set; } = string.Empty;
    public string WorldId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string StoredAssetPath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public DateTime ImportedUtc { get; set; } = DateTime.UtcNow;
}
