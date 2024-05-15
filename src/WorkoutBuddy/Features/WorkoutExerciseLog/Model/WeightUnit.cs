using System.Text.Json.Serialization;

namespace WorkoutBuddy.Features;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum WeightUnit
{
    Kilogram,
    Pound
}