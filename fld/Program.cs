using CommandLine;

using fld;
using fld.Models;

using var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    cts.Cancel();
    e.Cancel = true;
};

var result = Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
        Decoder.Decode(
            options.FixLog,
            options.Delimiter,
            options.ValidateLog,
            cts.Token
        ).Switch(
            entries => Print.AsMarkdown(entries, cts.Token),
            error =>
            {
                Console.WriteLine(error);
                Environment.Exit(1);
            }
        )
    );