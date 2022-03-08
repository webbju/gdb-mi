namespace GdbMi.Values
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// <c>ResultValue</c> represents a named <c>Value</c>.
    /// </summary>
    /// <remarks><see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>.</remarks>
    public class ResultValue : Value, IEquatable<ResultValue>
    {
        /// <summary>
        /// Construct <c>ResultValue</c> with specified name and value.
        /// </summary>
        /// <param name="variable">Name of the value.</param>
        /// <param name="value">Named value.</param>
        /// <exception cref="ArgumentException"><paramref name="variable"/> is not a valid <seealso cref="string"/> object, is empty, or is whitespace.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="variable"/>is not a valid object.</exception>
        public ResultValue(string variable, Value value)
        {
            if (string.IsNullOrWhiteSpace(variable))
            {
                throw new ArgumentException($"'{nameof(variable)}' cannot be null or whitespace.", nameof(variable));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            Variable = variable;

            Value = value;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected ResultValue()
        {
        }

        /// <summary>
        /// Gets variable of the result.
        /// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>.
        /// </summary>
        public string Variable { get; }

        /// <summary>
        /// Gets the value of the result.
        /// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>.
        /// </summary>
        public Value Value { get; }

        /// <inheritdoc/>
        public override int Count
        {
            get => Value.Count;
        }

        /// <inheritdoc/>
        public override Value this[string key]
        {
            get
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return Value[key];
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

                return Value[index];
            }
        }

        /// <inheritdoc/>
        public override bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
        {
            return Value.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ResultValue other && Equals(other);
        }

        /// <inheritdoc/>
        public bool Equals(ResultValue other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Equals(Variable, other.Variable) && Equals(Value, other.Value);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Variable, Value);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Variable}={Value}";
        }
    }
}
