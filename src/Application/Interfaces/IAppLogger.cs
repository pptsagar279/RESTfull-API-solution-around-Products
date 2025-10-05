namespace ProductAPI.Application.Interfaces;

/// <summary>
/// Application logger abstraction
/// </summary>
/// <typeparam name="T">The type whose name is used for the logger category</typeparam>
public interface IAppLogger<T>
{
    /// <summary>
    /// Logs an informational message
    /// </summary>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Logs a warning message
    /// </summary>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Logs an error message
    /// </summary>
    void LogError(string message, params object[] args);

    /// <summary>
    /// Logs an error message with exception
    /// </summary>
    void LogError(Exception exception, string message, params object[] args);

    /// <summary>
    /// Logs a debug message
    /// </summary>
    void LogDebug(string message, params object[] args);

    /// <summary>
    /// Logs a trace message
    /// </summary>
    void LogTrace(string message, params object[] args);
}

