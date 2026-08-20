using WacLexer.Exception;

namespace WacLexer;

public class Lexer
{
    public IEnumerable<Token> Tokenize(string source)
    {
        var state = new LexerState(0, 1, 1);

        while (state.Position < source.Length)
        {
            // WhiteSpace / NewLine character
            if (char.IsWhiteSpace(source[state.Position]))
            {
                // If return line, increment line and reset col
                if (source[state.Position] == '\n')
                {
                    state.Line++;
                    state.Column = 1;
                }
                else
                {
                    state.Column++;
                }

                state.Position++;
                continue;
            }

            if (source[state.Position] == '/')
            {
                if (Helper.Peek(source, state.Position) == '/')
                {
                    state.Position += 2;
                    state.Column += 2;

                    while (state.Position < source.Length && source[state.Position] != '\n')
                    {
                        state.Position++;
                    }

                    state.Position++;
                    state.Line++;
                    state.Column = 1;

                    continue;
                }

                if (Helper.Peek(source, state.Position) == '*')
                {
                    state.Position += 2;
                    state.Column += 2;
                    var commentEnd = false;

                    while (!commentEnd)
                    {
                        while (source[state.Position] != '*')
                        {
                            if (source[state.Position] == '\n')
                            {
                                state.Line++;
                                state.Column = 1;
                            }
                            else
                            {
                                state.Column++;
                            }

                            state.Position++;
                        }

                        if (state.Position >= source.Length || Helper.Peek(source, state.Position) == '/')
                        {
                            commentEnd = true;
                        }

                        state.Column++;
                        state.Position++;
                    }

                    state.Column++;
                    state.Position++;

                    continue;
                }
            }

            // Letter Tokenizer
            if (char.IsLetter(source[state.Position]) ||
                source[state.Position] == '_')
            {
                var start = state.Position;
                var position = state.Position;
                var current = source[state.Position];

                while (position < source.Length &&
                       char.IsLetter(current) ||
                       current == '_')
                {
                    position++;
                    current = source[position];
                }

                var word = source.Substring(start, position - start);
                var kind = word switch
                {
                    "if" => TokenKind.If,
                    "else" => TokenKind.Else,
                    "while" => TokenKind.While,
                    "return" => TokenKind.Return,
                    "void" => TokenKind.Void,
                    "int" => TokenKind.Int,
                    "float" => TokenKind.Float,
                    "string" => TokenKind.String,
                    "char" => TokenKind.Char,
                    _ => TokenKind.Id,
                };

                var token = new Token(kind, new(state.Line, state.Column), word);

                state.Position = position;
                state.Column += word.Length;

                yield return token;
                continue;
            }

            // Digit Tokenizer
            if (char.IsDigit(source[state.Position]))
            {
                var start = state.Position;
                var position = state.Position;
                var current = source[state.Position];

                while (position < source.Length &&
                       char.IsDigit(current) ||
                       current is '.' or 'e' or '_')
                {
                    position++;
                    current = source[position];
                }

                Token token;
                var word = source.Substring(start, position - start).Replace("_", string.Empty);

                if (word.Contains('.'))
                {
                    var count = word.Count(x => x == '.');

                    if (count > 1) throw new InvalidTokenException(new(state.Line, state.Column), word);

                    token = new Token(TokenKind.FloatLiteral, new(state.Line, state.Column), word);
                }
                else
                {
                    token = new Token(TokenKind.IntLiteral, new(state.Line, state.Column), word);
                }

                state.Position = position;
                state.Column += word.Length;

                yield return token;
                continue;
            }

            // DoubleQuote Tokenizer
            if (source[state.Position] == '"')
            {
                var start = state.Position;
                var position = state.Position;
                var current = source[state.Position];
                var count = 0;

                while (position < source.Length &&
                       count < 2)
                {
                    if (current == '"') count++;
                    if (Helper.IsEscape(current)) position++;

                    position++;
                    current = source[position];
                }

                var word = source.Substring(start, position - start);

                var token = new Token(TokenKind.StringLiteral, new(state.Line, state.Column), word);

                state.Position = position;
                state.Column += word.Length;

                yield return token;
                continue;
            }

            // Quote Tokenizer
            if (source[state.Position] == '\'')
            {
                var start = state.Position;
                var position = state.Position;
                var current = source[state.Position];
                var count = 0;

                while (position < source.Length &&
                       count < 2)
                {
                    if (current == '\'') count++;
                    if (Helper.IsEscape(current)) position++;

                    position++;
                    current = source[position];
                }

                Token token;
                var word = source.Substring(start, position - start);

                if (word.Length > 2 && Helper.IsEscape(word[1]))
                {
                    if (word is not (@"'\\'" or @"'\n'" or @"'\t'" or @"'\''"))
                        throw new InvalidTokenException(new(state.Line, state.Column), word);

                    token = new Token(TokenKind.CharLiteral, new(state.Line, state.Column), word);
                }
                else if (word.Length != 3)
                {
                    throw new InvalidTokenException(new(state.Line, state.Column), word);
                }
                else
                {
                    token = new Token(TokenKind.CharLiteral, new(state.Line, state.Column), word);
                }

                state.Position = position;
                state.Column += word.Length;

                yield return token;
                continue;
            }

            // Operator Tokenizer
            {
                Token token;

                switch (source[state.Position])
                {
                    case '+':
                    {
                        token = new Token(TokenKind.Plus, new(state.Line, state.Column), "+");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '-':
                    {
                        token = new Token(TokenKind.Minus, new(state.Line, state.Column), "-");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '*':
                    {
                        token = new Token(TokenKind.Star, new(state.Line, state.Column), "*");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '/':
                    {
                        token = new Token(TokenKind.Slash, new(state.Line, state.Column), "/");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '<':
                    {
                        if (Helper.Peek(source, state.Position) == '=')
                        {
                            token = new Token(TokenKind.LessOrEqualThan, new(state.Line, state.Column), "<=");
                            state.Position += 2;
                            state.Column += 2;
                            break;
                        }

                        token = new Token(TokenKind.LessThan, new(state.Line, state.Column), "<");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '>':
                    {
                        if (Helper.Peek(source, state.Position) == '=')
                        {
                            token = new Token(TokenKind.MoreOrEqualThan, new(state.Line, state.Column), ">=");
                            state.Position += 2;
                            state.Column += 2;
                            break;
                        }

                        token = new Token(TokenKind.MoreThan, new(state.Line, state.Column), ">");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '=':
                    {
                        if (Helper.Peek(source, state.Position) == '=')
                        {
                            token = new Token(TokenKind.EqualsTo, new(state.Line, state.Column), "==");
                            state.Position += 2;
                            state.Column += 2;
                            break;
                        }

                        token = new Token(TokenKind.Equal, new(state.Line, state.Column), "=");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '!':
                    {
                        if (Helper.Peek(source, state.Position) == '=')
                        {
                            token = new Token(TokenKind.NotEqualsTo, new(state.Line, state.Column), "!=");
                            state.Position += 2;
                            state.Column += 2;
                            break;
                        }

                        token = new Token(TokenKind.Not, new(state.Line, state.Column), "!");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case ';':
                    {
                        token = new Token(TokenKind.SemiColon, new(state.Line, state.Column), ";");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case ',':
                    {
                        token = new Token(TokenKind.Comma, new(state.Line, state.Column), ",");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '(':
                    {
                        token = new Token(TokenKind.LeftParenthesis, new(state.Line, state.Column), "(");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case ')':
                    {
                        token = new Token(TokenKind.RightParenthesis, new(state.Line, state.Column), ")");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '[':
                    {
                        token = new Token(TokenKind.LeftBracket, new(state.Line, state.Column), "[");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case ']':
                    {
                        token = new Token(TokenKind.RightBracket, new(state.Line, state.Column), "]");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '{':
                    {
                        token = new Token(TokenKind.LeftBrace, new(state.Line, state.Column), "{");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    case '}':
                    {
                        token = new Token(TokenKind.RightBrace, new(state.Line, state.Column), "}");
                        state.Position++;
                        state.Column++;
                        break;
                    }
                    default:
                    {
                        throw new UnknownTokenException(new(state.Line, state.Column),
                            source[state.Position].ToString());
                    }
                }

                yield return token;
                continue;
            }
        }

        yield return new Token(TokenKind.Eof, new(state.Line, state.Column), "EOF");
    }
}