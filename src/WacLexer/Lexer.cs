using WacLexer.Tokenizer;

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
                if (source[state.Position].Equals('\n'))
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
                    state.Column++;

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

            Token token;

            if (char.IsLetter(source[state.Position]) || source[state.Position] == '_')
            {
                token = LetterTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (char.IsDigit(source[state.Position]))
            {
                token = DigitTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (source[state.Position].Equals('"'))
            {
                token = DoubleQuoteTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            if (source[state.Position].Equals('\''))
            {
                token = QuoteTokenizer.Tokenize(source, ref state);
                yield return token;
                continue;
            }

            token = OperatorTokenizer.Tokenize(source, ref state);
            yield return token;
        }

        yield return new Token(TokenKind.Eof, new TokenPosition(state.Line, state.Column), "EOF");
    }
}