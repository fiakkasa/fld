namespace fld;

public static class Extensions
{
    private const char _fixLogStart = '[';
    private const char _fixLogEnd = ']';
    private const string _markdownTableStartingSeparator = "| ";
    private const string _markdownTableEndingSeparator = " |";
    private const string _markdownTableInnerSeparator = " | ";
    private const char _markdownTableUnderlineChar = '-';

    private string ToNormalizedFixLogText(string fixLogText) =>
        fixLogText
            .Replace(delimiter, Message.SOH)
            .Trim()
            .TrimStart(_fixLogStart)
            .TrimEnd(_fixLogEnd);

    public static IEnumerable<string> ToEnumeratedMarkdownTable(
        this IReadOnlyCollection<FixFragment> entries, 
        CancellationToken cancellationToken = default
    )
    {
        if (entries.Count == 0)
        {
            yield break;
        }

        var headerWithWidthPadding = new Dictionary<string, int>
        {
            [nameof(FixFragment.Tag)] = nameof(FixFragment.Tag).Length,
            [nameof(FixFragment.Name)] = nameof(FixFragment.Tag).Length,
            [nameof(FixFragment.Value)] = nameof(FixFragment.Tag).Length
        };
        SetMaxWidths(entries, headerWithWidthPadding, cancellationToken);

        int maxTagLength = headerWithWidthPadding[nameof(FixFragment.Tag)];
        int maxNameLength = headerWithWidthPadding[nameof(FixFragment.Name)];
        int maxValueLength = headerWithWidthPadding[nameof(FixFragment.Value)];

        foreach(var item in headerWithWidthPadding.ToMarkdownTableHeader(cancellationToken))
        {
            yield return item;
        }

        foreach (FixFragment entry in entries)
        {
            if(cancellationToken.IsCancellationRequested) 
            {
                yield break;
            }

            yield return _markdownTableStartingSeparator
                + Pad(entry.Tag, maxTagLength)
                + _markdownTableInnerSeparator
                + Pad(entry.Name, maxNameLength)
                + _markdownTableInnerSeparator
                + Pad(entry.Value, maxValueLength)
                + _markdownTableEndingSeparator;
        }
    }

    public static IEnumerable<string> ToMarkdownTableHeader(
        this Dictionary<string, int> headerWithWidthPadding,
        CancellationToken cancellationToken = default
    )
    {
        if(cancellationToken.IsCancellationRequested) 
        {
            yield break;
        }

        yield return _markdownTableStartingSeparator
            + string.Join(
                _markdownTableInnerSeparator,
                headerWithWidthPadding.Select((header, i) => Pad(header.Key, header.Value))
            )
            + _markdownTableEndingSeparator;
        
        yield return _markdownTableStartingSeparator
            + string.Join(
                _markdownTableInnerSeparator,
                headerWithWidthPadding.Select((header, i) => 
                    Pad(string.Empty, header.Value, _markdownTableUnderlineChar)
                )
            )
            + _markdownTableEndingSeparator;
    }

    private static string Pad(string value, int length, char paddingChar = ' ') =>
        value.PadRight(length, paddingChar);

    private static void SetMaxWidths(
        IReadOnlyCollection<FixFragment> entries,
        Dictionary<string, int> headerWithWidthPadding,
        CancellationToken cancellationToken = default
    )
    {
        foreach (FixFragment entry in entries)
        {
            if(cancellationToken.IsCancellationRequested) 
            {
                yield break;
            }

            if (entry.Tag.Length > headerWithWidthPadding[nameof(FixFragment.Tag)])
            {
                headerWithWidthPadding[nameof(FixFragment.Tag)] = entry.Tag.Length;
            }

            if (entry.Name.Length > headerWithWidthPadding[nameof(FixFragment.Name)])
            {
                headerWithWidthPadding[nameof(FixFragment.Name)] = entry.Name.Length;
            }

            if (entry.Value.Length > headerWithWidthPadding[nameof(FixFragment.Value)])
            {
                headerWithWidthPadding[nameof(FixFragment.Value)] = entry.Value.Length;
            }
        }
    }
}