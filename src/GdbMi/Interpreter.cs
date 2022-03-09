namespace GdbMi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GdbMi.Records;
    using GdbMi.Values;

    /// <summary>
    /// <c>Interpreter</c> for the GDB Machine Interface (GDB/MI).
    /// </summary>
    /// <remarks>
    /// <para>GDB/MI is a line based machine oriented text interface to GDB and is activated by specifying using the <c>--interpreter</c> command line option (<see href="https://sourceware.org/gdb/onlinedocs/gdb/Mode-Options.html#Mode-Options"/>).</para>
    /// <para>It is specifically intended to support the development of systems which use the debugger as just one small component of a larger system.</para>
    /// <para>GDB/MI documentation: <see href="https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI.html#GDB_002fMI"/>.</para>
    /// </remarks>
    public static class Interpreter
    {
        /// <summary>
        /// Parses and interprets GDB/MI output into a <seealso cref="Record"/>.
        /// </summary>
        /// <param name="line">Single line of GDB/MI output.</param>
        /// <returns>If successful, a <seealso cref="Record"/> result intepreted from <paramref name="line"/> contents, otherwise null.</returns>
        public static Record ParseOutput(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return null;
            }

            switch (line)
            {
                case var _ when line.StartsWith(PromptRecord.Prompt, StringComparison.Ordinal):
                    // GDB prompt. Waiting for input.
                    return new PromptRecord();

                case var _ when line[0] == StreamRecord.ConsolePrefix:
                    // Console stream record.
                    return new StreamRecord(StreamRecord.StreamType.Console, line.Substring(1).Trim(new char[] { '\"' }));

                case var _ when line[0] == StreamRecord.TargetPrefix:
                    // Target stream record.
                    return new StreamRecord(StreamRecord.StreamType.Target, line.Substring(1).Trim(new char[] { '\"' }));

                case var _ when line[0] == StreamRecord.LogPrefix:
                    // Log stream record.
                    return new StreamRecord(StreamRecord.StreamType.Log, line.Substring(1).Trim(new char[] { '\"' }));
            }

            string recordData = line;

            int bufferStartPos = 0;

            int bufferCurrentPos = bufferStartPos;

            char type = '?';

            int token = default;

            while (bufferCurrentPos < line.Length)
            {
                if (((bufferCurrentPos + 1) >= line.Length) || (line[bufferCurrentPos + 1] == ','))
                {
                    string @class = recordData.Substring(bufferStartPos, (bufferCurrentPos + 1) - bufferStartPos);

                    string data = string.Empty;

                    if (((bufferCurrentPos + 1) < line.Length) && (line[bufferCurrentPos + 1] == ','))
                    {
                        data = recordData.Substring(bufferCurrentPos + 2);
                    }

                    var results = ParseBuffer(data);

                    results = FilterDuplicateKeys(results);

                    return type switch
                    {
                        ResultRecord.ResultPrefix => new ResultRecord(token, @class, results),
                        AsyncRecord.ExecPrefix => new AsyncRecord(AsyncRecord.AsyncType.Exec, token, @class, results),
                        AsyncRecord.StatusPrefix => new AsyncRecord(AsyncRecord.AsyncType.Status, token, @class, results),
                        AsyncRecord.NotifyPrefix => new AsyncRecord(AsyncRecord.AsyncType.Notify, token, @class, results),
                        _ => throw new NotImplementedException($"unhandled record type: {type}"),
                    };
                }
                else if ((recordData[bufferCurrentPos] == ResultRecord.ResultPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.ExecPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.StatusPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.NotifyPrefix))
                {
                    type = recordData[bufferCurrentPos];

                    string stringToken = recordData.Substring(bufferStartPos, bufferCurrentPos);

                    if (!int.TryParse(stringToken, out token))
                    {
                        token = default;
                    }

                    bufferStartPos = ++bufferCurrentPos;
                }

                ++bufferCurrentPos;
            }

            return null;
        }

        private static IList<Value> FilterDuplicateKeys(IList<Value> list)
        {
            var distinctResults = new Dictionary<string, ResultValue>();

            foreach (Value value in list)
            {
                if (value is ResultValue result)
                {
                    distinctResults[result.Variable] = result;
                }
            }

            return distinctResults.Any() ? distinctResults.Values.ToList<Value>() : list;
        }

        private static IList<Value> ParseBuffer(string buffer)
        {
            var results = new List<Value>();

            int bufferStartPos = 0;

            int bufferCurrentPos = bufferStartPos;

            int enclosureCount = 0;

            int enclosuresProcessed = 0;

            bool insideQuotationEnclosure = false;

            string enclosureVariable = string.Empty;

            while (bufferCurrentPos < buffer.Length)
            {
                if ((buffer[bufferCurrentPos] == '=') && (enclosureCount == 0))
                {
                    enclosureVariable = buffer.Substring(bufferStartPos, bufferCurrentPos - bufferStartPos);

                    bufferStartPos = ++bufferCurrentPos;
                }

                if ((buffer[bufferCurrentPos] == '[') || (buffer[bufferCurrentPos] == '{'))
                {
                    ++enclosureCount;
                }
                else if ((buffer[bufferCurrentPos] == ']') || (buffer[bufferCurrentPos] == '}'))
                {
                    --enclosureCount;
                }
                else if ((buffer[bufferCurrentPos] == '\"') && (buffer[Math.Max(0, bufferCurrentPos - 1)] != '\\')) // non-escaped quotation
                {
                    if (insideQuotationEnclosure)
                    {
                        --enclosureCount;
                    }
                    else
                    {
                        ++enclosureCount;
                    }

                    insideQuotationEnclosure = !insideQuotationEnclosure;
                }

                // Handle a nested enclosure, const-variable, or the end of a string.
                if (((bufferCurrentPos + 1) >= buffer.Length) || ((buffer[bufferCurrentPos + 1] == ',') && (enclosureCount == 0)))
                {
                    string enclosedSegment = buffer.Substring(bufferStartPos, (bufferCurrentPos + 1) - bufferStartPos);

                    if (enclosedSegment.StartsWith("[", StringComparison.Ordinal))
                    {
                        string listValue = enclosedSegment.Trim(new char[] { '[', ']' }); // remove [] container

                        var nestedResults = ParseBuffer(listValue);

                        results.Add(new ResultValue(enclosureVariable, new ListValue(nestedResults)));
                    }
                    else if (enclosedSegment.StartsWith("{", StringComparison.Ordinal))
                    {
                        string tupleValueStr = enclosedSegment.Trim(new char[] { '{', '}' }); // remove {} container

                        var nestedResults = ParseBuffer(tupleValueStr);

                        var tupleValue = new TupleValue(nestedResults);

                        if (string.IsNullOrEmpty(enclosureVariable))
                        {
                            results.Add(tupleValue);
                        }
                        else
                        {
                            results.Add(new ResultValue(enclosureVariable, tupleValue));
                        }
                    }
                    else
                    {
                        string constValueStr = enclosedSegment.Trim(new char[] { '"' }); // remove quotation-marks container

                        var constValue = new ConstValue(constValueStr);

                        if (string.IsNullOrEmpty(enclosureVariable))
                        {
                            results.Add(constValue);
                        }
                        else
                        {
                            results.Add(new ResultValue(enclosureVariable, constValue));
                        }
                    }

                    ++enclosuresProcessed;

                    enclosureVariable = string.Empty;

                    insideQuotationEnclosure = false;

                    ++bufferCurrentPos;

                    bufferStartPos = bufferCurrentPos + 1;
                }

                ++bufferCurrentPos;
            }

            return results;
        }
    }
}
