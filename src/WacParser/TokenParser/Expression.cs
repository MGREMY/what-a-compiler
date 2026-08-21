using WacLexer;
using WacParser.Exception;
using WacParser.NodeType;

namespace WacParser;

internal static partial class TokenParser
{
    public static ExpressionNode ParseExpression(Token[] tokens, ref ParserState state)
    {
        const string exceptionMessage = "Invalid expression declaration";

        if (!TryPeekTo(tokens, state.Position, out var currentToken))
        {
            throw new ParsingException(exceptionMessage, currentToken);
        }

        ExpressionNode? expressionNode = null;

        if (currentToken.Kind is TokenKind.IntLiteral)
            expressionNode = new IntLiteralExpressionNode
            {
                Position = state.Position,
                Value = currentToken.Text
            };

        else if (currentToken.Kind is TokenKind.FloatLiteral)
            expressionNode = new FloatLiteralExpressionNode
            {
                Position = state.Position,
                Value = currentToken.Text
            };

        else if (currentToken.Kind is TokenKind.CharLiteral)
            expressionNode = new CharLiteralExpressionNode
            {
                Position = state.Position,
                Value = currentToken.Text
            };

        else if (currentToken.Kind is TokenKind.StringLiteral)
            expressionNode = new StringLiteralExpressionNode
            {
                Position = state.Position,
                Value = currentToken.Text
            };

        else if (currentToken.Kind is TokenKind.BooleanLiteral)
            expressionNode = new BooleanLiteralExpressionNode
            {
                Position = state.Position,
                Value = currentToken.Text
            };

        else if (currentToken.Kind is TokenKind.Not)
            expressionNode = new UnaryExpressionNode
            {
                Position = state.Position,
                Operator = currentToken.Text,
                Operand = ParseExpression(tokens, ref state)
            };

        else if (currentToken.Kind is TokenKind.Id)
            expressionNode = new IdExpressionNode
            {
                Position = state.Position,
                Name = currentToken.Text
            };

        if (expressionNode is null)
            throw new ParsingException(exceptionMessage, currentToken);

        state.Position++;

        if (TryPeekTo(tokens, state.Position, out var nextToken))
        {
            if (nextToken.Kind
                is TokenKind.EqualsTo
                or TokenKind.NotEqualsTo
                or TokenKind.LessThan
                or TokenKind.LessOrEqualThan
                or TokenKind.MoreThan
                or TokenKind.MoreOrEqualThan)
            {
                state.Position++;

                expressionNode = new BinaryExpressionNode
                {
                    Position = state.Position - 1,
                    Left = expressionNode,
                    Right = ParseExpression(tokens, ref state),
                    Operator = nextToken.Text,
                };
            }

            else if (nextToken.Kind
                     is TokenKind.Plus
                     or TokenKind.Minus
                     or TokenKind.Star
                     or TokenKind.Slash)
            {
                state.Position++;

                expressionNode = new ArithmeticExpressionNode
                {
                    Position = state.Position,
                    Left = expressionNode,
                    Right = ParseExpression(tokens, ref state),
                    Operator = nextToken.Text,
                };
            }
        }

        return expressionNode;
    }
}