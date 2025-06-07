namespace GdbMi;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

public abstract class RedirectProcess : IDisposable
{
    private int startTimestamp;

    private int lastOutputTimestamp;

    private Process process;

    private TextWriter standardInputWriter = TextWriter.Null;

    public RedirectProcess(ProcessStartInfo startInfo)
        : this(new Process()
        {
            StartInfo = startInfo ?? throw new ArgumentNullException(nameof(startInfo)),
            EnableRaisingEvents = true,
        })
    {
    }

    public RedirectProcess(Process process)
    {
        ArgumentNullException.ThrowIfNull(process);

        this.process = process;
    }

    public Process Process => process;

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            process.Dispose();

            standardInputWriter.Dispose();
        }
    }

    public static ProcessStartInfo CreateDefaultStartInfo(string filename, string arguments, string workingDirectory = null)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(filename);

        var startInfo = new ProcessStartInfo
        {
            FileName = filename,

            Arguments = arguments,

            WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(filename),

            CreateNoWindow = true,

            UseShellExecute = false,

            ErrorDialog = false,

            RedirectStandardOutput = true,

            RedirectStandardError = true,

            RedirectStandardInput = true,
        };

        return startInfo;
    }

    public void Start()
    {
        if (process.StartInfo.RedirectStandardOutput)
        {
            process.OutputDataReceived += ProcessStdout;
        }

        if (process.StartInfo.RedirectStandardError)
        {
            process.ErrorDataReceived += ProcessStderr;
        }

        if (process.EnableRaisingEvents)
        {
            process.Exited += ProcessExited;
        }

        Log.Debug($"[{nameof(RedirectProcess)}] Start: {process.StartInfo.FileName} (Args=\"{process.StartInfo.Arguments}\" Pwd=\"{process.StartInfo.WorkingDirectory}\")");

        startTimestamp = Environment.TickCount;

        lastOutputTimestamp = startTimestamp;

        if (!process.Start())
        {
            throw new InvalidOperationException($"Could not start process: {process.StartInfo.FileName}");
        }

        process.BeginOutputReadLine();

        process.BeginErrorReadLine();

        standardInputWriter = TextWriter.Synchronized(process.StandardInput);
    }

    public async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(command);

        Log.Debug($"[{nameof(RedirectProcess)}] SendCommand: {command}");

        await standardInputWriter.WriteLineAsync(command.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    private void ProcessStdout(object sender, DataReceivedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        lastOutputTimestamp = Environment.TickCount;

        Log.Debug($"[{nameof(RedirectProcess)}] ProcessStdout: {args.Data}");

        HandleStdout(args.Data);
    }

    internal virtual void HandleStdout(string stdout)
    {
        // Default implementation does nothing
    }

    private void ProcessStderr(object sender, DataReceivedEventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        lastOutputTimestamp = Environment.TickCount;

        Log.Debug($"[{nameof(RedirectProcess)}] ProcessStderr: {args.Data}");
    }

    internal virtual void HandleStderr(string stderr)
    {
        // Default implementation does nothing
    }

    private void ProcessExited(object sender, EventArgs args)
    {
        ArgumentNullException.ThrowIfNull(args);

        if (sender is Process p)
        {
            Log.Debug($"[{nameof(RedirectProcess)}] {p.StartInfo.FileName} exited ({p.ExitCode}) in {Environment.TickCount - startTimestamp} ms");
        }
    }

    internal virtual void HandleExit(int exitCode)
    {
        // Default implementation does nothing
    }
}
