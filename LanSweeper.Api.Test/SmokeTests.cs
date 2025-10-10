namespace LanSweeper.Api.Test;

/// <summary>
/// Basic smoke tests to verify project setup
/// </summary>
public sealed class SmokeTests
{
	[Fact]
	public void ProjectSetup_ShouldCompile() =>
		true.Should().BeTrue();
}
