namespace LanSweeper.Api.GraphQL;

/// <summary>
/// Reusable GraphQL fragments for query composition
/// </summary>
internal static class GraphQLFragments
{
	/// <summary>
	/// Basic asset information fragment
	/// </summary>
	public const string AssetBasicInfo = """
		fragment AssetBasicInfoFields on AssetBasicInfo {
			name
			domain
			ipAddress
			mac
			firstSeen
			lastSeen
			type
			userDomain
			userName
			fqdn
			description
		}
		""";

	/// <summary>
	/// Custom asset information fragment
	/// </summary>
	public const string AssetCustomInfo = """
		fragment AssetCustomFields on AssetCustom {
			manufacturer
			model
			serialNumber
			location
			contact
			comment
			warrantyDate
			purchaseDate
			stateName
			dnsName
			sku
			barcode
		}
		""";

	/// <summary>
	/// Complete asset fields fragment
	/// </summary>
	public const string CompleteAssetFields = """
		fragment CompleteAssetFields on Asset {
			assetBasicInfo {
				...AssetBasicInfoFields
			}
			assetCustom {
				...AssetCustomFields
			}
		}
		""";

	/// <summary>
	/// Site information fragment
	/// </summary>
	public const string SiteFields = """
		fragment SiteFields on Site {
			id
			name
			description
		}
		""";

	/// <summary>
	/// Pagination information fragment
	/// </summary>
	public const string PaginationFields = """
		fragment PaginationFields on CursorPagination {
			limit
			current
			next
			page
		}
		""";
}
