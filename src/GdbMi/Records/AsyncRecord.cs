namespace GdbMi.Records;

using System;
using System.Collections.Generic;
using GdbMi.Values;

/// <summary>
/// <c>AsyncRecord</c> represents async output GDB/MI responses.
/// </summary>
/// <remarks>
/// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>
/// </remarks>
public class AsyncRecord : Record
{
    /// <summary>
    /// <seealso cref="AsyncType.Exec"/> async record prefix.
    /// </summary>
    public const char ExecPrefix = '*';

    /// <summary>
    /// <seealso cref="AsyncType.Status"/> async record prefix.
    /// </summary>
    public const char StatusPrefix = '+';

    /// <summary>
    /// <seealso cref="AsyncType.Notify"/> async record prefix.
    /// </summary>
    public const char NotifyPrefix = '=';

    /// <summary>
    /// Constructs <c>AsyncRecord</c> with specified parameters.
    /// </summary>
    /// <param name="type">Record type.</param>
    /// <param name="token">Record token.</param>
    /// <param name="class">Record class.</param>
    /// <param name="values">Values used to initialise underlying <seealso cref="TupleValue"/> collection.</param>
    /// <exception cref="ArgumentException"><paramref name="class"/> must not be null or empty.</exception>
    public AsyncRecord(AsyncType type, int token, string @class, IList<Value> values)
        : base(values)
    {
        if (string.IsNullOrEmpty(@class))
        {
            throw new ArgumentException($"'{nameof(@class)}' cannot be null or empty.", nameof(@class));
        }

        Type = type;

        Token = token;

        Class = @class;
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    internal AsyncRecord()
        : base()
    {
    }

    /// <summary>
    /// <c>AsyncType</c> represents the various async output types.
    /// </summary>
    public enum AsyncType
    {
        /// <summary>
        /// <c>Exec</c> async output contains asynchronous state change on the target (stopped, started, disappeared). All async output is prefixed by <c>'*'</c>.
        /// </summary>
        Exec,

        /// <summary>
        /// <c>Status</c> async output contains on-going status information about the progress of a slow operation. It can be discarded. All status output is prefixed by <c>'+'</c>.
        /// </summary>
        Status,

        /// <summary>
        /// <c>Notify</c> async output contains supplementary information that the client should handle (e.g., a new breakpoint information). All notify output is prefixed by <c>'='</c>.
        /// </summary>
        Notify,
    }

    /// <summary>
    /// Gets the async record type.
    /// </summary>
    public AsyncType Type { get; init; }

    /// <summary>
    /// Gets the async record token.
    /// </summary>
    public int Token { get; init; }

    /// <summary>
    /// Gets the async record class.
    /// </summary>
    /// <remarks><c>"stopped" | others</c> (where others will be added depending on the needs—this is still in development).</remarks>
    public string Class { get; init; }
}
