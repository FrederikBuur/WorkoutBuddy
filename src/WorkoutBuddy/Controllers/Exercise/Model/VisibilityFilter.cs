using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WorkoutBuddy.Controllers.ExerciseModel;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum VisibilityFilter
{
    [EnumMember(Value = "owned")]
    OWNED,
    [EnumMember(Value = "public")]
    PUBLIC,
    [EnumMember(Value = "all")]
    ALL,
}