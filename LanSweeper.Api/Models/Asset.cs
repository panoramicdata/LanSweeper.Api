namespace LanSweeper.Api.Models;

/// <summary>
/// Represents a LanSweeper IT asset
/// </summary>
public sealed class Asset
{
	/// <summary>
	/// Gets or sets the asset identifier
	/// </summary>
	[JsonPropertyName("id")]
	public string? Id { get; init; }

	/// <summary>
	/// Gets or sets the basic asset information
	/// </summary>
	[JsonPropertyName("assetBasicInfo")]
	public AssetBasicInfo? BasicInfo { get; init; }

	/// <summary>
	/// Gets or sets the custom asset information
	/// </summary>
	[JsonPropertyName("assetCustom")]
	public AssetCustom? Custom { get; init; }
}
