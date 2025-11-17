using fld.Models;
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
            var result = new Message(fixLogText, validateLog);

            return result.Any() switch
            {
                true => result,
                _ => new ArgumentException(
                    "No fields could be parsed for provided fix log."
                )
            };
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    private static OneOf<string, Exception> GetFixVersion(Message fixLogMessage)
    {
        try
        {
            return fixLogMessage.Header.GetString(Tags.BeginString);
        }
        catch (Exception ex)
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
        if (string.IsNullOrWhiteSpace(fixLogText))
        {
            return "Please provide a FIX log string.";
        }

        var normalizedFixLogText = fixLogText.ToNormalizedFixLogText(delimiter);
        var fixLogMessageParsed = ParseFixLogText(normalizedFixLogText, validateLog);

        if (fixLogMessageParsed.IsT1)
        {
            return $"An error occurred while parsing the provided FIX log '{fixLogText}' with error: {fixLogMessageParsed.AsT1.Message}";
        }

        var fixLogMessage = fixLogMessageParsed.AsT0;

        var fixVersion = GetFixVersion(fixLogMessage).Match(
            version => version,
            _ => string.Empty
        );

        if (fixVersion == string.Empty)
        {
            return $"Could not determine FIX version for provided FIX log '{fixLogText}'.";
        }

        var fixNamesDefinition = fixVersion.ToFixTagDefinitions();

        if (fixNamesDefinition == null)
        {
            return $"Unsupported FIX version '{fixVersion}' for provided FIX log '{fixLogText}'.";
        }

        var result = new List<FixFragment>();
        var collection =
            fixLogMessage.Header
                .Concat(fixLogMessage)
                .Concat(fixLogMessage.Trailer);

        foreach (KeyValuePair<int, IField> field in collection)
        {
            if (cancellationToken.IsCancellationRequested)
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