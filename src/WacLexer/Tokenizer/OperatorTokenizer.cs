namespace WacLexer.Tokenizer;

public static class OperatorTokenizer
{
    public static Token Tokenize(string source, ref LexerState state)
    {
        Token token;

        switch (source[state.Position])
        {
            case '+':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Plus, new TokenPosition(state.Line - 1, state.Column - 1), "+");
                break;
            }
            case '-':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Minus, new TokenPosition(state.Line - 1, state.Column - 1), "-");
                break;
            }
            case '*':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Star, new TokenPosition(state.Line - 1, state.Column - 1), "*");
                break;
            }
            case '/':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Slash, new TokenPosition(state.Line - 1, state.Column - 1), "/");
                break;
            }
            case '<':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    state.Position += 2;
                    state.Column += 2;
                    token = new Token(TokenKind.LessOrEqualThan, new TokenPosition(state.Line - 2, state.Column - 2),
                        "<=");
                    break;
                }

                state.Position++;
                state.Column++;
                token = new Token(TokenKind.LessThan, new TokenPosition(state.Line - 1, state.Column - 1), "<");
                break;
            }
            case '>':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    state.Position += 2;
                    state.Column += 2;
                    token = new Token(TokenKind.MoreOrEqualThan, new TokenPosition(state.Line - 2, state.Column - 2),
                        ">=");
                    break;
                }

                state.Position++;
                state.Column++;
                token = new Token(TokenKind.MoreThan, new TokenPosition(state.Line - 1, state.Column - 1), ">");
                break;
            }
            case '=':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    state.Position += 2;
                    state.Column += 2;
                    token = new Token(TokenKind.EqualsTo, new TokenPosition(state.Line - 2, state.Column - 2), "==");
                    break;
                }

                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Equal, new TokenPosition(state.Line - 1, state.Column - 1), "=");
                break;
            }
            case '!':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    state.Position += 2;
                    state.Column += 2;
                    token = new Token(TokenKind.NotEqualsTo, new TokenPosition(state.Line - 2, state.Column - 2), "!=");
                    break;
                }

                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Not, new TokenPosition(state.Line - 1, state.Column - 1), "!");
                break;
            }
            case ';':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.SemiColon, new TokenPosition(state.Line - 1, state.Column - 1), ";");
                break;
            }
            case ',':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.Comma, new TokenPosition(state.Line - 1, state.Column - 1), ",");
                break;
            }
            case '(':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.LeftParenthesis, new TokenPosition(state.Line - 1, state.Column - 1), "(");
                break;
            }
            case ')':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.RightParenthesis, new TokenPosition(state.Line - 1, state.Column - 1), ")");
                break;
            }
            case '[':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.LeftBracket, new TokenPosition(state.Line - 1, state.Column - 1), "[");
                break;
            }
            case ']':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.RightBracket, new TokenPosition(state.Line - 1, state.Column - 1), "]");
                break;
            }
            case '{':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.LeftBrace, new TokenPosition(state.Line - 1, state.Column - 1), "{");
                break;
            }
            case '}':
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.RightBrace, new TokenPosition(state.Line - 1, state.Column - 1), "}");
                break;
            }
            default:
            {
                state.Position++;
                state.Column++;
                token = new Token(TokenKind.InvalidToken, new TokenPosition(state.Line - 1, state.Column - 1),
                    source[state.Position - 1].ToString());
                break;
            }
        }

        return token;
    }
}