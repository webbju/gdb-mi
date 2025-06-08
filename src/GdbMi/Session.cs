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

    /// <summary>
    /// Default constructor.
    /// </summary>
    internal Session(Process process)
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

    private void ProcessStdout(object sender, DataReceivedEventArgs args)
    {
        HandleStdout(args.Data);
    }

    /// <inheritdoc/>
    internal override void HandleStdout(string stdout)
    {
        if (string.IsNullOrWhiteSpace(stdout))
        {
            return;
        }

        var record = Interpreter.ParseOutput(stdout);

        switch (record)
        {
            case PromptRecord:
                break;
            case AsyncRecord:
                break;
            case StreamRecord:
                break;
            case ResultRecord:
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
}
