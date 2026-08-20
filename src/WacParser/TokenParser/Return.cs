using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static ReturnStatementNode ParseReturn(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid return statement";

        var returnNode = new ReturnStatementNode
        {
            Position = state.Position,
        };

        // Need a 'return' in 1st token
        if (TryPeekTo(tokens, state.Position, out var returnToken)
            && returnToken.Kind is TokenKind.Return)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, returnToken);
        }

        returnNode.Expression = ParseExpression(tokens, ref state);

        return returnNode;
    }
}