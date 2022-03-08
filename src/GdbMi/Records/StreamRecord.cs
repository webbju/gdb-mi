namespace GdbMi.Records
{
    using System;

    /// <summary>
    /// <c>StreamRecord</c> represents output stream GDB/MI responses.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     GDB internally maintains a number of output streams: the console, the target, and the log.
    ///     The output intended for each of these streams is funneled through the GDB/MI interface using stream records.
    /// </para>
    /// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Stream-Records.html#GDB_002fMI-Stream-Records"/>.
    /// </remarks>
    public class StreamRecord : Record
    {
        /// <summary>
        /// <seealso cref="StreamType.Console"/> output stream prefix.
        /// </summary>
        public const char ConsolePrefix = '~';

        /// <summary>
        /// <seealso cref="StreamType.Target"/> output stream prefix.
        /// </summary>
        public const char TargetPrefix = '@';

        /// <summary>
        /// <seealso cref="StreamType.Log"/> output stream prefix.
        /// </summary>
        public const char LogPrefix = '&';

        /// <summary>
        /// Constructs <c>StreamRecord</c> with specified values.
        /// </summary>
        /// <param name="type">Type of the output stream.</param>
        /// <param name="content">Contents of the output stream.</param>
        /// <exception cref="ArgumentException"><paramref name="content"/> must not be null or empty.</exception>
        internal StreamRecord(StreamType type, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new ArgumentException($"'{nameof(content)}' cannot be null or empty.", nameof(content));
            }

            Type = type;

            Content = content;
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal StreamRecord()
            : base()
        {
        }

        /// <summary>
        /// <c>StreamType</c> represents the various output stream types.
        /// </summary>
        public enum StreamType
        {
            /// <summary>
            /// <c>Console</c> stream contains text that should be displayed in the CLI console window. It contains the textual responses to CLI commands.
            /// </summary>
            Console,

            /// <summary>
            /// <c>Target</c> stream contains any textual output from the running target. This is only present when GDB’s event loop is truly asynchronous, which is currently only the case for remote targets.
            /// </summary>
            Target,

            /// <summary>
            /// <c>Log</c> stream contains debugging messages being produced by GDB’s internals.
            /// </summary>
            Log,
        }

        /// <summary>
        /// Gets the stream record type.
        /// </summary>
        public StreamType Type { get; init; }

        /// <summary>
        /// Gets the content of the stream record.
        /// </summary>
        /// <remarks>The content of the stream record is everything following the <seealso cref="StreamType"/> prefix.</remarks>
        public string Content { get; init; }
    }
}
