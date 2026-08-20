using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static FunctionNode ParseFunction(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid function declaration";

        var functionNode = new FunctionNode
        {
            Position = state.Position,
        };

        // Need a type in 1st token
        if (TryPeekTo(tokens, state.Position, out var typeToken)
            && typeToken.Kind
                is TokenKind.Void
                or TokenKind.Int
                or TokenKind.Float
                or TokenKind.String
                or TokenKind.Char)
        {
            state.Position++;
            functionNode.Type = typeToken.Text;
        }
        else
        {
            throw new ParsingException(exceptionMessage, typeToken);
        }

        // Need a name in 2nd token
        if (TryPeekTo(tokens, state.Position, out var idToken)
            && idToken.Kind is TokenKind.Id)
        {
            state.Position++;
            functionNode.Name = idToken.Text;
        }
        else
        {
            throw new ParsingException(exceptionMessage, idToken);
        }

        // Need a '(' in 3rd token
        if (TryPeekTo(tokens, state.Position, out var leftParenthesisToken)
            && leftParenthesisToken.Kind is TokenKind.LeftParenthesis)
        {
            state.Position++;
        }
        else
        {
            throw new ParsingException(exceptionMessage, leftParenthesisToken);
        }

        while (state.Position < tokens.Length)
        {
            if (!TryPeekTo(tokens, state.Position, out var currentToken)
                || currentToken.Kind is TokenKind.Eof)
            {
                throw new ParsingException(exceptionMessage, currentToken);
            }

            if (currentToken.Kind is TokenKind.Comma)
            {
                if (functionNode.Parameters.Count == 0)
                {
                    throw new ParsingException(exceptionMessage, currentToken);
                }

                state.Position++;
                continue;
            }

            if (currentToken.Kind is TokenKind.RightParenthesis)
            {
                state.Position++;
                break;
            }

            functionNode.Parameters.Add(ParseVariable(tokens, ref state));
        }

        if (TryPeekTo(tokens, state.Position, out var blockToken)
            && blockToken.Kind is TokenKind.LeftBrace)
        {
            functionNode.Block = ParseBlock(tokens, ref state);
        }
        else
        {
            throw new ParsingException(exceptionMessage, blockToken);
        }

        return functionNode;
    }
}