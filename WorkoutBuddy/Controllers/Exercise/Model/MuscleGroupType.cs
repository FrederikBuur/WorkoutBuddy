using System.Runtime.Serialization;

namespace workouts;

public enum MuscleGroupType
{
    // arms
    [EnumMember(Value = "Bicep")]
    Bicep,
    [EnumMember(Value = "Tricep")]
    Tricep,

    // upper body
    [EnumMember(Value = "Shoulder")]
    Shoulder,
    [EnumMember(Value = "Chest")]
    Chest,
    [EnumMember(Value = "Abs")]
    Abs,
    [EnumMember(Value = "UpperBack")]
    UpperBack,
    [EnumMember(Value = "LowerBack")]
    LowerBack,
    [EnumMember(Value = "Lat")]
    Lat,

    // lower body
    [EnumMember(Value = "Glutes")]
    Glutes,
    [EnumMember(Value = "Hamstring")]
    Hamstring,
    [EnumMember(Value = "Quads")]
    Quads,
}