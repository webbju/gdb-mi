namespace GdbMi.Tests
{
    using GdbMi.Objects;
    using GdbMi.Records;
    using GdbMi.Values;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObjectsTests
    {
        [TestMethod]
        public void TestBreakpoint()
        {
            var record = Interpreter.ParseOutput("^done,bkpt={number=\"1\",type=\"breakpoint\",disp=\"keep\",enabled=\"y\",addr=\"0x08048564\",func=\"main\",file=\"myprog.c\",fullname=\"/home/myprog.c\",line=\"68\",thread-groups=[\"i1\"],times=\"0\"}");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0u, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);

            var breakpoint = new Breakpoint(record["bkpt"]);
            Assert.AreEqual(new ConstValue("1"), breakpoint["number"]);
            Assert.AreEqual(new ConstValue("breakpoint"), breakpoint["type"]);
            Assert.AreEqual(new ConstValue("keep"), breakpoint["disp"]);
            Assert.AreEqual(new ConstValue("y"), breakpoint["enabled"]);
            Assert.AreEqual(new ConstValue("0x08048564"), breakpoint["addr"]);
            Assert.AreEqual(new ConstValue("main"), breakpoint["func"]);
            Assert.AreEqual(new ConstValue("myprog.c"), breakpoint["file"]);
            Assert.AreEqual(new ConstValue("/home/myprog.c"), breakpoint["fullname"]);
            Assert.AreEqual(new ConstValue("68"), breakpoint["line"]);
            Assert.AreEqual(new ConstValue("0"), breakpoint["times"]);

            Assert.AreEqual(1, breakpoint.Number);
            Assert.AreEqual("breakpoint", breakpoint.Type);
            Assert.AreEqual("keep", breakpoint.Disposition);
            Assert.AreEqual("y", breakpoint.Enabled);
            Assert.AreEqual("0x08048564", breakpoint.Address);
            Assert.AreEqual("main", breakpoint.Function);
            Assert.AreEqual("myprog.c", breakpoint.Filename);
            Assert.AreEqual("/home/myprog.c", breakpoint.Fullname);
            Assert.AreEqual(68, breakpoint.Line);
            Assert.AreEqual(0, breakpoint.HitCount);

            var threadGroups = breakpoint["thread-groups"];
            Assert.IsInstanceOfType(threadGroups, typeof(ListValue));
            Assert.AreEqual(1, threadGroups.Count);
            Assert.AreEqual(new ConstValue("i1"), threadGroups[0]);
            Assert.AreSame(threadGroups, breakpoint.ThreadGroups);
        }

        [TestMethod]
        public void TestFrame()
        {
            var record = Interpreter.ParseOutput("*stopped,reason=\"breakpoint - hit\",disp=\"keep\",bkptno=\"1\",thread-id=\"0\",frame={addr=\"0x08048564\",func=\"main\",args=[{name=\"argc\",value=\"1\"},{name=\"argv\",value=\"0xbfc4d4d4\"}],file=\"myprog.c\",fullname=\"/home/myprog.c\",line=\"68\",arch=\"i386: x86_64\"}");
            Assert.IsInstanceOfType(record, typeof(AsyncRecord));
            Assert.AreEqual(0u, (record as AsyncRecord).Token);
            Assert.AreEqual(AsyncRecord.AsyncType.Exec, (record as AsyncRecord).Type);
            Assert.AreEqual("stopped", (record as AsyncRecord).Class);

            var frame = new Frame(record["frame"]);
            Assert.AreEqual("0x08048564", frame.Address);
            Assert.AreEqual("main", frame.Function);
            Assert.AreEqual(new ConstValue("argc"), frame.FunctionArgs[0]["name"]);
            Assert.AreEqual("myprog.c", frame.Filename);
            Assert.AreEqual("/home/myprog.c", frame.Fullname);
            Assert.AreEqual(68, frame.Line);
        }

        [TestMethod]
        public void TestThread()
        {
            var record = Interpreter.ParseOutput("^done,threads=[{id=\"2\",target-id=\"Thread 0xb7e14b90 (LWP 21257)\",frame={level=\"0\",addr=\"0xffffe410\",func=\"__kernel_vsyscall\",args=[]},state=\"running\"},{id=\"1\",target-id=\"Thread 0xb7e156b0 (LWP 21254)\",frame={level=\"0\",addr=\"0x0804891f\",func=\"foo\",args=[{name=\"i\",value=\"10\"}],file=\"/tmp/a.c\",fullname=\"/tmp/a.c\",line=\"158\",arch=\"i386:x86_64\"},state=\"running\"}],current-thread-id=\"1\"");
            Assert.IsInstanceOfType(record, typeof(ResultRecord));
            Assert.AreEqual(0u, (record as ResultRecord).Token);
            Assert.AreEqual("done", (record as ResultRecord).Class);
            Assert.AreEqual(2, (record as ResultRecord)["threads"].Count);
            Assert.AreEqual(new ConstValue("1"), (record as ResultRecord)["current-thread-id"]);

            var thread = new Thread(record["threads"][0]);
            Assert.AreEqual(2, thread.Id);
            Assert.AreEqual("Thread 0xb7e14b90 (LWP 21257)", thread.TargetId);
            Assert.AreEqual("running", thread.State);
            Assert.AreEqual(0, thread.Frame.Level);
            Assert.AreEqual("0xffffe410", thread.Frame.Address);
            Assert.AreEqual("__kernel_vsyscall", thread.Frame.Function);
            Assert.AreEqual(0, thread.Frame.FunctionArgs.Count);

            thread = new Thread(record["threads"][1]);
            Assert.AreEqual(1, thread.Id);
            Assert.AreEqual("Thread 0xb7e156b0 (LWP 21254)", thread.TargetId);
            Assert.AreEqual("running", thread.State);
            Assert.AreEqual(0, thread.Frame.Level);
            Assert.AreEqual("0x0804891f", thread.Frame.Address);
            Assert.AreEqual("foo", thread.Frame.Function);
            Assert.AreEqual(1, thread.Frame.FunctionArgs.Count);
            Assert.AreEqual("/tmp/a.c", thread.Frame.Filename);
            Assert.AreEqual("/tmp/a.c", thread.Frame.Fullname);
            Assert.AreEqual(158, thread.Frame.Line);
        }
    }
}
