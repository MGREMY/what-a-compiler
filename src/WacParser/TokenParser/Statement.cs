using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static StatementNode ParseStatement(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid statement declaration";

        if (!TryPeekTo(tokens, state.Position, out var currentToken))
        {
            throw new ParsingException(exceptionMessage, currentToken);
        }

        StatementNode? statementNode = null;

        if (currentToken.Kind
            is TokenKind.Int
            or TokenKind.Float
            or TokenKind.Char
            or TokenKind.String)
        {
            statementNode = ParseVariable(tokens, ref state);

            if (!TryPeekTo(tokens, state.Position, out var nextToken)
                || nextToken.Kind is not TokenKind.SemiColon)
            {
                throw new ParsingException(exceptionMessage, currentToken);
            }
        }

        if (currentToken.Kind is TokenKind.Id)
        {
            statementNode = ParseAssignment(tokens, ref state);

            if (!TryPeekTo(tokens, state.Position, out var nextToken)
                || nextToken.Kind is not TokenKind.SemiColon)
            {
                throw new ParsingException(exceptionMessage, currentToken);
            }
        }

        if (currentToken.Kind is TokenKind.Return)
        {
            statementNode = ParseReturn(tokens, ref state);

            if (!TryPeekTo(tokens, state.Position, out var nextStatement)
                || nextStatement.Kind is not TokenKind.SemiColon)
            {
                throw new ParsingException(exceptionMessage, currentToken);
            }
        }

        if (currentToken.Kind is TokenKind.If)
            statementNode = ParseIf(tokens, ref state);

        if (currentToken.Kind is TokenKind.Elif)
            statementNode = ParseElif(tokens, ref state);

        if (currentToken.Kind is TokenKind.Else)
            statementNode = ParseElse(tokens, ref state);

        if (currentToken.Kind is TokenKind.While)
            statementNode = ParseWhile(tokens, ref state);

        if (currentToken.Kind is TokenKind.LeftBrace)
            statementNode = ParseBlock(tokens, ref state);

        if (statementNode is null)
            throw new ParsingException(exceptionMessage, currentToken);

        return statementNode;
    }
}