namespace GdbMi.Objects;

using System;
using GdbMi.Values;

/// <summary>
/// <c>Thread</c> manages reported information about a thread.
/// </summary>
/// <remarks>
/// It uses it uses a tuple with the following fields:
/// <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Thread-Information.html#GDB_002fMI-Thread-Information"/>.
/// </remarks>
public class Thread : TupleValue
{
    /// <summary>
    /// Constructs <c>Thread</c> from specified <c>Value</c>.
    /// </summary>
    /// <param name="value">Base <c>Value</c> object.</param>
    public Thread(Value value)
        : base(ValidateTuple(value))
    {
        SeedProperties();
    }

    /// <summary>
    /// Default constructor.
    /// </summary>
    protected Thread()
        : base()
    {
    }

    /// <summary>
    /// Gets the global numeric id assigned to the thread by GDB.
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// Gets the target-specific string identifying the thread.
    /// </summary>
    public string TargetId { get; private set; }

    /// <summary>
    /// Gets additional information about the thread provided by the target.
    /// </summary>
    /// <remarks>
    /// It is supposed to be human-readable and not interpreted by the frontend.
    /// This field is optional.
    /// </remarks>
    public string Details { get; private set; }

    /// <summary>
    /// Gets the name of the thread.
    /// </summary>
    /// <remarks>
    /// If the user specified a name using the thread name command, then this name is given.
    /// Otherwise, if GDB can extract the thread name from the target, then that name is given.
    /// If GDB cannot find the thread name, then this field is omitted.
    /// </remarks>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the execution state of the thread.
    /// </summary>
    /// <remarks>Either <c>"stopped"</c> or <c>"running"</c>, depending on whether the thread is presently running.</remarks>
    public string State { get; private set; }

    /// <summary>
    /// Gets the stack frame currently executing in the thread.
    /// </summary>
    /// <remarks>This field is only present if the thread is stopped.</remarks>
    public Frame Frame { get; private set; }

    /// <summary>
    /// Gets the value of this field as an integer number of the processor core the thread was last seen on.
    /// </summary>
    /// <remarks>This field is optional.</remarks>
    public int Core { get; private set; }

    /// <summary>
    /// Seeds property values based on underlying tuple contents.
    /// </summary>
    protected void SeedProperties()
    {
        if (TryGetValue("id", out Value idValue) && idValue.ConvertValue(out int id))
        {
            Id = id;
        }

        if (TryGetValue("target-id", out Value targetIdValue) && targetIdValue.ConvertValue(out string targetId))
        {
            TargetId = targetId;
        }

        if (TryGetValue("details", out Value detailsValue) && detailsValue.ConvertValue(out string details))
        {
            Details = details;
        }

        if (TryGetValue("name", out Value nameValue) && nameValue.ConvertValue(out string name))
        {
            Name = name;
        }

        if (TryGetValue("state", out Value stateValue) && stateValue.ConvertValue(out string state))
        {
            State = state;
        }

        if (TryGetValue("frame", out Value frameValue))
        {
            Frame = new Frame(frameValue);
        }

        if (TryGetValue("core", out Value coreValue) && coreValue.ConvertValue(out int core))
        {
            Core = core;
        }
    }

    static private TupleValue ValidateTuple(Value value)
    {
        if (value is TupleValue tuple)
        {
            return tuple;
        }

        throw new ArgumentException($"{nameof(value)} must be of type {nameof(TupleValue)}", nameof(value));
    }
}
