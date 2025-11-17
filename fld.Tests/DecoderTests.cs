namespace fld.Tests;

public class DecoderTests
{
    private const string _name = nameof(DecoderTests);

    [Theory]
    [InlineData("", '|', false, "Please provide a FIX log string.", "blank")]
    [InlineData(" ", '|', false, "Please provide a FIX log string.", "empty")]
    [InlineData("9=176|35=D", '|', false, "error occurred while parsing", "invalid_message")]
    [InlineData("55=AAPL|", '|', false, "not determine FIX version", "no_fix_version")]
    [InlineData("8=FIX.1.1|55=AAPL|", '|', false, "Unsupported FIX version", "unsupported_fix_version")]
    [InlineData("8=FIX.4.2|55=AAPL|", '|', true, "error occurred while parsing", "error_on_validation")]
    [InlineData("8=FIX.4.2|55=AAPL|", '^', true, "error occurred while parsing", "mismatched_delimiter")]
    public async Task Decode_Should_Return_Error_Message_When_FixLog_Is_Invalid(
        string fixLog,
        char delimiter,
        bool validateLog,
        string errorMessageMatcher,
        string discriminator
    )
    {
        var result = Decoder.Decode(fixLog, delimiter, validateLog);

        Assert.True(result.IsT1);
        Assert.Contains(errorMessageMatcher, result.AsT1);
        result.AsT1.MatchSnapshot($"{_name}.{nameof(Decode_Should_Return_Error_Message_When_FixLog_Is_Invalid)}.{discriminator}");
    }

    [Theory]
    [InlineData("8=FIX.4.2|55=AAPL|", '|', false, "parsable")]
    [InlineData("[8=FIX.4.2|55=AAPL|", '|', false, "start_bracket")]
    [InlineData("8=FIX.4.2|55=AAPL|]", '|', false, "end_bracket")]
    [InlineData("[8=FIX.4.2|55=AAPL|]", '|', false, "start_end_brackets")]
    [InlineData("  [  8=FIX.4.2|55=AAPL|   ]   ", '|', false, "start_end_brackets_spaces")]
    [InlineData("8=FIX.4.2|55=AAPL", '|', false, "no_trailing_delimiter")]
    [InlineData("8=FIX.4.2^55=AAPL^", '^', false, "custom_delimiter")]
    [InlineData("8=FIX.4.2|5555=AAPL|", '|', false, "unmatched_tag_name")]
    [InlineData("8=FIX.4.2^9=76^35=A^49=BuySide^56=SellSide^34=1^52=20190605-11:27:06.897^98=0^108=30^141=Y^10=008^", '^', true, "parsable_validated")]
    public async Task Decode_Should_Return_Rendered_Message_When_FixLog_Is_Valid(
        string fixLog,
        char delimiter,
        bool validateLog,
        string discriminator
    )
    {
        var result = Decoder.Decode(fixLog, delimiter, validateLog);

        Assert.True(result.IsT0);
        result.AsT0.MatchSnapshot($"{_name}.{nameof(Decode_Should_Return_Rendered_Message_When_FixLog_Is_Valid)}.{discriminator}");
    }

    [Fact]
    public async Task Decode_Should_Return_Empty_Message_When_Cancellation_Requested()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var result = Decoder.Decode("8=FIX.4.2|55=AAPL|", '|', false, cts.Token);

        Assert.True(result.IsT0);
        Assert.Empty(result.AsT0);
    }
}
