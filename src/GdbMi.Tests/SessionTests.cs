namespace GdbMi.Tests;

using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GdbMi.Objects;
using GdbMi.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

[TestClass]
public class SessionTests
{
    [TestMethod]
    public async Task MockSessionResponse()
    {
        var waitHandle = new AutoResetEvent(false);

        var mockProcess = new Mock<Process>();
        var session = new Session(mockProcess.Object);

        var command = await session.SendCommandAsync("111-break-insert main", r => waitHandle.Set());
        Assert.IsNotNull(command.CompletionSource.Task);
        Assert.AreEqual(111, command.Token);
        Assert.AreEqual("-break-insert main", command.Command);
        
        session.ProcessStdout("111^done,bkpt={number=\"1\",type=\"breakpoint\",disp=\"keep\",enabled=\"y\",addr=\"0x08048564\",func=\"main\",file=\"myprog.c\",fullname=\"/home/myprog.c\",line=\"68\",thread-groups=[\"i1\"],times=\"0\"}");

        var offloadTask = Task.Run(async () =>
        {
            var record = await command;
            Assert.IsNotNull(record);
            Assert.IsTrue(waitHandle.WaitOne(0));

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
            return record;
        });

        var offloadResult = await offloadTask;
        Assert.IsNotNull(offloadResult);
    }
}
