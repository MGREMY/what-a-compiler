using WacLexer;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static ProgramNode ParseProgram(Token[] tokens, ref ParserState state)
    {
        var programNode = new ProgramNode
        {
            Position = 0,
            Functions = [],
        };

        while (state.Position < tokens.Length)
        {
            if (!TryPeekTo(tokens, state.Position, out var currentToken)) break;
            if (currentToken.Kind is TokenKind.Eof) break;

            programNode.Functions.Add(ParseFunction(tokens, ref state));
        }

        return programNode;
    }
}