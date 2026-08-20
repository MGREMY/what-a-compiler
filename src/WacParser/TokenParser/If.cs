using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static IfStatementNode ParseIf(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid if declaration";

        var ifNode = new IfStatementNode
        {
            Position = state.Position,
        };

        // Need an 'if' in 1st token
        if (TryPeekTo(tokens, state.Position, out var ifToken)
            && ifToken.Kind is TokenKind.If)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, ifToken);
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

        ifNode.Condition = ParseExpression(tokens, ref state);

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
            ifNode.Statement = ParseBlock(tokens, ref state);
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftBraceToken);
        }

        return ifNode;
    }
}