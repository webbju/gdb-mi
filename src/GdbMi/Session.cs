namespace GdbMi;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GdbMi.Records;

public class Session : RedirectProcess
{
    private readonly ConcurrentQueue<string> responseBuffer = new ();

    private readonly ConcurrentDictionary<int, CommandData<Record>> sessionCommands = new ();

    private int commandToken;

    public Session(ProcessStartInfo startInfo)
        : base(startInfo)
    {
    }

    public Session(Process process)
        : base(process)
    {
    }

    public async Task<CommandData<Record>> SendCommandAsync(string command, Action<Record> @delegate, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentException($"'{nameof(command)}' cannot be null or whitespace.", nameof(command));
        }

        var userToken = Regex.Match(command, @"^\d+", RegexOptions.IgnoreCase);

        var tcs = new TaskCompletionSource<Record>();

        command = userToken.Success ? command[(userToken.Index + userToken.Length) ..] : command;

        if (string.IsNullOrEmpty(command))
        {
            throw new ArgumentException($"{nameof(command)} is unexpectedly short", nameof(command));
        }
        else if (command[0] != '-')
        {
            command = $"-interpreter-exec console \"{command}\"";
        }

        var commandData = new CommandData<Record>
        {
            Token = userToken.Success ? int.Parse(userToken.Value, NumberStyles.None, CultureInfo.InvariantCulture) : ++commandToken,

            Command = command,

            CancellationToken = cancellationToken,

            CompletionSource = tcs,

            ResultDelegate = @delegate,
        };

        if (!sessionCommands.TryAdd(commandData.Token, commandData))
        {
            throw new InvalidOperationException("failed adding tracked command data.");
        }

        await SendCommandAsync(commandData.Command, cancellationToken).ConfigureAwait(false);

        return commandData;
    }

    internal void ProcessStdout(string stdout)
    {
        if (string.IsNullOrWhiteSpace(stdout))
        {
            return;
        }

        var record = Interpreter.ParseOutput(stdout);

        switch (record)
        {
            case var _ when record is PromptRecord:
                break;
            case var _ when record is AsyncRecord:
                break;
            case var _ when record is StreamRecord:
                break;
            case var _ when record is ResultRecord:
                break;
        }

        if (record is ResultRecord resultRecord && sessionCommands.TryRemove(resultRecord.Token, out var commandData))
        {
            try
            {
                commandData.CancellationToken.ThrowIfCancellationRequested();

                commandData.ResultDelegate?.Invoke(resultRecord);

                commandData.CompletionSource?.SetResult(resultRecord);
            }
            catch (Exception e)
            {
                commandData.CompletionSource?.SetException(e);
            }
        }

        //responseBuffer.Enqueue(stdout);
    }

    internal void ProcessStderr(string stderr)
    {
        if (string.IsNullOrWhiteSpace(stderr))
        {
            return;
        }
    }

    internal void ProcessExited()
    {
    }
}
