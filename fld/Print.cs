namespace fld;

public static class Print
{
    public static void AsMarkdown(
        IReadOnlyCollection<FixFragment> entries,
        CancellationToken cancellationToken = default
    )
    {
        if (entries.Count == 0)
        {
            Console.WriteLine("Nothing to display...");
            return;
        }

        foreach (var item in entries.ToEnumeratedMarkdownTable(cancellationToken))
        {
            Console.WriteLine(item);
        }
    }
}