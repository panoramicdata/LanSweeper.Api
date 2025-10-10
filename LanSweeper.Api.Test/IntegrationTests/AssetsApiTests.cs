namespace LanSweeper.Api.Test.IntegrationTests;

/// <summary>
/// Integration tests for Assets API
/// </summary>
public sealed class AssetsApiTests : IntegrationTestBase
{
	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetBySiteAsync_ShouldReturnAssets()
	{
		// Arrange - Get a valid site ID first
		var sites = await Client.Data.Sites.GetAllAsync(CancellationToken);

		if (sites.Count == 0)
		{
			Logger.LogWarning("Skipping test - no sites available");
			return;
		}

		var siteId = sites[0].Id;

		// Act
		var assets = await Client.Data.Assets.GetBySiteAsync(siteId, CancellationToken);

		// Assert
		_ = assets.Should().NotBeNull();
		_ = assets.Should().BeAssignableTo<IReadOnlyList<Asset>>();

		Logger.LogInformation("Retrieved {Count} assets from site: {SiteId}", assets.Count, siteId);

		if (assets.Count > 0)
		{
			var firstAsset = assets[0];
			_ = firstAsset.Should().NotBeNull();

			if (firstAsset.BasicInfo is not null)
			{
				Logger.LogInformation(
					"First asset: Name={Name}, IP={IpAddress}, Type={Type}",
					firstAsset.BasicInfo.Name,
					firstAsset.BasicInfo.IpAddress,
					firstAsset.BasicInfo.Type);
			}
		}
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetBySiteAsync_WithInvalidSiteId_ShouldThrowException()
	{
		// Arrange
		var invalidSiteId = "invalid-site-id-12345";

		// Act
		var act = async () => await Client.Data.Assets.GetBySiteAsync(invalidSiteId, CancellationToken);

		// Assert
		_ = await act.Should().ThrowAsync<LanSweeperException>();
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetByIdAsync_WithValidId_ShouldReturnAsset()
	{
		// Arrange - Get assets first to get a valid asset ID
		var sites = await Client.Data.Sites.GetAllAsync(CancellationToken);

		if (sites.Count == 0)
		{
			Logger.LogWarning("Skipping test - no sites available");
			return;
		}

		var siteId = sites[0].Id;
		var assets = await Client.Data.Assets.GetBySiteAsync(siteId, CancellationToken);

		if (assets.Count == 0)
		{
			Logger.LogWarning("Skipping test - no assets available in site {SiteId}", siteId);
			return;
		}

		var assetId = assets[0].Id ?? throw new InvalidOperationException("Asset ID is null");

		// Act
		var asset = await Client.Data.Assets.GetByIdAsync(assetId, CancellationToken);

		// Assert
		_ = asset.Should().NotBeNull();
		_ = asset.Id.Should().Be(assetId);

		Logger.LogInformation("Retrieved asset by ID: {AssetName}", asset.BasicInfo?.Name);
	}

	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetBySiteAsync_WithEmptySiteId_ShouldThrowArgumentException()
	{
		// Act
		var act = async () => await Client.Data.Assets.GetBySiteAsync("", CancellationToken);

		// Assert
		_ = await act.Should().ThrowAsync<ArgumentException>();
	}
}
