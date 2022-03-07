namespace GdbMi
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using GdbMi.Records;
    using GdbMi.Values;

    public static class Interpreter
    {
        public static IRecord ParseOutputLine(string line)
        {
            if (string.IsNullOrEmpty(line))
            {
                return null;
            }

            switch (line)
            {
                case var _ when line.StartsWith(PromptRecord.Prompt):
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

            //
            // The following record types have associated key-pair data; identify the type and build a result collection.
            //

            string recordData = line;

            int bufferStartPos = 0;

            int bufferCurrentPos = bufferStartPos;

            char type = '?';

            uint token = 0;

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

                    switch (type)
                    {
                        case ResultRecord.ResultPrefix:
                            return new ResultRecord(token, @class, results);
                        case AsyncRecord.ExecPrefix:
                            return new AsyncRecord(AsyncRecord.AsyncType.Exec, token, @class, results);
                        case AsyncRecord.StatusPrefix:
                            return new AsyncRecord(AsyncRecord.AsyncType.Status, token, @class, results);
                        case AsyncRecord.NotifyPrefix:
                            return new AsyncRecord(AsyncRecord.AsyncType.Notify, token, @class, results);
                        default:
                            throw new NotImplementedException($"unhandled record type: {type}");
                    }
                }
                else if ((recordData[bufferCurrentPos] == ResultRecord.ResultPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.ExecPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.StatusPrefix)
                    || (recordData[bufferCurrentPos] == AsyncRecord.NotifyPrefix))
                {
                    type = recordData[bufferCurrentPos];

                    string stringToken = recordData.Substring(bufferStartPos, bufferCurrentPos);

                    uint.TryParse(stringToken, out token);

                    bufferStartPos = ++bufferCurrentPos;
                }

                ++bufferCurrentPos;
            }

            return null;
        }

        private static IList<Value> FilterDuplicateKeys(IList<Value> list)
        {
            var distinct = new Dictionary<string, ResultValue>();

            foreach (Value value in list)
            {
                if (value is ResultValue result)
                {
                    distinct[result.Variable] = result;
                }
            }

            if (distinct.Any())
            {
                return distinct.Values.ToList<Value>();
            }

            return list;
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

                if (((bufferCurrentPos + 1) >= buffer.Length) || ((buffer[bufferCurrentPos + 1] == ',') && (enclosureCount == 0)))
                {
                    //
                    // Handle a nested enclosure, const-variable, or the end of a string.
                    //

                    string enclosedSegment = buffer.Substring(bufferStartPos, (bufferCurrentPos + 1) - bufferStartPos);

                    if (enclosedSegment.StartsWith("["))
                    {
                        string listValue = enclosedSegment.Trim(new char[] { '[', ']' }); // remove [] container

                        var nestedResults = ParseBuffer(listValue);

                        results.Add(new ResultValue(enclosureVariable, new ListValue(nestedResults)));
                    }
                    else if (enclosedSegment.StartsWith("{"))
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
