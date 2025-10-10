namespace LanSweeper.Api.Test.IntegrationTests;

/// <summary>
/// Integration tests for Sites API
/// </summary>
public sealed class SitesApiTests : IntegrationTestBase
{
	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetAllAsync_ShouldReturnSites()
	{
		// Act
		var sites = await Client.Data.Sites.GetAllAsync(CancellationToken);

		// Assert
		_ = sites.Should().NotBeNull();
		_ = sites.Should().BeAssignableTo<IReadOnlyList<Site>>();

		if (sites.Count > 0)
		{
			var firstSite = sites[0];
			_ = firstSite.Id.Should().NotBeNullOrEmpty("Site ID should not be empty");
			_ = firstSite.Name.Should().NotBeNullOrEmpty("Site name should not be empty");

			Logger.LogInformation("Retrieved {Count} sites. First site: {SiteName}", sites.Count, firstSite.Name);
		}
		else
		{
			Logger.LogWarning("No sites returned from API");
		}
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetByIdAsync_WithValidId_ShouldReturnSite()
	{
		// Arrange - Get all sites first to get a valid ID
		var allSites = await Client.Data.Sites.GetAllAsync(CancellationToken);

		if (allSites.Count == 0)
		{
			Logger.LogWarning("Skipping test - no sites available");
			return;
		}

		var siteId = allSites[0].Id;

		// Act
		var site = await Client.Data.Sites.GetByIdAsync(siteId, CancellationToken);

		// Assert
		_ = site.Should().NotBeNull();
		_ = site.Id.Should().Be(siteId);
		_ = site.Name.Should().NotBeNullOrEmpty();

		Logger.LogInformation("Retrieved site by ID: {SiteName}", site.Name);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetByIdAsync_WithInvalidId_ShouldThrowNotFoundException()
	{
		// Arrange
		var invalidSiteId = "invalid-site-id-12345";

		// Act
		var act = async () => await Client.Data.Sites.GetByIdAsync(invalidSiteId, CancellationToken);

		// Assert
		_ = await act.Should().ThrowAsync<LanSweeperNotFoundException>()
			.WithMessage($"*{invalidSiteId}*");
	}
}
