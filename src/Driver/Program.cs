using System.Text.Json;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Driver.Helper;
using WacParser;

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
        var tokens = lexer.Tokenize(sourceCode).ToArray();
        FileHelper.CreateAndWriteToFile("out/lexer", JsonSerializer.Serialize(tokens));

        var parser = new Parser();
        var programAstNode = parser.Parse(tokens);
        FileHelper.CreateAndWriteToFile("out/parser", JsonSerializer.Serialize(programAstNode));

        return 0;
    }
}

[MinColumn, MaxColumn, MemoryDiagnoser]
public class DriverBenchmark
{
    [Benchmark]
    public int Start()
    {
        var sourceCode = File.ReadAllText("Resources/CodeSample.wac");

        var lexer = new Lexer();
        var tokens = lexer.Tokenize(sourceCode).ToArray();

        return 0;
    }
}