using System.Runtime.Serialization;

namespace WorkoutBuddy.Controllers.Exercise.Model;

public enum ExerciseFilter
{
    [EnumMember(Value = "private")]
    PRIVATE,
    [EnumMember(Value = "public")]
    PUBLIC,
    [EnumMember(Value = "all")]
    ALL,
}