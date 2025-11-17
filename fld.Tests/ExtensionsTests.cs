using fld.Models;
using QuickFix;

namespace fld.Tests;

public class ExtensionsTests
{
    private const string _name = nameof(ExtensionsTests);

    [Theory]
    [InlineData("8=FIX.4.2|55=AAPL|", '|', "8=FIX.4.2\u000155=AAPL\u0001", "delimiter_substitution")]
    [InlineData("  8=FIX.4.2|55=AAPL|   ", '|', "8=FIX.4.2\u000155=AAPL\u0001", "start_end_spaces")]
    [InlineData("[8=FIX.4.2|55=AAPL|]", '|', "8=FIX.4.2\u000155=AAPL\u0001", "start_end_brackets")]
    [InlineData("[8=FIX.4.2|55=AAPL|", '|', "8=FIX.4.2\u000155=AAPL\u0001", "start_bracket")]
    [InlineData("8=FIX.4.2|55=AAPL|]", '|', "8=FIX.4.2\u000155=AAPL\u0001", "end_bracket")]
    [InlineData("  [  8=FIX.4.2|55=AAPL|   ]   ", '|', "8=FIX.4.2\u000155=AAPL\u0001", "start_end_brackets_spaces")]
    [InlineData("8=FIX.4.2|55=AAPL", '|', "8=FIX.4.2\u000155=AAPL\u0001", "no_trailing_delimiter")]
    [InlineData("8=FIX.4.2^55=AAPL^", '^', "8=FIX.4.2\u000155=AAPL\u0001", "custom_delimiter")]
    [InlineData("8=FIX.4.2*55=AAPL*", '|', "8=FIX.4.2*55=AAPL*", "mismatched_delimiter")]
    public void ToNormalizedFixLogText_Should_Return_Resulting_Text(
        string fixLog,
        char delimiter,
        string expectedResult,
        string discriminator
    )
    {
        var result = fixLog.ToNormalizedFixLogText(delimiter);

        Assert.Equal(expectedResult, result);
        result.MatchSnapshot($"{_name}.{nameof(ToNormalizedFixLogText_Should_Return_Resulting_Text)}.{discriminator}");
    }

    [Theory]
    [InlineData("FIX.4.2")]
    [InlineData("FIX.4.4")]
    public void ToFixTagDefinitions_Should_Return_Correct_Definitions_When_Supported(
        string fixVersion
    )
    {
        var result = fixVersion.ToFixTagDefinitions();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("FIX.1.1")]
    public void ToFixTagDefinitions_Should_Return_Null_Definitions_When_Not_Supported(
        string fixVersion
    )
    {
        var result = fixVersion.ToFixTagDefinitions();

        Assert.Null(result);
    }

    [Theory]
    [InlineData("8=FIX.4.2\u0001", 1, "header_only")]
    [InlineData("5=AAPL\u0001", 1, "body_only")]
    [InlineData("10=5\u0001", 1, "trailer_only")]
    [InlineData("8=FIX.4.2\u000155=AAPL\u0001", 2, "header_body")]
    [InlineData("8=FIX.4.2\u000110=5\u0001", 2, "header_trailer")]
    [InlineData("55=AAPL\u000110=5\u0001", 2, "body_trailer")]
    [InlineData("8=FIX.4.2\u000155=AAPL\u000110=5\u0001", 3, "all_sections")]
    public void AsFullEnumerable_Should_Return_Definitions_For_Sections(
        string fixLog,
        int expectedCount,
        string discriminator
    )
    {
        var result = new Message(fixLog, false).AsFullEnumerable().ToArray();

        Assert.Equal(expectedCount, result.Length);
        result.MatchSnapshot($"{_name}.{nameof(AsFullEnumerable_Should_Return_Definitions_For_Sections)}.{discriminator}");
    }

    [Fact]
    public void ToEnumeratedMarkdownTable_Should_Return_Empty_Collection_Empty()
    {
        FixFragment[] collection = [];

        var result = collection.ToEnumeratedMarkdownTable();

        Assert.Empty(result);
    }

    [Fact]
    public void ToEnumeratedMarkdownTable_Should_Return_Empty_Collection_When_Cancellation_Requested()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        FixFragment[] collection =
        [
            new("8", "BeginString", "FIX.4.2"),
            new("55", "Symbol", "AAPL")
        ];

        var result = collection.ToEnumeratedMarkdownTable(cts.Token);

        Assert.Empty(result);
    }

    [Fact]
    public void ToEnumeratedMarkdownTable_Should_Return_Collection_When_Not_Empty()
    {
        FixFragment[] collection =
        [
            new("8", "BeginString", "FIX.4.2"),
            new("55", "Symbol", "AAPL"),
            new("55555", "Long Tag Name", "tag value")
        ];

        var result = collection.ToEnumeratedMarkdownTable().ToArray();

        Assert.NotEmpty(result);
        Assert.Equal(5, result.Length);
        result.MatchSnapshot();
    }
}
