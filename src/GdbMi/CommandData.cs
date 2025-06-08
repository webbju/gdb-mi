namespace GdbMi;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class CommandData<TResult>
{
    /// <summary>
    /// Gets or sets the token associated with the command.
    /// </summary>
    /// <remarks>
    /// The token, when present, is passed back when the command finishes to denote which command the result belongs to.
    /// </remarks>
    public int Token { get; init; }

    /// <summary>
    /// Gets or sets the GDB/MI command.
    /// </summary>
    public string Command { get; init; }

    /// <summary>
    /// Gets or sets the cancellation token for the command.
    /// </summary>
    public CancellationToken CancellationToken { get; init; }

    public TaskCompletionSource<TResult> CompletionSource { get; init; }

    /// <summary>
    /// Gets or sets the delegate to be invoked when the command completes.
    /// </summary>
    public Action<TResult> ResultDelegate { get; init; }

    /// <summary>
    /// Gets an awaiter used to await completion of the command.
    /// </summary>
    public TaskAwaiter<TResult> GetAwaiter()
    {
        return CompletionSource.Task.GetAwaiter();
    }

    /// <summary>
    /// Configures an awaiter used to await completion of the command.
    /// </summary>
    public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
    {
        return CompletionSource.Task.ConfigureAwait(continueOnCapturedContext);
    }
}
