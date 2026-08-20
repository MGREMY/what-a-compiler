using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static BlockStatementNode ParseBlock(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid block declaration";

        var blockNode = new BlockStatementNode
        {
            Position = state.Position,
        };

        // Need a '{' in 1st token
        if (TryPeekTo(tokens, state.Position, out var leftBraceToken)
            && leftBraceToken.Kind is TokenKind.LeftBrace)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftBraceToken);
        }

        while (state.Position < tokens.Length)
        {
            if (!TryPeekTo(tokens, state.Position, out var currentToken)
                || currentToken.Kind is TokenKind.Eof)
            {
                throw new ParsingException(exceptionMessage, currentToken);
            }

            if (currentToken.Kind is TokenKind.SemiColon)
            {
                state.Position++;
                continue;
            }

            if (currentToken.Kind is TokenKind.RightBrace)
            {
                state.Position++;
                break;
            }

            blockNode.Statements.Add(ParseStatement(tokens, ref state));
        }

        return blockNode;
    }
}