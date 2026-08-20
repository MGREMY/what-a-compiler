using WacLexer;
using WacParser.NodeType;

namespace WacParser;

public class Parser
{
    public ProgramNode Parse(Token[] tokens)
    {
        var state = new ParserState(0);

        var program = TokenParser.ParseProgram(tokens, ref state);

        return program;
    }
}