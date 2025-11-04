using OneOf;

using QuickFix;
using QuickFix.Fields;

namespace fld;

internal static class Decoder
{
    internal static OneOf<IReadOnlyCollection<FixFragment>, string> Decode(
        string text,
        char delimiter,
        bool validateLog
    )
    {
        Message fixLog = new(text.Replace(delimiter, '\x01'), validateLog);
        string fixVersion = fixLog.Header.GetString(Tags.BeginString);
        IReadOnlyDictionary<int, string>? fixNamesDefinition = fixVersion switch
        {
            "FIX.4.2" => FixTagDefinitions.Fix42,
            "FIX.4.4" => FixTagDefinitions.Fix44,
            _ => null
        };

        if (fixNamesDefinition == null)
        {
            return $"Unsupported fix version {fixVersion}";
        }

        List<FixFragment> result = new();
        IEnumerable<KeyValuePair<int, IField>> collection =
            fixLog.Header
                .Concat(fixLog)
                .Concat(fixLog.Trailer);

        foreach (KeyValuePair<int, IField> field in collection)
        {
            int tag = field.Value.Tag;
            fixNamesDefinition.TryGetValue(tag, out string? name);

            result.Add(new FixFragment(
                    tag.ToString(),
                    name ?? string.Empty,
                    field.Value.ToString()
                )
            );
        }

        return result;
    }
}