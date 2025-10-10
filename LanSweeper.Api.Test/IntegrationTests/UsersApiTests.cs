namespace LanSweeper.Api.Test.IntegrationTests;

/// <summary>
/// Integration tests for Users API
/// </summary>
public sealed class UsersApiTests : IntegrationTestBase
{
	[Fact]
	[Trait("Category", "Integration")]
	public async Task GetCurrentAsync_ShouldReturnCurrentUser()
	{
		// Act
		var user = await Client.Data.Users.GetCurrentAsync(CancellationToken);

		// Assert
		_ = user.Should().NotBeNull();
		_ = user.Id.Should().NotBeNullOrEmpty("User ID should not be empty");

		Logger.LogInformation(
			"Current user: ID={Id}, Email={Email}, Name={Name}",
			user.Id,
			user.Email,
			user.Name);

		// Email and Name might be optional depending on API response
		if (!string.IsNullOrEmpty(user.Email))
		{
			_ = user.Email.Should().Contain("@", "Email should contain @ symbol");
		}
	}
}
