namespace LanSweeper.Api.GraphQL;

/// <summary>
/// GraphQL query definitions for LanSweeper API
/// </summary>
internal static class GraphQLQueries
{
	/// <summary>
	/// Query to get all authorized sites
	/// </summary>
	public const string GetAuthorizedSites = """
		query GetAuthorizedSites {
			authorizedSites {
				sites {
					id
					name
				}
			}
		}
		""";

	/// <summary>
	/// Query to get a specific site by ID
	/// </summary>
	public const string GetSiteById = """
		query GetSiteById($siteId: ID!) {
			site(id: $siteId) {
				id
				name
			}
		}
		""";

	/// <summary>
	/// Query to get assets by site
	/// </summary>
	public const string GetAssetsBySite = """
		query GetAssetsBySite($siteId: ID!, $limit: Int!) {
			site(id: $siteId) {
				assetResources(
					assetPagination: { limit: $limit }
				) {
					total
					items
				}
			}
		}
		""";

	/// <summary>
	/// Query to get current user information
	/// </summary>
	public const string GetCurrentUser = """
		query GetCurrentUser {
			me {
				id
				email
				name
			}
		}
		""";

	/// <summary>
	/// Query to get detailed asset information by ID
	/// </summary>
	public const string GetAssetById = """
		query GetAssetById($assetId: ID!) {
			asset(id: $assetId) {
				...DetailedAssetFields
			}
		}
		
		fragment DetailedAssetFields on Asset {
			assetBasicInfo {
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
			assetCustom {
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
		}
		""";
}
