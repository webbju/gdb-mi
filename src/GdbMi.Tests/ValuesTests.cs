namespace GdbMi.Tests;

using System;
using System.Collections.Generic;
using GdbMi.Values;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class ValuesTests
{
    [TestMethod]
    public void TestValueTypes()
    {
        var emptyList = new List<Value>();

        Assert.IsInstanceOfType(new ConstValue("value"), typeof(Value));
        Assert.IsInstanceOfType(new TupleValue(emptyList), typeof(Value));
        Assert.IsInstanceOfType(new ListValue(emptyList), typeof(Value));

        Assert.IsInstanceOfType(new ResultValue("variable", new ConstValue("value")), typeof(Value));
        Assert.IsInstanceOfType(new ResultValue("variable", new TupleValue(emptyList)), typeof(Value));
        Assert.IsInstanceOfType(new ResultValue("variable", new ListValue(emptyList)), typeof(Value));

        Assert.ThrowsException<ArgumentNullException>(() => new ConstValue(null), "value must be non-null");
        Assert.ThrowsException<ArgumentNullException>(() => new TupleValue(null), "values must be non-null");
        Assert.ThrowsException<ArgumentNullException>(() => new ListValue(null), "values must be non-null");

        Assert.ThrowsException<ArgumentException>(() => new ResultValue("", new ConstValue("value")), "variable must be non-empty");
        Assert.ThrowsException<ArgumentException>(() => new ResultValue("", new TupleValue(emptyList)), "variable must be non-empty");
        Assert.ThrowsException<ArgumentException>(() => new ResultValue("", new ListValue(emptyList)), "variable must be non-empty");
    }
}
