using OneOf;
using QuickFix;
using QuickFix.Fields;

namespace fld;

public static class Decoder
{
    private static OneOf<Message, Exception> ParseFixLogText(string fixLogText, bool validateLog)
    {
        try
        {
            return new Message(normalizedFixLogText, validateLog);
        } 
        catch(Exception ex)
        {
            return ex;
        }
    }
    public static OneOf<IReadOnlyCollection<FixFragment>, string> Decode(
        string fixLogText,
        char delimiter,
        bool validateLog,
        CancellationToken cancellationToken = default
    )
    {
        var normalizedFixLogText = fixLogText.ToNormalizedFixLogText();
        var fixLogMessageParsed = ParseFixLogText(normalizedFixLogText, validateLog).Value;

        if(fixLogMessageParsed.Value is Exception ex)
        {
            return $"""An error occured while parsing the provided fix log '({fixLogText})' with error: {ex.Message}""";
        }

        var fixLogMessage = fixLogMessageParsed.AsT0;

        var fixVersion = fixLog.Header.GetString(Tags.BeginString);
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

        var result = new List<FixFragment>();
        var collection =
            fixLogMessage.Header
                .Concat(fixLogMessage)
                .Concat(fixLogMessage.Trailer);

        foreach (KeyValuePair<int, IField> field in collection)
        {
            if(cancellationToken.IsCancellationRequested) 
            {
                return [];
            }

            var tag = field.Value.Tag;
            fixNamesDefinition.TryGetValue(tag, out var? name);

            result.Add(
                new(
                    tag.ToString(), 
                    name ?? string.Empty,
                    field.Value.ToString()
                )
            );
        }

        return result;
    }
}