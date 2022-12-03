using System.CommandLine;
using Figgle;
using System;
using System.Linq;

if (args.Length == 0)
{
    args = new[] { "--help" };
}

var textArgument = new Argument<string>("text", "The text to generate ASCII art");
var fontOption = new Option<string>(new[] { "--font", "-f" }, () => "Standard", "The font to use for the ASCII art. For more information, see https://github.com/drewnoakes/figgle");
var randomFontOption = new Option<bool>(new[] { "--random-font", "-r" }, "Use a random font for the ASCII art");

var rootCommand = new RootCommand("A dotnet tool to generate ASCII art using the Figgle library. Find more information about the Figgle library at https://github.com/drewnoakes/figgle")
{
    textArgument,
    fontOption,
    randomFontOption
};

rootCommand.Name = "asciiart";
rootCommand.SetHandler((text, font, useRandomFont) =>
{
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

}, textArgument, fontOption, randomFontOption);

return await rootCommand.InvokeAsync(args);

static void RenderText(string text, FiggleFont figgleFont)
{
    Console.WriteLine();
    Console.WriteLine(figgleFont.Render(text));
}