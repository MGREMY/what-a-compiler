namespace Driver;

using WacLexer;

public static class Driver
{
    public static int Main()
    {
        var sourceCode = File.ReadAllText("Resources/CodeSample.wac");
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(sourceCode);

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        return 0;
    }
}