namespace LanSweeper.Api.Test;

/// <summary>
/// Tests for LanSweeperClientOptions configuration
/// </summary>
public sealed class LanSweeperClientOptionsTests
{
	[Fact]
	public void Constructor_WithValidOptions_ShouldSucceed()
	{
		// Arrange & Act
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token-12345"
		};

		// Assert
		options.AccessToken.Should().Be("test-token-12345");
		options.GraphQLEndpoint.Should().Be("https://api.lansweeper.com/api/v2/graphql");
		options.RequestTimeout.Should().Be(TimeSpan.FromSeconds(30));
		options.MaxRetryAttempts.Should().Be(3);
	}

	[Fact]
	public void Validate_WithMissingAccessToken_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = ""
		};

		// Act
		var act = options.Validate;

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*AccessToken*");
	}

	[Fact]
	public void Validate_WithInvalidEndpoint_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token",
			GraphQLEndpoint = "not-a-valid-url"
		};

		// Act
		var act = options.Validate;

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*GraphQLEndpoint*");
	}

	[Fact]
	public void Validate_WithNegativeRetryAttempts_ShouldThrow()
	{
		// Arrange
		var options = new LanSweeperClientOptions
		{
			AccessToken = "test-token",
			MaxRetryAttempts = -1
		};

		// Act
		var act = options.Validate;

		// Assert
		act.Should().Throw<ArgumentException>()
			.WithMessage("*MaxRetryAttempts*");
	}
}
