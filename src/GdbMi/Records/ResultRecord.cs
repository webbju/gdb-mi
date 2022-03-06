namespace GdbMi.Records
{
    using System;
    using System.Collections.Generic;
    using GdbMi.Values;

    public class ResultRecord : TupleValue, IRecord
    {
        public const char ResultPrefix = '^';

        public ResultRecord(uint token, string @class, IEnumerable<Value> values)
            : base(values)
        {
            if (string.IsNullOrWhiteSpace(@class))
            {
                throw new ArgumentException($"'{nameof(@class)}' cannot be null or whitespace.", nameof(@class));
            }

            Token = token;

            Class = @class;
        }

        public uint Token { get; init; }

        public string Class { get; init; }
    }
}
