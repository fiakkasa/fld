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
            return new Message(fixLogText, validateLog);
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
        var normalizedFixLogText = fixLogText.ToNormalizedFixLogText(delimiter);
        var fixLogMessageParsed = ParseFixLogText(normalizedFixLogText, validateLog);

        if(fixLogMessageParsed.IsT1)
        {
            return $"An error occurred while parsing the provided fix log '({fixLogText})' with error: {fixLogMessageParsed.AsT1.Message}";
        }

        var fixLogMessage = fixLogMessageParsed.AsT0;

        var fixVersion = fixLogMessage.Header.GetString(Tags.BeginString);
        var fixNamesDefinition = fixVersion.ToFixTagDefinitions();

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
                return Array.Empty<FixFragment>();
            }

            var tag = field.Value.Tag;
            fixNamesDefinition.TryGetValue(tag, out var name);

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