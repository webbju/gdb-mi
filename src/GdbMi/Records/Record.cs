namespace GdbMi.Records
{
    using System.Collections.Generic;
    using GdbMi.Values;

    /// <summary>
    /// <c>Record</c> provides an abstract base for all GDB/MI responses.
    /// </summary>
    public abstract class Record : TupleValue
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Record()
        {
        }

        /// <summary>
        /// Clone constructor.
        /// </summary>
        /// <param name="other"><c>Record</c> to clone.</param>
        protected Record(Record other)
            : base(other)
        {
        }

        /// <summary>
        /// Constructs <c>Record</c> with specified values.
        /// </summary>
        /// <param name="values">Values used to initialise underlying <seealso cref="TupleValue"/> collection.</param>
        protected Record(IList<Value> values)
            : base(values)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this object represents an out-of-bound record.
        /// </summary>
        /// <remarks><see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Output-Syntax.html#GDB_002fMI-Output-Syntax"/>.</remarks>
        public bool IsOutOfBand => this is AsyncRecord || this is StreamRecord;
    }
}
