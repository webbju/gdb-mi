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

    private ProcessStartInfo startInfo;

    private TextWriter standardInputWriter = TextWriter.Null;

    public RedirectProcess(ProcessStartInfo startInfo)
    {
        this.startInfo = startInfo;
    }

    public RedirectProcess(Process process)
    {
        this.process = process;

        this.startInfo = process?.StartInfo;
    }

    public Process Process => process;

    public ProcessStartInfo StartInfo => startInfo;

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            process?.Dispose();
            process = null;

            standardInputWriter?.Dispose();
            standardInputWriter = null;
        }
    }

    public static ProcessStartInfo CreateDefaultStartInfo(string filename, string arguments, string workingDirectory = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = filename ?? throw new ArgumentNullException(nameof(filename)),

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
        process = new Process
        {
            StartInfo = startInfo,

            EnableRaisingEvents = true,
        };

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
            throw new InvalidOperationException($"Could start process: {process.StartInfo.FileName}");
        }

        process.BeginOutputReadLine();

        process.BeginErrorReadLine();

        standardInputWriter = TextWriter.Synchronized(process.StandardInput);
    }

    public async Task SendCommandAsync(string command, CancellationToken cancellationToken)
    {
        Log.Debug($"[{nameof(RedirectProcess)}] SendCommand: {command}");

        if (string.IsNullOrWhiteSpace(command))
        {
            throw new ArgumentNullException(nameof(command));
        }

        await standardInputWriter.WriteLineAsync(command.AsMemory(), cancellationToken).ConfigureAwait(false);
    }

    protected virtual void ProcessStdout(object sender, DataReceivedEventArgs args)
    {
        lastOutputTimestamp = Environment.TickCount;

        Log.Debug($"[{nameof(RedirectProcess)}] ProcessStdout: {args.Data}");
    }

    protected virtual void ProcessStderr(object sender, DataReceivedEventArgs args)
    {
        lastOutputTimestamp = Environment.TickCount;

        Log.Debug($"[{nameof(RedirectProcess)}] ProcessStderr: {args.Data}");
    }

    protected virtual void ProcessExited(object sender, EventArgs args)
    {
        if (sender is Process p)
        {
            Log.Debug($"[{nameof(RedirectProcess)}] {p.StartInfo.FileName} exited ({p.ExitCode}) in {Environment.TickCount - startTimestamp} ms");
        }
    }
}
