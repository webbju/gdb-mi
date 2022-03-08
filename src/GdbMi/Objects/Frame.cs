namespace GdbMi.Objects
{
    using System;
    using GdbMi.Values;

    /// <summary>
    /// <c>Frame</c> manages reported information about a stack frame.
    /// </summary>
    /// <remarks><see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Frame-Information.html#GDB_002fMI-Frame-Information"/>.</remarks>
    public class Frame : TupleValue
    {
        /// <summary>
        /// Constructs <c>Frame</c> from specified <c>Value</c>.
        /// </summary>
        /// <param name="value">Base <c>Value</c> object.</param>
        public Frame(Value value)
            : base(ValidateTuple(value))
        {
            SeedProperties();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected Frame()
            : base()
        {
        }

        /// <summary>
        /// Gets the level of the stack frame.
        /// </summary>
        /// <remarks>The innermost frame has the level of zero. This field is always present.</remarks>
        public int Level { get; private set; }

        /// <summary>
        /// Gets the name of the function corresponding to the frame.
        /// </summary>
        /// <remarks>This field may be absent if GDB is unable to determine the function name.</remarks>
        public string Function { get; private set; }

        /// <summary>
        /// Gets the arguments of the function corresponding to the frame.
        /// </summary>
        /// <remarks>This field may be absent if GDB is unable to determine the function name.</remarks>
        public ListValue FunctionArgs { get; private set; }

        /// <summary>
        /// Gets the code address for the frame.
        /// </summary>
        /// <remarks>This field is always present.</remarks>
        public string Address { get; private set; }

        /// <summary>
        /// Gets any optional flags related to the address.
        /// </summary>
        /// <remarks>These flags are architecture-dependent; <see href="https://sourceware.org/gdb/onlinedocs/gdb/Architectures.html#Architectures"/> for their meaning for a particular CPU.</remarks>
        public string AddressFlags { get; private set; }

        /// <summary>
        /// Gets the name of the source files that correspond to the frame's code address.
        /// </summary>
        /// <remarks>This field may be absent.</remarks>
        public string Filename { get; private set; }

        /// <summary>
        /// Gets the full file name of the source file which contains this function.
        /// </summary>
        /// <remarks>This field may be absent.</remarks>
        public string Fullname { get; private set; }

        /// <summary>
        /// Gets the source line corresponding to the frames' code address.
        /// </summary>
        /// <remarks>This field may be absent.</remarks>
        public int Line { get; private set; }

        /// <summary>
        /// Gets the name of the binary file (either executable or shared library) the corresponds to the frame's code address.
        /// </summary>
        /// <remarks>This field may be absent.</remarks>
        public string From { get; private set; }

        /// <summary>
        /// Seeds property values based on underlying tuple contents.
        /// </summary>
        protected void SeedProperties()
        {
            if (TryGetValue("level", out Value levelValue) && levelValue.ConvertValue(out int level))
            {
                Level = level;
            }

            if (TryGetValue("func", out Value funcValue) && funcValue.ConvertValue(out string func))
            {
                Function = func;
            }

            if (TryGetValue("args", out Value argsValue))
            {
                FunctionArgs = argsValue as ListValue;
            }

            if (TryGetValue("addr", out Value addrValue) && addrValue.ConvertValue(out string addr))
            {
                Address = addr;
            }

            if (TryGetValue("addr_flags", out Value addrFlagsValue) && addrFlagsValue.ConvertValue(out string addrFlags))
            {
                AddressFlags = addrFlags;
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

            if (TryGetValue("from", out Value fromValue) && fromValue.ConvertValue(out string from))
            {
                From = from;
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
