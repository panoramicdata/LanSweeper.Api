namespace LanSweeper.Api.Models;

/// <summary>
/// Custom asset information
/// </summary>
public sealed class AssetCustom
{
	/// <summary>
	/// Gets or sets the manufacturer
	/// </summary>
	[JsonPropertyName("manufacturer")]
	public string? Manufacturer { get; init; }

	/// <summary>
	/// Gets or sets the model
	/// </summary>
	[JsonPropertyName("model")]
	public string? Model { get; init; }

	/// <summary>
	/// Gets or sets the serial number
	/// </summary>
	[JsonPropertyName("serialNumber")]
	public string? SerialNumber { get; init; }

	/// <summary>
	/// Gets or sets the location
	/// </summary>
	[JsonPropertyName("location")]
	public string? Location { get; init; }

	/// <summary>
	/// Gets or sets the contact information
	/// </summary>
	[JsonPropertyName("contact")]
	public string? Contact { get; init; }

	/// <summary>
	/// Gets or sets comments
	/// </summary>
	[JsonPropertyName("comment")]
	public string? Comment { get; init; }

	/// <summary>
	/// Gets or sets the warranty date
	/// </summary>
	[JsonPropertyName("warrantyDate")]
	public DateTime? WarrantyDate { get; init; }

	/// <summary>
	/// Gets or sets the purchase date
	/// </summary>
	[JsonPropertyName("purchaseDate")]
	public DateTime? PurchaseDate { get; init; }

	/// <summary>
	/// Gets or sets the state name
	/// </summary>
	[JsonPropertyName("stateName")]
	public string? StateName { get; init; }

	/// <summary>
	/// Gets or sets the DNS name
	/// </summary>
	[JsonPropertyName("dnsName")]
	public string? DnsName { get; init; }

	/// <summary>
	/// Gets or sets the SKU
	/// </summary>
	[JsonPropertyName("sku")]
	public string? Sku { get; init; }

	/// <summary>
	/// Gets or sets the barcode
	/// </summary>
	[JsonPropertyName("barcode")]
	public string? Barcode { get; init; }
}
