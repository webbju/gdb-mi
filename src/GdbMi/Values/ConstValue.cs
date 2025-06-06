namespace GdbMi.Values;

using System;

/// <summary>
/// <c>ConstValue</c> is represents a single (constant) <c>c-string</c> value.
/// </summary>
/// <remarks><see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>.</remarks>
public class ConstValue : Value, IEquatable<ConstValue>
{
    /// <summary>
    /// Constructs <c>ConstValue</c> with specified <c>c-string</c>.
    /// </summary>
    /// <param name="value">c-string value.</param>
    public ConstValue(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected ConstValue()
    {
    }

    /// <summary>
    /// Gets the <c>c-string</c> value represented by this object.
    /// </summary>
    public string Value { get; }

    /// <inheritdoc/>
    public override int Count
    {
        get => 1;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is ConstValue other && Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(ConstValue other)
    {
        return Equals(Value, other?.Value);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"\"{Value}\"";
    }
}
