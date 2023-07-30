using WorkoutBuddy.Data.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

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
        var creatorId = Guid.NewGuid();
        var item = new Exercise(
            id: null,
            owner: creatorId,
            creatorId: creatorId,
            name: "TEST",
            description: "TEST",
            imageUrl: null,
            isPublic: true,
            muscleGroups: new List<WorkoutBuddy.Controllers.ExerciseModel.MuscleGroupType> { }
            );
        Assert.IsNotNull(item);
    }
}