namespace GdbMi
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommandData<TResult>
    {
        public int Token { get; init; }

        public string Command { get; init; }

        public CancellationToken CancellationToken { get; init; }

        public Task<TResult> CompletionTask => CompletionSource.Task;

        internal TaskCompletionSource<TResult> CompletionSource { get; init; }

        internal Action<TResult> ResultDelegate { get; init; }

        public TaskAwaiter<TResult> GetAwaiter() => CompletionTask.GetAwaiter();

        public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext) => CompletionTask.ConfigureAwait(continueOnCapturedContext);
    }
}
