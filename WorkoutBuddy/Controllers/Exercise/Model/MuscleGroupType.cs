using System.Runtime.Serialization;

namespace workouts;

public enum MuscleGroupType
{
    [EnumMember(Value = "Bicep")]
    Bicep,
    [EnumMember(Value = "Tricep")]
    Tricep,
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
    [EnumMember(Value = "Glutes")]
    Glutes,
    [EnumMember(Value = "Hamstring")]
    Hamstring,
    [EnumMember(Value = "Quads")]
    Quads,
    [EnumMember(Value = "Lat")]
    Lat,
}