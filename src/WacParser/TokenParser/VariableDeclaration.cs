using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static VariableDeclarationStatementNode ParseVariableDeclaration(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid variable declaration";

        var variableDeclarationNode = new VariableDeclarationStatementNode
        {
            Position = state.Position,
        };

        // Need a type in 1st token
        if (TryPeekTo(tokens, state.Position, out var typeToken)
            && typeToken.Kind
                is TokenKind.Int
                or TokenKind.Float
                or TokenKind.String
                or TokenKind.Char
                or TokenKind.Boolean)
        {
            state.Position++;
            variableDeclarationNode.Type = typeToken.Text;
        }
        else
        {
            throw new ParsingException(exceptionMessage, typeToken);
        }

        var assignment = ParseAssignment(tokens, ref state);

        variableDeclarationNode.Name = assignment.Name;
        variableDeclarationNode.Initializer = assignment.Expression;

        return variableDeclarationNode;
    }
}