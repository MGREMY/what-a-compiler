using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static AssignmentStatementNode ParseAssignment(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid assignment declaration";

        var assignmentNode = new AssignmentStatementNode
        {
            Position = state.Position,
        };

        // Need a name in 1st token
        if (TryPeekTo(tokens, state.Position, out var idToken)
            && idToken.Kind is TokenKind.Id)
        {
            state.Position++;
            assignmentNode.Name = idToken.Text;
        }
        else
        {
            throw new ParsingException(exceptionMessage, idToken);
        }

        // If a '=' is present as a second token, create an expression
        if (TryPeekTo(tokens, state.Position, out var operatorToken)
            && operatorToken.Kind is TokenKind.Equal)
        {
            state.Position++;
            assignmentNode.Expression = ParseExpression(tokens, ref state);
        }

        return assignmentNode;
    }
}