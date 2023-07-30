using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WorkoutBuddy.Controllers.ExerciseModel;

[JsonConverter(typeof(JsonStringEnumMemberConverter))]
public enum MuscleGroupType
{
    // arms
    [EnumMember(Value = "Biceps")]
    Biceps,
    [EnumMember(Value = "Triceps")]
    Triceps,

    // upper body
    [EnumMember(Value = "Shoulders")]
    Shoulders,
    [EnumMember(Value = "Rhomboids")]
    Rhomboids,
    [EnumMember(Value = "Chest")]
    Chest,
    [EnumMember(Value = "UpperChest")]
    UpperChest,
    [EnumMember(Value = "Abs")]
    Abs,
    [EnumMember(Value = "UpperBack")]
    UpperBack,
    [EnumMember(Value = "LowerBack")]
    LowerBack,
    [EnumMember(Value = "Lats")]
    Lats,


    // lower body
    [EnumMember(Value = "Glutes")]
    Glutes,
    [EnumMember(Value = "Hamstrings")]
    Hamstrings,
    [EnumMember(Value = "Quadriceps")]
    Quadriceps,
}