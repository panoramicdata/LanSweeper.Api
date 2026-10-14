namespace LanSweeper.Api.Test;

using System.Net;

/// <summary>
/// Unit tests for LanSweeper exception types
/// </summary>
public sealed class ExceptionTests
{
	[Fact]
	public void LanSweeperRateLimitException_MessageOnly_SetsProperties()
	{
		// Act
		var ex = new LanSweeperRateLimitException("rate limited");

		// Assert
		ex.Message.Should().Be("rate limited");
		ex.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
		ex.RetryAfter.Should().BeNull();
		ex.ErrorDetails.Should().BeNull();
	}

	[Fact]
	public void LanSweeperRateLimitException_MessageAndDetails_SetsErrorDetails()
	{
		// Act
		var ex = new LanSweeperRateLimitException("rate limited", "too many requests");

		// Assert
		ex.ErrorDetails.Should().Be("too many requests");
		ex.RetryAfter.Should().BeNull();
	}

	[Fact]
	public void LanSweeperRateLimitException_FullConstructor_SetsRetryAfter()
	{
		// Arrange
		var retryAfter = TimeSpan.FromSeconds(30);

		// Act
		var ex = new LanSweeperRateLimitException("rate limited", "details", retryAfter);

		// Assert
		ex.ErrorDetails.Should().Be("details");
		ex.RetryAfter.Should().Be(retryAfter);
	}

	[Fact]
	public void LanSweeperRateLimitException_InnerException_SetsInner()
	{
		// Arrange
		var inner = new InvalidOperationException("wrapped");

		// Act
		var ex = new LanSweeperRateLimitException("rate limited", inner);

		// Assert
		ex.InnerException.Should().BeSameAs(inner);
	}

	[Fact]
	public void LanSweeperBadRequestException_MessageOnly_SetsProperties()
	{
		// Act
		var ex = new LanSweeperBadRequestException("bad request");

		// Assert
		ex.Message.Should().Be("bad request");
		ex.StatusCode.Should().Be(HttpStatusCode.BadRequest);
		ex.ErrorDetails.Should().BeNull();
	}

	[Fact]
	public void LanSweeperBadRequestException_MessageAndDetails_SetsErrorDetails()
	{
		// Act
		var ex = new LanSweeperBadRequestException("bad request", "details here");

		// Assert
		ex.ErrorDetails.Should().Be("details here");
	}

	[Fact]
	public void LanSweeperBadRequestException_InnerException_SetsInner()
	{
		// Arrange
		var inner = new Exception("wrapped");

		// Act
		var ex = new LanSweeperBadRequestException("bad request", inner);

		// Assert
		ex.InnerException.Should().BeSameAs(inner);
	}

	[Fact]
	public void LanSweeperNotFoundException_MessageOnly_SetsProperties()
	{
		// Act
		var ex = new LanSweeperNotFoundException("not found");

		// Assert
		ex.Message.Should().Be("not found");
		ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
		ex.ErrorDetails.Should().BeNull();
	}

	[Fact]
	public void LanSweeperNotFoundException_MessageAndDetails_SetsErrorDetails()
	{
		// Act
		var ex = new LanSweeperNotFoundException("not found", "details");

		// Assert
		ex.ErrorDetails.Should().Be("details");
	}

	[Fact]
	public void LanSweeperNotFoundException_InnerException_SetsInner()
	{
		// Arrange
		var inner = new Exception("wrapped");

		// Act
		var ex = new LanSweeperNotFoundException("not found", inner);

		// Assert
		ex.InnerException.Should().BeSameAs(inner);
	}

	[Fact]
	public void LanSweeperAuthenticationException_MessageOnly_SetsProperties()
	{
		// Act
		var ex = new LanSweeperAuthenticationException("auth failed");

		// Assert
		ex.Message.Should().Be("auth failed");
		ex.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
		ex.ErrorDetails.Should().BeNull();
	}

	[Fact]
	public void LanSweeperAuthenticationException_MessageAndDetails_SetsErrorDetails()
	{
		// Act
		var ex = new LanSweeperAuthenticationException("auth failed", "invalid token");

		// Assert
		ex.ErrorDetails.Should().Be("invalid token");
	}

	[Fact]
	public void LanSweeperAuthenticationException_InnerException_SetsInner()
	{
		// Arrange
		var inner = new Exception("wrapped");

		// Act
		var ex = new LanSweeperAuthenticationException("auth failed", inner);

		// Assert
		ex.InnerException.Should().BeSameAs(inner);
	}

	[Fact]
	public void LanSweeperException_MessageOnly_SetsMessage()
	{
		// Act
		var ex = new LanSweeperException("error");

		// Assert
		ex.Message.Should().Be("error");
		ex.StatusCode.Should().BeNull();
		ex.ErrorDetails.Should().BeNull();
	}

	[Fact]
	public void LanSweeperException_MessageAndInner_SetsInner()
	{
		// Arrange
		var inner = new Exception("wrapped");

		// Act
		var ex = new LanSweeperException("error", inner);

		// Assert
		ex.InnerException.Should().BeSameAs(inner);
	}

	[Fact]
	public void LanSweeperException_MessageAndStatus_SetsStatusCode()
	{
		// Act
		var ex = new LanSweeperException("error", HttpStatusCode.ServiceUnavailable);

		// Assert
		ex.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
	}

	[Fact]
	public void LanSweeperException_FullConstructor_SetsAll()
	{
		// Act
		var ex = new LanSweeperException("error", HttpStatusCode.InternalServerError, "details");

		// Assert
		ex.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
		ex.ErrorDetails.Should().Be("details");
	}

	[Fact]
	public void LanSweeperGraphQLException_MessageAndErrors_SetsErrors()
	{
		// Arrange
		var errors = new List<GraphQLError>
		{
			new() { Message = "first error" },
			new() { Message = "second error" }
		};

		// Act
		var ex = new LanSweeperGraphQLException("graphql failed", errors);

		// Assert
		ex.Errors.Should().BeEquivalentTo(errors);
		ex.Message.Should().Be("graphql failed");
	}

	[Fact]
	public void LanSweeperGraphQLException_MessageAndInner_SetsEmptyErrors()
	{
		// Arrange
		var inner = new Exception("wrapped");

		// Act
		var ex = new LanSweeperGraphQLException("graphql failed", inner);

		// Assert
		ex.Errors.Should().BeEmpty();
		ex.InnerException.Should().BeSameAs(inner);
	}
}
