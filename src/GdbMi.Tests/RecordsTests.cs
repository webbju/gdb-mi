namespace GdbMi.Tests
{
    using GdbMi;
    using GdbMi.Records;
    using GdbMi.Values;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RecordsTests
    {
        [TestMethod]
        public void ParseInvalidLines()
        {
            Assert.IsNull(Interpreter.ParseOutput(""));
            Assert.IsNull(Interpreter.ParseOutput("\t\t\t\t\t"));
        }

        [TestMethod]
        public void ParsePromptRecord()
        {
            var promptRecord = Interpreter.ParseOutput("(gdb)");
            Assert.IsInstanceOfType(promptRecord, typeof(PromptRecord));
        }

        [TestMethod]
        public void ParseStreamRecords()
        {
            var consoleRecord = Interpreter.ParseOutput("~~~console text output~~");
            Assert.IsInstanceOfType(consoleRecord, typeof(StreamRecord));
            Assert.AreEqual(StreamRecord.StreamType.Console, (consoleRecord as StreamRecord).Type);
            Assert.AreEqual("~~console text output~~", (consoleRecord as StreamRecord).Content);

            var targetRecord = Interpreter.ParseOutput("@@@target text output@@");
            Assert.IsInstanceOfType(targetRecord, typeof(StreamRecord));
            Assert.AreEqual(StreamRecord.StreamType.Target, (targetRecord as StreamRecord).Type);
            Assert.AreEqual("@@target text output@@", (targetRecord as StreamRecord).Content);

            var logRecord = Interpreter.ParseOutput("&&&log text output&&");
            Assert.IsInstanceOfType(logRecord, typeof(StreamRecord));
            Assert.AreEqual(StreamRecord.StreamType.Log, (logRecord as StreamRecord).Type);
            Assert.AreEqual("&&log text output&&", (logRecord as StreamRecord).Content);
        }

        [TestMethod]
        public void ParseAsyncRecords()
        {
            var record = Interpreter.ParseOutput("*stopped,reason=\"reason\",thread-id=\"id\",stopped-threads=\"stopped\",core=\"core\"");
            Assert.IsInstanceOfType(record, typeof(AsyncRecord));
            Assert.AreEqual(AsyncRecord.AsyncType.Exec, (record as AsyncRecord).Type);
            Assert.AreEqual("stopped", (record as AsyncRecord).Class);

            // TODO: Find a real-world example of a status message.
            record = Interpreter.ParseOutput("+status,reason=\"reason\"");
            Assert.IsInstanceOfType(record, typeof(AsyncRecord));
            Assert.AreEqual(AsyncRecord.AsyncType.Status, (record as AsyncRecord).Type);
            Assert.AreEqual("status", (record as AsyncRecord).Class);

            record = Interpreter.ParseOutput("=thread-group-added,id=\"id\"");
            Assert.IsInstanceOfType(record, typeof(AsyncRecord));
            Assert.AreEqual(AsyncRecord.AsyncType.Notify, (record as AsyncRecord).Type);
            Assert.AreEqual("thread-group-added", (record as AsyncRecord).Class);
        }

        [TestMethod]
        public void ParseResultRecordToken()
        {
            string[] classes = { "done", "running", "connected", "error", "exit" };

            foreach (var @class in classes)
            {
                var record = Interpreter.ParseOutput($"{int.MaxValue}^{@class}");
                Assert.IsInstanceOfType(record, typeof(ResultRecord));
                Assert.AreEqual(int.MaxValue, (record as ResultRecord).Token);
                Assert.AreEqual(@class, (record as ResultRecord).Class);
            }
        }

        [TestMethod]
        public void ParseResultRecordWithConstValue()
        {
            var record = Interpreter.ParseOutput("^done,value=\"0xefffeb7c\"");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);

            var resultValue = (record as ResultRecord)["value"];
            Assert.IsInstanceOfType(resultValue, typeof(ConstValue));
            Assert.AreEqual(new ConstValue("0xefffeb7c"), resultValue);
            Assert.AreEqual("\"0xefffeb7c\"", resultValue.ToString());
        }

        [TestMethod]
        public void ParseResultRecordWithListValue()
        {
            var record = Interpreter.ParseOutput("^done,features=[\"async\",\"breakpoint-notifications\"]");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);

            var features = (record as ResultRecord)["features"];
            Assert.IsInstanceOfType(features, typeof(ListValue));
            Assert.AreEqual(new ConstValue("async"), features[0]);
            Assert.AreEqual(new ConstValue("breakpoint-notifications"), features[1]);
            Assert.AreEqual("[\"async\",\"breakpoint-notifications\"]", features.ToString());
        }

        [TestMethod]
        public void ParseResultRecordWithResultListValue()
        {
            var record = Interpreter.ParseOutput("^done,numchild=\"1\",children=[child={name=\"name\",exp=\"exp\",numchild=\"0\",value=\"value\",type=\"type\"}]");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);
            Assert.IsInstanceOfType((record as ResultRecord)["numchild"], typeof(ConstValue));
            Assert.IsInstanceOfType((record as ResultRecord)["children"], typeof(ListValue));
        }

        [TestMethod]
        public void ParseResultRecordWithTupleValue()
        {
            var record = Interpreter.ParseOutput("^done,groups=[{id=\"17\",type=\"process\",pid=\"yyy\",num_children=\"2\"}]");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);

            var groups = (record as ResultRecord)["groups"];
            Assert.IsInstanceOfType(groups, typeof(ListValue));
            Assert.IsInstanceOfType(groups[0], typeof(TupleValue));
            Assert.AreEqual(new ConstValue("17"), groups[0]["id"]);
            Assert.AreEqual("[{id=\"17\",type=\"process\",pid=\"yyy\",num_children=\"2\"}]", groups.ToString());
        }

        [TestMethod]
        public void ParseResultRecordWithDuplicateListSubkeys()
        {
            var line = "^done,asm_insns=[src_and_asm_line={line=\"31\",file=\"../../../src/gdb/testsuite/gdb.mi/basics.c\",fullname=\"/absolute/path/to/src/gdb/testsuite/gdb.mi/basics.c\",line_asm_insn=[{address=\"0x000107bc\",func-name=\"main\",offset=\"0\",inst=\"save  %sp, -112, %sp\"}]},src_and_asm_line={line=\"32\",file=\"../../../src/gdb/testsuite/gdb.mi/basics.c\",fullname=\"/absolute/path/to/src/gdb/testsuite/gdb.mi/basics.c\",line_asm_insn=[{address=\"0x000107c0\",func-name=\"main\",offset=\"4\",inst=\"mov  2, %o0\"},{address=\"0x000107c4\",func-name=\"main\",offset=\"8\",inst=\"sethi  %hi(0x11800), %o2\"}]}]";
            var record = Interpreter.ParseOutput(line);
            Assert.IsInstanceOfType(record, typeof(ResultRecord));

            var asm_insns = (record as ResultRecord)["asm_insns"];
            Assert.IsInstanceOfType(asm_insns, typeof(ListValue));
            Assert.IsInstanceOfType(asm_insns[0], typeof(ResultValue));
            Assert.AreEqual(new ConstValue("31"), asm_insns[0]["line"]);
            Assert.AreEqual(new ConstValue("../../../src/gdb/testsuite/gdb.mi/basics.c"), asm_insns[0]["file"]);
            Assert.AreEqual(new ConstValue("/absolute/path/to/src/gdb/testsuite/gdb.mi/basics.c"), asm_insns[0]["fullname"]);
            Assert.AreEqual(2, asm_insns.Count);
        }

        [TestMethod]
        public void ParseAsyncRecordWithDuplicateResults()
        {
            // Not sure if this is a bug, but it's possible for GDB to output multiple duplicate fields in async records.
            // Docs seem to indicate that this isn't typical behaviour though for migitation we'll select the "right most" (latest) keys if this occurs.
            // https://sourceware.org/gdb/onlinedocs/gdb/GDB_002fMI-Async-Records.html#GDB_002fMI-Async-Records

            var record = Interpreter.ParseOutput("*stopped,reason=\"signal - received\",signal-name=\"SIGSEGV\",signal-meaning=\"Segmentation fault\",reason=\"signal - received\",signal-name=\"SIGSEGV\",signal-meaning=\"Segmentation fault\",reason=\"exited - signalled\",signal-name=\"SIGSEGV\",signal-meaning=\"Segmentation fault\"");
            Assert.IsInstanceOfType(record, typeof(AsyncRecord));
            Assert.AreEqual(0, (record as AsyncRecord).Token);
            Assert.AreEqual("stopped", (record as AsyncRecord).Class);

            Assert.AreEqual(new ConstValue("exited - signalled"), (record as AsyncRecord)["reason"]);
            Assert.AreEqual(new ConstValue("SIGSEGV"), (record as AsyncRecord)["signal-name"]);
            Assert.AreEqual(new ConstValue("Segmentation fault"), (record as AsyncRecord)["signal-meaning"]);
        }
    }
}
