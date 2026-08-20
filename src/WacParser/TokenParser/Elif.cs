using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static ElifStatementNode ParseElif(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid elif declaration";

        var elifNode = new ElifStatementNode()
        {
            Position = state.Position,
        };

        // Need an 'elif' in 1st token
        if (TryPeekTo(tokens, state.Position, out var elifToken)
            && elifToken.Kind is TokenKind.Elif)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, elifToken);
        }

        // Need a '(' in 1st token
        if (TryPeekTo(tokens, state.Position, out var leftParenthesisToken)
            && leftParenthesisToken.Kind is TokenKind.LeftParenthesis)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftParenthesisToken);
        }

        elifNode.Condition = ParseExpression(tokens, ref state);

        // Need a ')' in 1st token
        if (TryPeekTo(tokens, state.Position, out var rightParenthesisToken)
            && rightParenthesisToken.Kind is TokenKind.RightParenthesis)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, rightParenthesisToken);
        }

        if (TryPeekTo(tokens, state.Position, out var leftBraceToken)
            && leftBraceToken.Kind is TokenKind.LeftBrace)
        {
            elifNode.Statement = ParseBlock(tokens, ref state);
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftBraceToken);
        }

        return elifNode;
    }
}