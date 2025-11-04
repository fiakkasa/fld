using CommandLine;

using fld;

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
            options.ValidateLog
        ).Switch(
            Print.AsMarkdown,
            Console.WriteLine
        );
    });