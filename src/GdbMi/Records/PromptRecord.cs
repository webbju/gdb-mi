namespace GdbMi.Records;

/// <summary>
/// <c>PromptRecord</c> represents the GDB/MI response when idle and/or waiting for user input.
/// </summary>
public class PromptRecord : Record
{
    /// <summary>
    /// GDB/MI user prompt.
    /// </summary>
    public const string Prompt = "(gdb)";

    /// <summary>
    /// Default constructor.
    /// </summary>
    public PromptRecord()
    {
    }
}
