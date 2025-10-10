namespace LanSweeper.Api.Models;

/// <summary>
/// Basic asset information
/// </summary>
public sealed class AssetBasicInfo
{
	/// <summary>
	/// Gets or sets the asset name
	/// </summary>
	[JsonPropertyName("name")]
	public string? Name { get; init; }

	/// <summary>
	/// Gets or sets the domain name
	/// </summary>
	[JsonPropertyName("domain")]
	public string? Domain { get; init; }

	/// <summary>
	/// Gets or sets the IP address
	/// </summary>
	[JsonPropertyName("ipAddress")]
	public string? IpAddress { get; init; }

	/// <summary>
	/// Gets or sets the MAC address
	/// </summary>
	[JsonPropertyName("mac")]
	public string? Mac { get; init; }

	/// <summary>
	/// Gets or sets when the asset was first seen
	/// </summary>
	[JsonPropertyName("firstSeen")]
	public DateTime? FirstSeen { get; init; }

	/// <summary>
	/// Gets or sets when the asset was last seen
	/// </summary>
	[JsonPropertyName("lastSeen")]
	public DateTime? LastSeen { get; init; }

	/// <summary>
	/// Gets or sets the asset type
	/// </summary>
	[JsonPropertyName("type")]
	public string? Type { get; init; }

	/// <summary>
	/// Gets or sets the user domain
	/// </summary>
	[JsonPropertyName("userDomain")]
	public string? UserDomain { get; init; }

	/// <summary>
	/// Gets or sets the user name
	/// </summary>
	[JsonPropertyName("userName")]
	public string? UserName { get; init; }

	/// <summary>
	/// Gets or sets the fully qualified domain name
	/// </summary>
	[JsonPropertyName("fqdn")]
	public string? Fqdn { get; init; }

	/// <summary>
	/// Gets or sets the asset description
	/// </summary>
	[JsonPropertyName("description")]
	public string? Description { get; init; }
}
