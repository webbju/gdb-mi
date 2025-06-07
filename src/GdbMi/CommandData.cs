namespace GdbMi;

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

public class CommandData<TResult>
{
    public int Token { get; init; }

    public string Command { get; init; }

    public CancellationToken CancellationToken { get; init; }

    internal TaskCompletionSource<TResult> CompletionSource { get; init; }

    internal Action<TResult> ResultDelegate { get; init; }

    public TaskAwaiter<TResult> GetAwaiter() => CompletionSource.Task.GetAwaiter();

    public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) => CompletionSource.Task.ConfigureAwait(continueOnCapturedContext);
}
