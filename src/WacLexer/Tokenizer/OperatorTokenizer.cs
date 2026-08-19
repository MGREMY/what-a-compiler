using WacLexer.Exception;

namespace WacLexer.Tokenizer;

internal class OperatorTokenizer : ITokenizer
{
    public bool CanTokenize(string source, ref LexerState state)
    {
        return true;
    }

    public Token Tokenize(string source, ref LexerState state)
    {
        Token token;

        switch (source[state.Position])
        {
            case '+':
            {
                token = new Token(TokenKind.Plus, new TokenPosition(state.Line, state.Column), "+");
                state.Position++;
                state.Column++;
                break;
            }
            case '-':
            {
                token = new Token(TokenKind.Minus, new TokenPosition(state.Line, state.Column), "-");
                state.Position++;
                state.Column++;
                break;
            }
            case '*':
            {
                token = new Token(TokenKind.Star, new TokenPosition(state.Line, state.Column), "*");
                state.Position++;
                state.Column++;
                break;
            }
            case '/':
            {
                token = new Token(TokenKind.Slash, new TokenPosition(state.Line, state.Column), "/");
                state.Position++;
                state.Column++;
                break;
            }
            case '<':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    token = new Token(TokenKind.LessOrEqualThan, new TokenPosition(state.Line, state.Column), "<=");
                    state.Position += 2;
                    state.Column += 2;
                    break;
                }

                token = new Token(TokenKind.LessThan, new TokenPosition(state.Line, state.Column), "<");
                state.Position++;
                state.Column++;
                break;
            }
            case '>':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    token = new Token(TokenKind.MoreOrEqualThan, new TokenPosition(state.Line, state.Column), ">=");
                    state.Position += 2;
                    state.Column += 2;
                    break;
                }

                token = new Token(TokenKind.MoreThan, new TokenPosition(state.Line, state.Column), ">");
                state.Position++;
                state.Column++;
                break;
            }
            case '=':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    token = new Token(TokenKind.EqualsTo, new TokenPosition(state.Line, state.Column), "==");
                    state.Position += 2;
                    state.Column += 2;
                    break;
                }

                token = new Token(TokenKind.Equal, new TokenPosition(state.Line, state.Column), "=");
                state.Position++;
                state.Column++;
                break;
            }
            case '!':
            {
                if (Helper.Peek(source, state.Position) == '=')
                {
                    token = new Token(TokenKind.NotEqualsTo, new TokenPosition(state.Line, state.Column), "!=");
                    state.Position += 2;
                    state.Column += 2;
                    break;
                }

                token = new Token(TokenKind.Not, new TokenPosition(state.Line, state.Column), "!");
                state.Position++;
                state.Column++;
                break;
            }
            case ';':
            {
                token = new Token(TokenKind.SemiColon, new TokenPosition(state.Line, state.Column), ";");
                state.Position++;
                state.Column++;
                break;
            }
            case ',':
            {
                token = new Token(TokenKind.Comma, new TokenPosition(state.Line, state.Column), ",");
                state.Position++;
                state.Column++;
                break;
            }
            case '(':
            {
                token = new Token(TokenKind.LeftParenthesis, new TokenPosition(state.Line, state.Column), "(");
                state.Position++;
                state.Column++;
                break;
            }
            case ')':
            {
                token = new Token(TokenKind.RightParenthesis, new TokenPosition(state.Line, state.Column), ")");
                state.Position++;
                state.Column++;
                break;
            }
            case '[':
            {
                token = new Token(TokenKind.LeftBracket, new TokenPosition(state.Line, state.Column), "[");
                state.Position++;
                state.Column++;
                break;
            }
            case ']':
            {
                token = new Token(TokenKind.RightBracket, new TokenPosition(state.Line, state.Column), "]");
                state.Position++;
                state.Column++;
                break;
            }
            case '{':
            {
                token = new Token(TokenKind.LeftBrace, new TokenPosition(state.Line, state.Column), "{");
                state.Position++;
                state.Column++;
                break;
            }
            case '}':
            {
                token = new Token(TokenKind.RightBrace, new TokenPosition(state.Line, state.Column), "}");
                state.Position++;
                state.Column++;
                break;
            }
            default:
            {
                throw new UnknownTokenException(new TokenPosition(state.Line, state.Column),
                    source[state.Position].ToString());
            }
        }

        return token;
    }
}