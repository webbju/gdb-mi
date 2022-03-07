namespace GdbMi.Records
{
    using System;
    using System.Collections.Generic;
    using GdbMi.Values;

    // exec-async-output contains asynchronous state change on the target(stopped, started, disappeared). All async output is prefixed by ‘*’.
    // status-async-output contains on-going status information about the progress of a slow operation. It can be discarded. All status output is prefixed by ‘+’.
    // notify-async-output contains supplementary information that the client should handle(e.g., a new breakpoint information). All notify output is prefixed by ‘=’.
    // https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax
    public class AsyncRecord : TupleValue, IOutOfBandRecord
    {
        public const char ExecPrefix = '*';

        public const char StatusPrefix = '+';

        public const char NotifyPrefix = '=';

        public enum AsyncType
        {
            Exec,   // [ token ] "*" async-output nl
            Status, // [ token ] "+" async-output nl
            Notify, // [ token ] "=" async-output nl
        }

        public AsyncRecord(AsyncType type, uint token, string @class, IEnumerable<Value> values)
            : base(values)
        {
            if (string.IsNullOrEmpty(@class))
            {
                throw new ArgumentException($"'{nameof(@class)}' cannot be null or empty.", nameof(@class));
            }

            Type = type;

            Token = token;

            Class = @class;
        }

        public AsyncType Type { get; init; }

        public uint Token { get; init; }

        public string Class { get; init; }
    }
}
