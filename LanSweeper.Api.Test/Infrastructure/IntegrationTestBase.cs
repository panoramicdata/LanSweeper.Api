namespace LanSweeper.Api.Test.Infrastructure;

/// <summary>
/// Base class for integration tests with LanSweeper API
/// </summary>
public abstract class IntegrationTestBase : IDisposable
{
	private bool _disposed;

	/// <summary>
	/// Gets the LanSweeper API client
	/// </summary>
	protected LanSweeperClient Client { get; }

	/// <summary>
	/// Gets the test configuration
	/// </summary>
	protected TestConfig Config { get; }

	/// <summary>
	/// Gets the cancellation token for test operations
	/// </summary>
	protected CancellationToken CancellationToken { get; }

	/// <summary>
	/// Gets the logger instance
	/// </summary>
	protected ILogger Logger { get; }

	/// <summary>
	/// Initializes a new instance of the integration test base
	/// </summary>
	protected IntegrationTestBase()
	{
		// Load configuration from user secrets
		var configuration = new ConfigurationBuilder()
			.AddUserSecrets<IntegrationTestBase>()
			.Build();

		Config = configuration.GetSection("LanSweeperApi").Get<TestConfig>()
			?? throw new InvalidOperationException(
				"Test configuration not found. Please set up user secrets. " +
				"See secrets.example.json for the required structure.");

		// Create logger
		var loggerFactory = LoggerFactory.Create(builder =>
		{
			_ = builder.AddConsole();
			_ = builder.AddDebug();
			_ = builder.SetMinimumLevel(Config.EnableVerboseLogging ? LogLevel.Debug : LogLevel.Information);
		});

		Logger = loggerFactory.CreateLogger(GetType().Name);

		// Create client options
		var options = new LanSweeperClientOptions
		{
			AccessToken = Config.AccessToken,
			EnableRequestLogging = Config.EnableVerboseLogging,
			EnableResponseLogging = Config.EnableVerboseLogging,
			Logger = Logger
		};

		// Create client
		Client = new LanSweeperClient(options);

		// Create cancellation token with 5 minute timeout
		var cts = new CancellationTokenSource(TimeSpan.FromMinutes(5));
		CancellationToken = cts.Token;

		Logger.LogInformation("Integration test initialized for {TestType}", GetType().Name);
	}

	/// <summary>
	/// Disposes the test resources
	/// </summary>
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		Client?.Dispose();
		_disposed = true;
		GC.SuppressFinalize(this);
	}
}
