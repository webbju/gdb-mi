namespace GdbMi.Values
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// <c>Value</c> provides an abstract base for all GDB/MI values.
    /// </summary>
    public abstract class Value
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Value()
        {
        }

        /// <summary>
        /// Gets the number of elements represented by <c>Value</c> object.
        /// </summary>
        public virtual int Count
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the <c>Value</c> element at the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        public virtual Value this[string key]
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the <c>Value</c> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        public virtual Value this[int index]
        {
            get => throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the object contains an element with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>true if the <c>Value</c> contains an element with the key; otherwise, false.</returns>
        public bool ContainsKey(string key)
        {
            if (key is null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            return TryGetValue(key, out _);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>true if the <c>Value</c> contains an element with the specified key; otherwise, false.</returns>
        public virtual bool TryGetValue(string key, [MaybeNullWhen(false)] out Value value)
        {
            throw new NotImplementedException();
        }
    }
}
