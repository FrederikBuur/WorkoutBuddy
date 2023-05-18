using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace WorkoutBuddy.EFConverters;

public class RecordArrayConveter : ValueConverter<Guid[], string>
{
    // Unicode RECORD SEPARATOR (U+241E)
    public const char Seperator = '\u241E';
    // Pad result string with Seperator at the beginning and the end, to allow contains queries by checking if column contains $"{Seperator}{Query}${Seperator}".
    private static readonly Expression<Func<Guid[], string>> convertForward = v => v.Length > 0 ? Seperator + string.Join(Seperator, v) + Seperator : "";
    private static readonly Expression<Func<string, Guid[]>> convertBackwards = stringValue =>
        string.IsNullOrEmpty(stringValue)
            ? new Guid[0]
            : stringValue
                .Substring(("" + Seperator).Length, stringValue.Length - ("" + Seperator + Seperator).Length)
                .Split(Seperator, System.StringSplitOptions.None)
                .Select(v => Guid.Parse(v))
                .ToArray();
    public RecordArrayConveter(ConverterMappingHints? mappingHints = null) : base(convertForward, convertBackwards, mappingHints) { }
}