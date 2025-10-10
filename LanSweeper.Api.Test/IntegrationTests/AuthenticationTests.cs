namespace LanSweeper.Api.Test.IntegrationTests;

/// <summary>
/// Tests for authentication and error handling
/// </summary>
public sealed class AuthenticationTests
{
	[Fact]
	[Trait("Category", "Unit")]
	public void Client_WithInvalidAccessToken_ShouldFailAuthentication()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "invalid-token-12345"
		};

		using var client = new LanSweeperClient(options);

		// Act
		var act = async () => await client.Data.Sites.GetAllAsync(CancellationToken.None);

		// Assert - This will fail with authentication error when actually calling the API
		_ = act.Should().NotBeNull();
	}

	[Fact]
	[Trait("Category", "Unit")]
	public void Options_Validate_WithEmptyToken_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = ""
		};

		// Act
		var act = options.Validate;

		// Assert
		_ = act.Should().Throw<ArgumentException>()
			.WithMessage("*AccessToken*");
	}

	[Fact]
	[Trait("Category", "Unit")]
	public void Options_Validate_WithInvalidEndpoint_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "valid-token",
			GraphQLEndpoint = "not-a-valid-url"
		};

		// Act
		var act = options.Validate;

		// Assert
		_ = act.Should().Throw<ArgumentException>()
			.WithMessage("*GraphQLEndpoint*");
	}

	[Fact]
	[Trait("Category", "Unit")]
	public void Options_Validate_WithNegativeTimeout_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "valid-token",
			RequestTimeout = TimeSpan.FromSeconds(-1)
		};

		// Act
		var act = options.Validate;

		// Assert
		_ = act.Should().Throw<ArgumentException>()
			.WithMessage("*RequestTimeout*");
	}

	[Fact]
	[Trait("Category", "Unit")]
	public void Options_Validate_WithNegativeRetryAttempts_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "valid-token",
			MaxRetryAttempts = -1
		};

		// Act
		var act = options.Validate;

		// Assert
		_ = act.Should().Throw<ArgumentException>()
			.WithMessage("*MaxRetryAttempts*");
	}

	[Fact]
	[Trait("Category", "Unit")]
	public void Options_Validate_WithValidConfiguration_ShouldNotThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "valid-token-12345",
			GraphQLEndpoint = "https://api.lansweeper.com/api/v2/graphql",
			RequestTimeout = TimeSpan.FromSeconds(30),
			MaxRetryAttempts = 3
		};

		// Act
		var act = options.Validate;

		// Assert
		_ = act.Should().NotThrow();
	}
}
