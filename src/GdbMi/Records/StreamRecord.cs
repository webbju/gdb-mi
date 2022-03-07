namespace GdbMi.Records
{
    using System;

    public class StreamRecord : Record
    {
        public const char ConsolePrefix = '~';

        public const char TargetPrefix = '@';

        public const char LogPrefix = '&';

        public enum StreamType
        {
            Console, // "~" c-string nl
            Target,  // "@" c-string nl
            Log,     // "&" c-string nl
        }

        public StreamRecord(StreamType streamType, string stream)
        {
            if (string.IsNullOrEmpty(stream))
            {
                throw new ArgumentException($"'{nameof(stream)}' cannot be null or empty.", nameof(stream));
            }

            Type = streamType;

            Stream = stream;
        }

        public StreamType Type { get; init; }

        public string Stream { get; init; }
    }
}
