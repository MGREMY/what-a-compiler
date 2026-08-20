using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static ElseStatementNode ParseElse(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid else declaration";

        var elseNode = new ElseStatementNode
        {
            Position = state.Position,
        };

        // Need an 'else' in 1st token
        if (TryPeekTo(tokens, state.Position, out var elseToken)
            && elseToken.Kind is TokenKind.Else)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, elseToken);
        }

        if (TryPeekTo(tokens, state.Position, out var leftBraceToken)
            && leftBraceToken.Kind is TokenKind.LeftBrace)
        {
            elseNode.Statement = ParseBlock(tokens, ref state);
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftBraceToken);
        }

        return elseNode;
    }
}