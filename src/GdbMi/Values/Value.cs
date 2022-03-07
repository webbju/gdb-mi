namespace GdbMi.Values
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public abstract class Value : IEnumerable<Value>, IEnumerable<KeyValuePair<string, Value>>, IEnumerable
    {
        protected Value()
        {
        }

        public virtual int Count
        {
            get => throw new NotImplementedException();
        }

        public virtual Value this[string key]
        {
            get => throw new NotImplementedException();
        }

        public virtual Value this[int index]
        {
            get => throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return TryGetValue(key, out _);
        }

        public virtual bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<Value> GetValues()
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<KeyValuePair<string, Value>> GetKeyValues()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Value> GetEnumerator()
        {
            return GetValues().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, Value>> IEnumerable<KeyValuePair<string, Value>>.GetEnumerator()
        {
            return GetKeyValues().GetEnumerator();
        }
    }
}
