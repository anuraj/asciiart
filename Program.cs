using System.CommandLine;
using Figgle;
using System;
using System.Linq;
using Figgle.Fonts;

if (args.Length == 0)
{
    args = ["--help"];
}

var textArgument = new Argument<string>("text") { Description = "The text to generate ASCII art" };
var fontOption = new Option<string>("--font", ["-f"])
{
    Description = "The font to use for the ASCII art. For more information, see https://github.com/drewnoakes/figgle"
};
var randomFontOption = new Option<bool>("--random-font", ["-r"])
{
    Description = "Use a random font for the ASCII art"
};

var rootCommand = new RootCommand("A dotnet tool to generate ASCII art using the Figgle library. Find more information about the Figgle library at https://github.com/drewnoakes/figgle")
{
    textArgument,
    fontOption,
    randomFontOption
};

rootCommand.SetAction((result) =>
{
    var text = result.GetRequiredValue(textArgument);
    var font = result.GetValue(fontOption);
    var useRandomFont = result.GetValue(randomFontOption);

    var figgleFont = FiggleFonts.Standard;
    if (useRandomFont)
    {
        var random = new Random();
        var properties = typeof(FiggleFonts).GetProperties();
        var randomFont = properties.ElementAt(random.Next(0, properties.Length));
        Console.WriteLine($"Using random font: {randomFont.Name}");
        figgleFont = (FiggleFont)randomFont.GetValue(null);
    }
    else if (!string.IsNullOrEmpty(font))
    {
        var properties = typeof(FiggleFonts).GetProperties();
        var inputFont = properties.FirstOrDefault(p => p.Name.Equals(font, StringComparison.OrdinalIgnoreCase));
        if (inputFont != null)
        {
            figgleFont = (FiggleFont)inputFont.GetValue(null);
        }
    }

    if (!string.IsNullOrEmpty(text))
    {
        RenderText(text!, figgleFont);
    }
});

return rootCommand.Parse(args).Invoke();

static void RenderText(string text, FiggleFont figgleFont)
{
    Console.WriteLine();
    Console.WriteLine(figgleFont.Render(text));
}