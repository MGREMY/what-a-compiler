using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Driver;

using WacLexer;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length > 0 && args.Contains("BENCHMARK"))
        {
            BenchmarkRunner.Run<DriverBenchmark>();
        }
        else
        {
            var driver = new Driver();

            driver.Start();
        }

        return 0;
    }
}

public class Driver
{
    public int Start()
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

public class DriverBenchmark
{
    [Benchmark]
    public int Start()
    {
        var sourceCode = File.ReadAllText("Resources/CodeSample.wac");
        var lexer = new Lexer();

        var tokens = lexer.Tokenize(sourceCode).ToList();

        return 0;
    }
}