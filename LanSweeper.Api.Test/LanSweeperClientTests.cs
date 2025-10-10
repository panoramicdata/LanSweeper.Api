namespace LanSweeper.Api.Test;

/// <summary>
/// Tests for LanSweeperClient initialization and basic functionality
/// </summary>
public sealed class LanSweeperClientTests
{
	[Fact]
	public void Constructor_WithValidOptions_ShouldSucceed()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token-12345"
		};

		// Act
		using var client = new LanSweeperClient(options);

		// Assert
		client.Should().NotBeNull();
		client.Data.Should().NotBeNull();
		client.Data.Sites.Should().NotBeNull();
		client.Data.Assets.Should().NotBeNull();
		client.Data.Reports.Should().NotBeNull();
		client.Data.Users.Should().NotBeNull();
	}

	[Fact]
	public void Constructor_WithNullOptions_ShouldThrow()
	{
		// Act
		var act = () => new LanSweeperClient(null!);

		// Assert
		act.Should().Throw<ArgumentNullException>();
	}

	[Fact]
	public void Constructor_WithInvalidOptions_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = ""
		};

		// Act
		var act = () => new LanSweeperClient(options);

		// Assert
		_ = act.Should().Throw<ArgumentException>();
	}

	[Fact]
	public void Dispose_ShouldNotThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token-12345"
		};
		var client = new LanSweeperClient(options);

		// Act & Assert - should not throw
		var act1 = client.Dispose;
		_ = act1.Should().NotThrow();

		// Multiple dispose calls should be safe
		var act2 = client.Dispose;
		_ = act2.Should().NotThrow();
	}
}
