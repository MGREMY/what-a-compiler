namespace Driver;

public static class Driver
{
    public static int Main()
    {
        var sourceCode = File.ReadAllText("Resources/CodeSample.wac");
        var lexer = new Lexer.Lexer(sourceCode);

        var tokens = lexer.Tokenize();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }

        return 0;
    }
}