using LanSweeper.Api.Api;

namespace LanSweeper.Api;

/// <summary>
/// Client for interacting with the LanSweeper GraphQL API
/// </summary>
public sealed class LanSweeperClient : ILanSweeperClient
{
	private readonly LanSweeperClientOptions _options;
	private readonly GraphQLHttpClient _graphQLClient;
	private bool _disposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="LanSweeperClient"/> class
	/// </summary>
	/// <param name="options">Configuration options for the client</param>
	/// <exception cref="ArgumentNullException">Thrown when options is null</exception>
	/// <exception cref="ArgumentException">Thrown when options validation fails</exception>
	public LanSweeperClient(LanSweeperClientOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);
		options.Validate();

		_options = options;
		_graphQLClient = CreateGraphQLClient();

		// Initialize Data API module
		Data = new DataApi(_graphQLClient, _options.Logger);
	}

	/// <summary>
	/// Gets the Data API for GraphQL operations (sites, assets, users, reports)
	/// </summary>
	public IDataApi Data { get; }

	private GraphQLHttpClient CreateGraphQLClient()
	{
		var handler = CreateHandlerChain();

		var httpClient = new HttpClient(handler)
		{
			BaseAddress = new Uri(_options.GraphQLEndpoint),
			Timeout = _options.RequestTimeout
		};

		var graphQLOptions = new GraphQLHttpClientOptions
		{
			EndPoint = new Uri(_options.GraphQLEndpoint)
		};

		return new GraphQLHttpClient(
			graphQLOptions,
			new SystemTextJsonSerializer(),
			httpClient);
	}

	private DelegatingHandler CreateHandlerChain()
	{
		// Build the handler chain from innermost to outermost
		// Order: HttpClientHandler -> Auth -> Retry -> Logging -> Error

		var innerHandler = new HttpClientHandler();

		DelegatingHandler chain = new AuthenticationHandler(_options)
		{
			InnerHandler = innerHandler
		};

		if (_options.MaxRetryAttempts > 0)
		{
			chain = new RetryHandler(_options)
			{
				InnerHandler = chain
			};
		}

		if (_options.EnableRequestLogging || _options.EnableResponseLogging)
		{
			chain = new LoggingHandler(_options)
			{
				InnerHandler = chain
			};
		}

		chain = new ErrorHandler(_options.Logger)
		{
			InnerHandler = chain
		};

		return chain;
	}

	/// <summary>
	/// Disposes the client and releases resources
	/// </summary>
	public void Dispose()
	{
		if (_disposed)
		{
			return;
		}

		_graphQLClient?.Dispose();
		_disposed = true;
		GC.SuppressFinalize(this);
	}
}
