using fld.Models;

namespace fld.Tests;

public class PrintTests
{
    [Fact]
    public void AsMarkdown_When_Empty_Collection_Writes_Nothing_Message()
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            var entries = Array.Empty<FixFragment>();
            Print.AsMarkdown(entries);

            var result = sw.ToString();
            Assert.Contains("Nothing to display...", result);
            result.MatchSnapshot();
        }
        finally
        {
            Console.SetOut(Console.Out);
        }
    }

    [Fact]
    public void AsMarkdown_When_Collection_Writes_Result()
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            FixFragment[] entries =
            [
                new ("8", "BeginString", "FIX.4.2"),
                new ("55", "Symbol", "AAPL"),
            ];
            Print.AsMarkdown(entries, CancellationToken.None);

            var result = sw.ToString();
            Assert.Contains("8", result);
            Assert.Contains("BeginString", result);
            Assert.Contains("FIX.4.2", result);
            result.MatchSnapshot();
        }
        finally
        {
            Console.SetOut(Console.Out);
        }
    }
}
