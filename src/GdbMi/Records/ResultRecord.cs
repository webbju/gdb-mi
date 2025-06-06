namespace GdbMi.Records;

using System;
using System.Collections.Generic;
using GdbMi.Values;

/// <summary>
/// <c>ResultRecord</c> represents result GDB/MI responses.
/// </summary>
/// <remarks><see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Result-Records.html#GDB_002fMI-Result-Records"/>.</remarks>
public class ResultRecord : Record
{
    /// <summary>
    /// Result record prefix.
    /// </summary>
    public const char ResultPrefix = '^';

    /// <summary>
    /// Constructs <c>ResultRecord</c> with specified values.
    /// </summary>
    /// <param name="token">Record token.</param>
    /// <param name="class">Record class.</param>
    /// <param name="values">Values used to initialise underlying <seealso cref="TupleValue"/> collection.</param>
    /// <exception cref="ArgumentException"><paramref name="class"/> must not be null or empty.</exception>
    public ResultRecord(int token, string @class, IList<Value> values)
        : base(values)
    {
        if (string.IsNullOrWhiteSpace(@class))
        {
            throw new ArgumentException($"'{nameof(@class)}' cannot be null or whitespace.", nameof(@class));
        }

        Token = token;

        Class = @class;
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    internal ResultRecord()
        : base()
    {
    }

    /// <summary>
    /// Gets the result record token.
    /// </summary>
    /// <remarks>The token is any sequence of digits specified by the user when issuing the command.</remarks>
    public int Token { get; init; }

    /// <summary>
    /// Gets the result record class.
    /// </summary>
    /// <remarks>The class is one of: <c>"done" | "running" | "connected" | "error" | "exit"</c>.</remarks>
    public string Class { get; init; }
}
