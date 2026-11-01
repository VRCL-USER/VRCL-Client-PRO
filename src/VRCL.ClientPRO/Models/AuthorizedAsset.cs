namespace VRCL.ClientPRO.Models;

public sealed class AuthorizedAsset
{
    public string RecordId { get; set; } = Guid.NewGuid().ToString("N");
    public string AssetName { get; set; } = string.Empty;
    public string ContributorName { get; set; } = string.Empty;
    public string ContributorUserId { get; set; } = string.Empty;
    public string AvatarId { get; set; } = string.Empty;
    public string PermissionScope { get; set; } = string.Empty;
    public string StoredAssetPath { get; set; } = string.Empty;
    public string StoredConsentPath { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    public DateTime ImportedUtc { get; set; } = DateTime.UtcNow;
}
