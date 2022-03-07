namespace GdbMi.Values
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    // Keyed collection of results.
    public class TupleValue : Value, IEquatable<TupleValue>
    {
        private readonly ReadOnlyDictionary<string, Value> values;

        private readonly IReadOnlyList<string> order;

        protected TupleValue()
        {
            values = new ReadOnlyDictionary<string, Value>(new Dictionary<string, Value>());

            order = Array.Empty<string>().ToList();
        }

        public TupleValue(IEnumerable<Value> values)
        {
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            if (values.Where(x => x is not ResultValue).Any())
            {
                throw new ArgumentException($"'{nameof(values)}' contains non {typeof(ResultValue)} entries.");
            }

            this.order = values.Select(x => (x as ResultValue).Variable).ToList();

            this.values = new ReadOnlyDictionary<string, Value>(values.Where(x => x is ResultValue).ToDictionary(x => (x as ResultValue).Variable, x => (x as ResultValue).Value));
        }

        public override Value this[string key]
        {
            get
            {
                if (key is null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                return values[key];
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

                return values[order[index]];
            }
        }

        public override int Count => order.Count;

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
        {
            return values.TryGetValue(key, out value);
        }

        public override IEnumerable<Value> GetValues()
        {
            foreach (var key in order)
            {
                yield return this[key];
            }
        }

        public override IEnumerable<KeyValuePair<string, Value>> GetKeyValues()
        {
            foreach (var key in order)
            {
                yield return new KeyValuePair<string, Value>(key, this[key]);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is TupleValue other && Equals(other);
        }

        public bool Equals(TupleValue other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other != null && Enumerable.SequenceEqual(order, other.order);
        }

        public override string ToString()
        {
            var valueStr = string.Join(",", GetKeyValues().Select(v => $"{v.Key}={v.Value}"));

            return $"{{{valueStr}}}";
        }
    }
}
