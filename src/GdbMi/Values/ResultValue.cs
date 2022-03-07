namespace GdbMi.Values
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    // Result is a key-value pair of variable and child value.
    public class ResultValue : Value, IEquatable<ResultValue>
    {
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

        public override int Count => Value.Count;

        public string Variable { get; }

        public Value Value { get; }

        public override bool Equals(object obj)
        {
            return obj is ResultValue other && Equals(other);
        }

        public bool Equals(ResultValue other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Equals(Variable, other.Variable) && Equals(Value, other.Value);
        }

        public override string ToString()
        {
            return $"{Variable}={Value}";
        }

        public override bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
        {
            return Value.TryGetValue(key, out value);
        }

        public override IEnumerable<KeyValuePair<string, Value>> GetKeyValues()
        {
            return Value.GetKeyValues();
        }

        public override IEnumerable<Value> GetValues()
        {
            return Value.GetValues();
        }
    }
}
