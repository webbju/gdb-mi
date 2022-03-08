namespace GdbMi.Objects
{
    using System;
    using GdbMi.Values;

    /// <summary>
    /// <c>Breakpoint</c> manages reported information about a breakpoint, a tracepoint, a watchpoint, or a catchpoint.
    /// </summary>
    /// <remarks>
    /// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Breakpoint-Information.html#GDB_002fMI-Breakpoint-Information"></see>.
    /// </remarks>
    public class Breakpoint : TupleValue
    {
        /// <summary>
        /// Constructs <c>Breakpoint</c> from specified <c>Value</c>.
        /// </summary>
        /// <param name="value">Base <c>Value</c> object.</param>
        public Breakpoint(Value value)
            : base(ValidateTuple(value))
        {
            SeedProperties();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Breakpoint()
            : base()
        {
        }

        /// <summary>
        /// Gets the breakpoint number.
        /// </summary>
        public int Number { get; private set; }

        /// <summary>
        /// Gets the type of the breakpoint.
        /// </summary>
        /// <remarks>For ordinary breakpoints this will be <c>"breakpoint"</c>, but many values are possible.</remarks>
        public string Type { get; private set; }

        /// <summary>
        /// Gets the type of catchpoint.
        /// </summary>
        /// <remarks>If the <c>Type</c> of the breakpoint is <c>"catchpoint"</c>, then this indicates the exact type of catchpoint.</remarks>
        public string CatchpointType { get; private set; }

        /// <summary>
        /// Gets the disposition of the breakpoint.
        /// </summary>
        /// <remarks>Either <c>"del"</c>, meaning that the breakpoint will be deleted at the next stop, or <c>"keep"</c>, meaning that the breakpoint will not be deleted.</remarks>
        public string Disposition { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the breakpoint is enabled, in which case the value is <c>"y"</c>, or disabled, in which case the value is <c>"n"</c>.
        /// </summary>
        /// <remarks>Note that this is not the same as the field "enable".</remarks>
        public string Enabled { get; private set; }

        /// <summary>
        /// Gets the address of the breakpoint.
        /// </summary>
        /// <remarks>
        /// This may be a hexidecimal number, giving the address; or the string <c>&lt;PENDING&gt;</c>, for a pending breakpoint; or the string <c>&lt;MULTIPLE&gt;</c>, for a breakpoint with multiple locations.
        /// This field will not be present if no address can be determined. For example, a watchpoint does not have an address.
        /// </remarks>
        public string Address { get; private set; }

        /// <summary>
        /// Gets any optional flags related to the address.
        /// </summary>
        /// <remarks>These flags are architecture-dependent; <see href="https://sourceware.org/gdb/onlinedocs/gdb/Architectures.html#Architectures"/> for their meaning for a particular CPU.</remarks>
        public string AddressFlags { get; private set; }

        /// <summary>
        /// Gets the function in which the breakpoint appears.
        /// </summary>
        /// <remarks>If not known, this field is not present.</remarks>
        public string Function { get; private set; }

        /// <summary>
        /// Gets the name of the source file which contains this function.
        /// </summary>
        /// <remarks>If not known, this field is not present.</remarks>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the full file name of the source file which contains this function.
        /// </summary>
        /// <remarks>If not known, this field is not present.</remarks>
        public string Fullname { get; private set; }

        /// <summary>
        /// Gets the line number at which the breakpoint appears.
        /// </summary>
        /// <remarks>If not known, this field is not present.</remarks>
        public int Line { get; private set; }

        /// <summary>
        /// Gets the address of the breakpoint, possibly followed by a symbol name.
        /// </summary>
        /// <remarks>If the source file is not known, this field may be provided.</remarks>
        public string At { get; private set; }

        /// <summary>
        /// Gets the breakpoint's pending expression.
        /// </summary>
        /// <remarks>If the breakpoint is pending, this field is present and holds the text used to set the breakpoint, as entered by the user.</remarks>
        public string Pending { get; private set; }

        /// <summary>
        /// Gets where the breakpoint's condition is evaluated, either <c>"host"</c> or <c>"target"</c>.
        /// </summary>
        public string EvaluatedBy { get; private set; }

        /// <summary>
        /// Gets the thread on which the breakpoint can trigger.
        /// </summary>
        /// <remarks>If this is a thread-specific breakpoint, then this identifies the thread in which the breakpoint can trigger.</remarks>
        public string Thread { get; private set; }

        /// <summary>
        /// Gets the thread groups this location is in.
        /// </summary>
        public ListValue ThreadGroups { get; private set; }

        /// <summary>
        /// Gets the Ada task identifier.
        /// </summary>
        /// <remarks>If the breakpoint is not restricted to a particular Ada task, then this field is not present.</remarks>
        public string Task { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the breakpoint is conditional.
        /// </summary>
        /// <remarks>This is the condition expression.</remarks>
        public string Condition { get; private set; }

        /// <summary>
        /// Gets the ignore count of the breakpoint.
        /// </summary>
        public int IgnoreCount { get; private set; }

        /// <summary>
        /// Gets the enable count of the breakpoint.
        /// </summary>
        public int EnableCount { get; private set; }

        /// <summary>
        /// Gets a tracepoint's pass count.
        /// </summary>
        public int PassCount { get; private set; }

        /// <summary>
        /// Gets the number of times the breakpoint has been hit.
        /// </summary>
        public int HitCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the tracepoint is installed. This is either <c>"y"</c>, meaning that the tracepoint is installed, or <c>"n"</c>, meaning that it is not.
        /// </summary>
        /// <remarks>
        /// This field is only given for tracepoints.
        /// </remarks>
        public string Installed { get; private set; }

        /// <summary>
        /// Gets the locations of the breakpoint.
        /// </summary>
        /// <remarks>
        /// This field is present if the breakpoint has multiple locations.
        /// It is also exceptionally present if the breakpoint is enabled and has a single, disabled location.
        /// </remarks>
        public ListValue Locations { get; private set; }

        /// <summary>
        /// Seeds property values based on underlying tuple contents.
        /// </summary>
        protected void SeedProperties()
        {
            if (TryGetValue("number", out Value numberValue) && numberValue.ConvertValue(out int number))
            {
                Number = number;
            }

            if (TryGetValue("type", out Value typeValue) && typeValue.ConvertValue(out string type))
            {
                Type = type;
            }

            if (TryGetValue("catch-type", out Value catchTypeValue) && catchTypeValue.ConvertValue(out string catchType))
            {
                CatchpointType = catchType;
            }

            if (TryGetValue("disp", out Value dispValue) && dispValue.ConvertValue(out string disp))
            {
                Disposition = disp;
            }

            if (TryGetValue("enabled", out Value enabledValue) && enabledValue.ConvertValue(out string enabled))
            {
                Enabled = enabled;
            }

            if (TryGetValue("addr", out Value addrValue) && addrValue.ConvertValue(out string addr))
            {
                Address = addr;
            }

            if (TryGetValue("addr_flags", out Value addrFlagsValue) && addrFlagsValue.ConvertValue(out string addrFlags))
            {
                AddressFlags = addrFlags;
            }

            if (TryGetValue("func", out Value funcValue) && funcValue.ConvertValue(out string func))
            {
                Function = func;
            }

            if (TryGetValue("file", out Value fileValue) && fileValue.ConvertValue(out string file))
            {
                Filename = file;
            }
            else if (TryGetValue("filename", out Value filenameValue) && filenameValue.ConvertValue(out string filename))
            {
                Filename = filename;
            }

            if (TryGetValue("fullname", out Value fullnameValue) && fullnameValue.ConvertValue(out string fullname))
            {
                Fullname = fullname;
            }

            if (TryGetValue("line", out Value lineValue) && lineValue.ConvertValue(out int line))
            {
                Line = line;
            }

            if (TryGetValue("at", out Value atValue) && atValue.ConvertValue(out string at))
            {
                At = at;
            }

            if (TryGetValue("pending", out Value pendingValue) && pendingValue.ConvertValue(out string pending))
            {
                Pending = pending;
            }

            if (TryGetValue("evaluated-by", out Value evaluatedByValue) && evaluatedByValue.ConvertValue(out string evaluatedBy))
            {
                EvaluatedBy = evaluatedBy;
            }

            if (TryGetValue("thread", out Value threadValue) && threadValue.ConvertValue(out string thread))
            {
                Thread = thread;
            }

            if (TryGetValue("thread-groups", out Value threadGroupValue) && threadGroupValue is ListValue threadGroupList)
            {
                ThreadGroups = threadGroupList;
            }

            if (TryGetValue("task", out Value taskValue) && taskValue.ConvertValue(out string task))
            {
                Task = task;
            }

            if (TryGetValue("cond", out Value condValue) && condValue.ConvertValue(out string cond))
            {
                Condition = cond;
            }

            if (TryGetValue("ignore", out Value ignoreValue) && ignoreValue.ConvertValue(out int ignore))
            {
                IgnoreCount = ignore;
            }

            if (TryGetValue("enable", out Value enableValue) && enableValue.ConvertValue(out int enable))
            {
                EnableCount = enable;
            }

            if (TryGetValue("pass", out Value passValue) && passValue.ConvertValue(out int pass))
            {
                PassCount = pass;
            }

            if (TryGetValue("times", out Value timesValue) && timesValue.ConvertValue(out int times))
            {
                HitCount = times;
            }

            if (TryGetValue("installed", out Value installedValue) && installedValue.ConvertValue(out string installed))
            {
                Installed = installed;
            }

            if (TryGetValue("locations", out Value locationsValue) && locationsValue is ListValue locationsList)
            {
                Locations = locationsList;
            }
        }

        private static TupleValue ValidateTuple(Value value)
        {
            if (value is TupleValue tuple)
            {
                return tuple;
            }

            throw new ArgumentException($"{nameof(value)} must be of type {nameof(TupleValue)}", nameof(value));
        }
    }
}
