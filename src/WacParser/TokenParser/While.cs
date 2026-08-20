using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static WhileStatementNode ParseWhile(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid if declaration";

        var whileNode = new WhileStatementNode
        {
            Position = state.Position,
        };

        // Need a 'while' in 1st token
        if (TryPeekTo(tokens, state.Position, out var whileToken)
            && whileToken.Kind is TokenKind.While)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, whileToken);
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

        whileNode.Condition = ParseExpression(tokens, ref state);

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
            whileNode.Statement = ParseBlock(tokens, ref state);
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftBraceToken);
        }

        return whileNode;
    }
}