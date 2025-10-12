namespace folderchat.Services.Mcp
{
    /// <summary>
    /// Interface for MCP (Model Context Protocol) client operations.
    /// Implementations provide connectivity to MCP servers via various transports (stdio, HTTP).
    /// Current implementation uses the official ModelContextProtocol.Core SDK.
    /// </summary>
    public interface IMcpClient : IDisposable
    {
        /// <summary>
        /// Establishes a connection to the MCP server.
        /// </summary>
        /// <returns>True if connection was successful, false otherwise.</returns>
        Task<bool> ConnectAsync();

        /// <summary>
        /// Disconnects from the MCP server and releases resources.
        /// </summary>
        Task DisconnectAsync();

        /// <summary>
        /// Initializes the MCP protocol with the server.
        /// </summary>
        /// <param name="request">Initialization parameters including client info and capabilities.</param>
        /// <returns>Server response containing server info and capabilities.</returns>
        Task<InitializeResponse> InitializeAsync(InitializeRequest request);

        /// <summary>
        /// Retrieves the list of tools available on the connected MCP server.
        /// </summary>
        /// <returns>List of available tools.</returns>
        Task<List<Tool>> ListToolsAsync();

        /// <summary>
        /// Invokes a specific tool on the MCP server.
        /// </summary>
        /// <param name="toolName">Name of the tool to call.</param>
        /// <param name="arguments">Arguments to pass to the tool (can be null, dictionary, or object).</param>
        /// <returns>Tool execution result.</returns>
        Task<CallToolResponse> CallToolAsync(string toolName, object? arguments);

        /// <summary>
        /// Gets a value indicating whether the client is currently connected to an MCP server.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Event raised when the client has a log message to report.
        /// </summary>
        event EventHandler<string>? LogMessage;
    }
}