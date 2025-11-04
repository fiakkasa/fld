namespace fld;

internal static class Print
{
    internal static void AsMarkdown(this IReadOnlyCollection<FixFragment> entries)
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("Nothing to display...");
            return;
        }

        Dictionary<string, int> headersAndWidths = new()
        {
            { nameof(FixFragment.Tag), nameof(FixFragment.Tag).Length },
            { nameof(FixFragment.Name), nameof(FixFragment.Tag).Length },
            { nameof(FixFragment.Value), nameof(FixFragment.Tag).Length }
        };
        SetMaxWidths(entries, headersAndWidths);

        int maxTagLength = headersAndWidths[nameof(FixFragment.Tag)];
        int maxNameLength = headersAndWidths[nameof(FixFragment.Name)];
        int maxValueLength = headersAndWidths[nameof(FixFragment.Value)];

        PrintHeader(headersAndWidths);

        foreach (FixFragment entry in entries)
        {
            Console.WriteLine(
                $"| {Pad(entry.Tag, maxTagLength)} | {Pad(entry.Name, maxNameLength)} | {Pad(entry.Value, maxValueLength)} |"
            );
        }
    }

    private static void PrintHeader(Dictionary<string, int> headersAndWidths)
    {
        Console.WriteLine(
            "| " +
            string.Join(
                " | ",
                headersAndWidths.Select((header, i) => Pad(header.Key, header.Value))
            ) +
            " |"
        );
        Console.WriteLine(
            "| " +
            string.Join(
                " | ",
                headersAndWidths.Select((header, i) => Pad(string.Empty, header.Value, '-'))
            ) +
            " |"
        );
    }

    private static string Pad(string value, int length, char paddingChar = ' ')
    {
        return value.PadRight(length, paddingChar);
    }

    private static void SetMaxWidths(IReadOnlyCollection<FixFragment> entries, Dictionary<string, int> headersAndWidths)
    {
        foreach (FixFragment entry in entries)
        {
            if (entry.Tag.Length > headersAndWidths[nameof(FixFragment.Tag)])
            {
                headersAndWidths[nameof(FixFragment.Tag)] = entry.Tag.Length;
            }

            if (entry.Name.Length > headersAndWidths[nameof(FixFragment.Name)])
            {
                headersAndWidths[nameof(FixFragment.Name)] = entry.Name.Length;
            }

            if (entry.Value.Length > headersAndWidths[nameof(FixFragment.Value)])
            {
                headersAndWidths[nameof(FixFragment.Value)] = entry.Value.Length;
            }
        }
    }
}