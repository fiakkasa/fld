using CommandLine;

using fld;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

Parser.Default.ParseArguments<Options>(args)
    .WithParsed(options =>
    {
        if (string.IsNullOrEmpty(options.FixLog))
        {
            Console.WriteLine("Please provide a FIX log string.");
            return;
        }

        Decoder.Decode(
            options.FixLog,
            options.Delimiter,
            options.ValidateLog,
            cts.Token
        ).Switch(
            entries => Print.AsMarkdown(entries, cts.Token),
            Console.WriteLine
        );
    });