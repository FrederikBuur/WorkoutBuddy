using WorkoutBuddy.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Deloitte42.Test;

[TestClass]
public class TestSuite
{
    [TestMethod]
    public void CanRunTest()
    {
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void CanReferenceProject()
    {
        var item = new ExerciseDto();
        Assert.IsNotNull(item);
    }
}