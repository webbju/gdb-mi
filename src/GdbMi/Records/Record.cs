namespace GdbMi.Records
{
    using System.Collections.Generic;
    using GdbMi.Values;

    public abstract class Record : TupleValue
    {
        protected Record()
        {
        }

        protected Record(IEnumerable<Value> values)
            : base(values)
        {
        }

        public bool IsOutOfBand => this is AsyncRecord || this is StreamRecord;
    }
}
