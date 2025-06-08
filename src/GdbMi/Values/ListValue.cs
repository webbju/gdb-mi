namespace GdbMi.Values;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

/// <summary>
/// <c>ListValue</c> is an indexed collection of <c>Value</c> or <c>ResultValue</c> objects.
/// </summary>
/// <remarks>
/// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>
/// </remarks>
public class ListValue : Value, IEquatable<ListValue>
{
    /// <remarks>Despite naming. ReadOnlyCollection implements IReadOnlyList.</remarks>
    private readonly ReadOnlyCollection<Value> values;

    /// <summary>
    /// Constructs <c>ListValue</c> with specified values.
    /// </summary>
    /// <param name="values">Values used to initialise underlying collection.</param>
    public ListValue(IList<Value> values)
    {
        this.values = new ReadOnlyCollection<Value>(values);
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected ListValue()
    {
        values = new ReadOnlyCollection<Value>(Array.Empty<Value>());
    }

    /// <inheritdoc/>
    public override int Count
    {
        get => values.Count;
    }

    /// <inheritdoc/>
    public override Value this[int index]
    {
        get
        {
            if (index < 0 || index >= Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return values[index];
        }
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is ListValue other && Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(ListValue other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other != null && Enumerable.SequenceEqual(values, other.values);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(values);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{string.Join(",", values.Select(r => $"{r}"))}]";
    }
}
