﻿namespace GdbMi.Values;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

/// <summary>
/// <c>TupleValue</c> is a keyed collection of <c>ResultValue</c> objects.
/// </summary>
/// <remarks>
/// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>
/// </remarks>
public class TupleValue : Value, IEquatable<TupleValue>
{
    private readonly ReadOnlyDictionary<string, ResultValue> values;

    private readonly IReadOnlyList<string> order;

    /// <summary>
    /// Constructs <c>TupleValue</c> with specified values.
    /// </summary>
    /// <param name="values">Values used to initialise underlying collection.</param>
    /// <exception cref="ArgumentNullException"><paramref name="values"/> does not reference a valid object.</exception>
    /// <exception cref="ArgumentException"><paramref name="values"/> contains entries which are not of type <seealso cref="ResultValue"/>.</exception>
    public TupleValue(IList<Value> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (!values.All(x => x is ResultValue))
        {
            throw new ArgumentException($"'{nameof(values)}' contains non {typeof(ResultValue)} entries.");
        }

        this.order = values.Select(x => (x as ResultValue).Variable).ToList();

        this.values = new ReadOnlyDictionary<string, ResultValue>(values.ToDictionary(x => (x as ResultValue).Variable, x => x as ResultValue));
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected TupleValue()
    {
        values = new ReadOnlyDictionary<string, ResultValue>(new Dictionary<string, ResultValue>());

        order = [];
    }

    /// <summary>
    /// Constructor which clones an existing <c>TupleValue</c>.
    /// </summary>
    /// <remarks>This constructor reuses existing collections. It does not deep copy.</remarks>
    /// <param name="tuple"><c>TupleValue</c> to clone.</param>
    protected TupleValue(TupleValue tuple)
    {
        ArgumentNullException.ThrowIfNull(tuple);

        order = tuple.order;

        values = tuple.values;
    }

    /// <inheritdoc/>
    public override int Count
    {
        get => order.Count;
    }

    /// <inheritdoc/>
    public override Value this[string key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);

            return values[key].Value;
        }
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

            return values[order[index]].Value;
        }
    }

    /// <inheritdoc/>
    public override bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (values.TryGetValue(key, out ResultValue result))
        {
            value = result.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        return obj is TupleValue other && Equals(other);
    }

    /// <inheritdoc/>
    public bool Equals(TupleValue other)
    {
        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return other != null && Enumerable.SequenceEqual(order, other.order);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        return HashCode.Combine(values, order);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var builder = new StringBuilder();

        builder.Append('{');

        foreach (int i in Enumerable.Range(0, order.Count))
        {
            if (i > 0)
            {
                builder.Append(',');
            }

            builder.Append($"{order[i]}={this[order[i]]}");
        }

        builder.Append('}');

        return builder.ToString();
    }
}
