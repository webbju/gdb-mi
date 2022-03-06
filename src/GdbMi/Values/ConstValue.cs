namespace GdbMi.Values
{
    using System;

    public class ConstValue : Value, IEquatable<ConstValue>
    {
        public ConstValue(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }

        public override bool Equals(object obj)
        {
            return obj is ConstValue other && Equals(other);
        }

        public bool Equals(ConstValue other)
        {
            return Equals(Value, other?.Value);
        }

        public override string ToString()
        {
            return $"\"{Value}\"";
        }
    }
}
