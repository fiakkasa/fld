using CommandLine;

namespace fld.Models;

[ExcludeFromCodeCoverage]
public sealed record Options
{
    [Value(0, MetaName = "fixlog", HelpText = "The FIX log string to decode", Required = false)]
    public string FixLog { get; set; } = string.Empty;

    [Option('d', "delimiter", HelpText = "Custom FIX delimiter", Default = '|')]
    public char Delimiter { get; set; } = '|';

    [Option('v', "validate-log", HelpText = "Validate log (true|false)", Default = false)]
    public bool ValidateLog { get; set; }
}
