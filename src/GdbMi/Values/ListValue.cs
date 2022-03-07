namespace GdbMi.Values
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    // Collection of values or results.
    public class ListValue : Value, IEquatable<ListValue>
    {
        // Despite naming. ReadOnlyCollection implements IReadOnlyList.
        private readonly ReadOnlyCollection<Value> values;

        public ListValue(IList<Value> values)
        {
            this.values = new ReadOnlyCollection<Value>(values);
        }

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

        public override int Count
        {
            get => values.Count;
        }

        public override IEnumerable<Value> GetValues()
        {
            return values;
        }

        public override bool Equals(object obj)
        {
            return obj is ListValue other && Equals(other);
        }

        public bool Equals(ListValue other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Enumerable.SequenceEqual(values, other.values);
        }

        public override string ToString()
        {
            return $"[{string.Join(",", values.Select(r => $"{r}"))}]";
        }
    }
}
