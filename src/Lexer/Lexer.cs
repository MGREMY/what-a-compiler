namespace Lexer;

public class Lexer
{
    private int _pos = 0; // Current pointer position into string variable
    private int _line = 1, _col = 1; // Current pointer Line and Column in a 2D char table used for Token creation
    private readonly string _source;

    public Lexer(string source)
    {
        _source = source;
    }

    public IEnumerable<Token> Tokenize()
    {
        while (_pos < _source.Length)
        {
            // WhiteSpace / NewLine character
            if (char.IsWhiteSpace(_source[_pos]))
            {
                // If return line, increment line and reset col
                if (_source[_pos].Equals('\n'))
                {
                    _line++;
                    _col = 1;
                }
                else
                {
                    _col++;
                }

                _pos++;
                continue;
            }

            if (char.IsLetter(_source[_pos]) || _source[_pos] == '_')
            {
                var start = _pos;

                while (char.IsLetter(Peek()) || Peek() == '_')
                {
                    _pos++;
                }

                _pos++;

                var word = _source.Substring(start, _pos - start);
                var kind = word switch
                {
                    "if" => TokenKind.If,
                    "else" => TokenKind.Else,
                    "return" => TokenKind.Return,
                    "void" => TokenKind.Void,
                    "int" => TokenKind.Int,
                    "float" => TokenKind.Float,
                    "string" => TokenKind.String,
                    "char" => TokenKind.Char,
                    _ => TokenKind.Id,
                };

                _col += word.Length;
                yield return new Token(kind, new TokenPosition(_line, _col), word);
                continue;
            }

            if (char.IsDigit(_source[_pos]))
            {
                var start = _pos;

                while (char.IsDigit(Peek()) || Peek() == '.' || Peek() == 'e')
                {
                    _pos++;
                }

                _pos++;

                var word = _source.Substring(start, _pos - start);

                _col += word.Length;

                yield return word.Contains('.')
                    ? new Token(TokenKind.FloatLiteral, new TokenPosition(_line, _col), word)
                    : new Token(TokenKind.IntLiteral, new TokenPosition(_line, _col), word);
                continue;
            }

            if (_source[_pos].Equals('"'))
            {
                var start = _pos;

                while (!Peek().Equals('"'))
                {
                    _pos++;
                }

                _pos += 2;

                var word = _source.Substring(start, _pos - start);

                _col += word.Length;

                yield return new Token(TokenKind.StringLiteral, new TokenPosition(_line, _col), word);
                continue;
            }

            if (_source[_pos].Equals('\''))
            {
                var start = _pos;

                while (!Peek().Equals('\''))
                {
                    _pos++;
                }

                _pos += 2;

                var word = _source.Substring(start, _pos - start);

                _col += word.Length;

                yield return new Token(TokenKind.CharLiteral, new TokenPosition(_line, _col), word);
                continue;
            }

            switch (_source[_pos])
            {
                case '+':
                {
                    yield return new Token(TokenKind.Plus, new TokenPosition(_line, _col), "+");
                    _pos++;
                    _col++;
                    break;
                }
                case '-':
                {
                    yield return new Token(TokenKind.Minus, new TokenPosition(_line, _col), "-");
                    _pos++;
                    _col++;
                    break;
                }
                case '*':
                {
                    yield return new Token(TokenKind.Star, new TokenPosition(_line, _col), "*");
                    _pos++;
                    _col++;
                    break;
                }
                // Be aware of comments sections
                case '/':
                {
                    // If double slash (//), go to the next line
                    if (Peek() == '/')
                    {
                        _pos += 2;
                        _col += 2;

                        while (_pos < _source.Length && _source[_pos] != '\n')
                        {
                            _pos++;
                        }

                        _pos++;
                        _line++;
                        _col = 1;

                        break;
                    }

                    // If slash star (/*), wait until star slash (*/) and release here
                    if (Peek() == '*')
                    {
                        _pos += 2;
                        _col += 2;
                        var commentEnd = false;

                        while (!commentEnd)
                        {
                            while (_source[_pos] != '*')
                            {
                                if (_source[_pos] == '\n')
                                {
                                    _line++;
                                    _col = 1;
                                }
                                else
                                {
                                    _col++;
                                }

                                _pos++;
                            }

                            if (_pos >= _source.Length || Peek() == '/')
                            {
                                commentEnd = true;
                            }

                            _col++;
                            _pos++;
                        }

                        _col++;
                        _pos++;

                        break;
                    }

                    yield return new Token(TokenKind.Slash, new TokenPosition(_line, _col), "/");
                    _pos++;
                    _col++;
                    break;
                }
                case '<':
                {
                    if (Peek() == '=')
                    {
                        yield return new Token(TokenKind.LessOrEqualThan, new TokenPosition(_line, _col), "<=");
                        _pos += 2;
                        _col += 2;
                        break;
                    }

                    yield return new Token(TokenKind.LessThan, new TokenPosition(_line, _col), "<");
                    _pos++;
                    _col++;
                    break;
                }
                case '>':
                {
                    if (Peek() == '=')
                    {
                        yield return new Token(TokenKind.MoreOrEqualThan, new TokenPosition(_line, _col), ">=");
                        _pos += 2;
                        _col += 2;
                        break;
                    }

                    yield return new Token(TokenKind.MoreThan, new TokenPosition(_line, _col), ">");
                    _pos++;
                    _col++;
                    break;
                }
                case '=':
                {
                    if (Peek() == '=')
                    {
                        yield return new Token(TokenKind.EqualsTo, new TokenPosition(_line, _col), "==");
                        _pos += 2;
                        _col += 2;
                        break;
                    }

                    yield return new Token(TokenKind.Equal, new TokenPosition(_line, _col), "=");
                    _pos++;
                    _col++;
                    break;
                }
                case '!':
                {
                    if (Peek() == '=')
                    {
                        yield return new Token(TokenKind.NotEqualsTo, new TokenPosition(_line, _col), "!=");
                        _pos += 2;
                        _col += 2;
                        break;
                    }

                    yield return new Token(TokenKind.Not, new TokenPosition(_line, _col), "!");
                    _pos++;
                    _col++;
                    break;
                }
                case ';':
                {
                    yield return new Token(TokenKind.SemiColon, new TokenPosition(_line, _col), ";");
                    _pos++;
                    _col++;
                    break;
                }
                case ',':
                {
                    yield return new Token(TokenKind.Comma, new TokenPosition(_line, _col), ",");
                    _pos++;
                    _col++;
                    break;
                }
                case '(':
                {
                    yield return new Token(TokenKind.LeftParenthesis, new TokenPosition(_line, _col), "(");
                    _pos++;
                    _col++;
                    break;
                }
                case ')':
                {
                    yield return new Token(TokenKind.RightParenthesis, new TokenPosition(_line, _col), ")");
                    _pos++;
                    _col++;
                    break;
                }
                case '[':
                {
                    yield return new Token(TokenKind.LeftBracket, new TokenPosition(_line, _col), "[");
                    _pos++;
                    _col++;
                    break;
                }
                case ']':
                {
                    yield return new Token(TokenKind.RightBracket, new TokenPosition(_line, _col), "]");
                    _pos++;
                    _col++;
                    break;
                }
                case '{':
                {
                    yield return new Token(TokenKind.LeftBrace, new TokenPosition(_line, _col), "{");
                    _pos++;
                    _col++;
                    break;
                }
                case '}':
                {
                    yield return new Token(TokenKind.RightBrace, new TokenPosition(_line, _col), "}");
                    _pos++;
                    _col++;
                    break;
                }
                default: throw new Exception("Invalid token");
            }
        }

        yield return new Token(TokenKind.Eof, new TokenPosition(_line, _col), "EOF");
    }

    /// <summary>
    /// Get the next character without moving the current position.
    ///
    /// Return '\0' if we reach the end of string.
    /// </summary>
    /// <returns>The next character.</returns>
    private char Peek()
    {
        return _pos < _source.Length - 1 ? _source[_pos + 1] : '\0';
    }
}