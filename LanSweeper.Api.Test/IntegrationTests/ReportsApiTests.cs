namespace LanSweeper.Api.Test.IntegrationTests;

/// <summary>
/// Integration tests for Reports API (custom queries)
/// </summary>
public sealed class ReportsApiTests : IntegrationTestBase
{
	[Fact]
	[Trait("Category", "Integration")]
	public async Task ExecuteQueryAsync_WithValidQuery_ShouldReturnData()
	{
		// Arrange - Use the GetAuthorizedSites query as a test
		var query = """
			query GetAuthorizedSites {
				authorizedSites {
					sites {
						id
						name
					}
				}
			}
			""";

		// Act
		var result = await Client.Data.Reports.ExecuteQueryAsync<AuthorizedSitesResponse>(
			query,
			null,
			CancellationToken);

		// Assert
		_ = result.Should().NotBeNull();
		_ = result.AuthorizedSites.Should().NotBeNull();
		_ = result.AuthorizedSites.Sites.Should().NotBeNull();

		Logger.LogInformation(
			"Custom query executed successfully. Sites count: {Count}",
			result.AuthorizedSites.Sites?.Count ?? 0);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task ExecuteQueryAsync_WithInvalidQuery_ShouldThrowGraphQLException()
	{
		// Arrange - Invalid query syntax
		var query = """
			query InvalidQuery {
				nonExistentField {
					data
				}
			}
			""";

		// Act
		var act = async () => await Client.Data.Reports.ExecuteQueryAsync<object>(
			query,
			null,
			CancellationToken);

		// Assert
		_ = await act.Should().ThrowAsync<LanSweeperGraphQLException>();
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task ExecuteQueryAsync_WithVariables_ShouldReturnData()
	{
		// Arrange - Get a valid site ID first
		var sites = await Client.Data.Sites.GetAllAsync(CancellationToken);

		if (sites.Count == 0)
		{
			Logger.LogWarning("Skipping test - no sites available");
			return;
		}

		var siteId = sites[0].Id;

		var query = """
			query GetSiteById($siteId: ID!) {
				site(id: $siteId) {
					id
					name
				}
			}
			""";

		var variables = new Dictionary<string, object>
		{
			{ "siteId", siteId }
		};

		// Act
		var result = await Client.Data.Reports.ExecuteQueryAsync<SiteResponse>(
			query,
			variables,
			CancellationToken);

		// Assert
		_ = result.Should().NotBeNull();
		_ = result.Site.Should().NotBeNull();
		_ = result.Site.Id.Should().Be(siteId);

		Logger.LogInformation("Query with variables executed successfully. Site: {SiteName}", result.Site.Name);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task ExecuteQueryAsync_WithEmptyQuery_ShouldThrowArgumentException()
	{
		// Act
		var act = async () => await Client.Data.Reports.ExecuteQueryAsync<object>(
			"",
			null,
			CancellationToken);

		// Assert
		_ = await act.Should().ThrowAsync<ArgumentException>();
	}
}
